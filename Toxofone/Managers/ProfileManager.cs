namespace Toxofone.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using SharpTox.Av;
    using SharpTox.Core;
    using Toxofone.UI;

    public sealed class ProfileManager : IDisposable
    {
        private const string SearchFilePattern = "*" + ProfileInfo.DotExt;

        private static readonly object syncLock = new object();

        private static string profileDataPath;
        private static ProfileManager instance;

        private ProfileManager()
        {
        }

        #region IDisposable Support

        private bool disposed = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }

                if (this.ToxAv != null)
                {
                    this.ToxAv.Dispose();
                    this.ToxAv = null;
                }

                if (this.Tox != null)
                {
                    this.Tox.Dispose();
                    this.Tox = null;
                }

                this.disposed = true;
            }
        }

        ~ProfileManager()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public static string ProfileDataPath
        {
            get
            {
                lock (syncLock)
                {
                    if (profileDataPath == null)
                    {
                        profileDataPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    }

                    return profileDataPath;
                }
            }

        }

        public static ProfileManager Instance
        {
            get
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new ProfileManager();
                    }

                    return instance;
                }
            }
        }

        public ConnectionManager ConnectionManager { get; private set; }
        public CallManager CallManager { get; private set; }

        public Tox Tox { get; private set; }
        public ToxAv ToxAv { get; private set; }
        public ProfileInfo CurrentProfile { get; private set; }

        public ProfileInfo CreateNew(string userName, string statusMessage)
        {
            string profilePath = Path.Combine(ProfileDataPath, userName + ProfileInfo.DotExt);

            if (File.Exists(profilePath))
            {
                Logger.Log(LogLevel.Warning, "Attempt to override existing profile: " + userName);
                return null;
            }

            using (Tox tox = new Tox(ToxOptions.Default))
            {
                tox.Name = userName;
                tox.StatusMessage = statusMessage;

                try
                {
                    if (!Directory.Exists(ProfileDataPath))
                    {
                        Directory.CreateDirectory(ProfileDataPath);
                    }
                }
                catch
                {
                    return null;
                }

                if (!tox.GetData().Save(profilePath))
                {
                    return null;
                }
            }

            return new ProfileInfo(profilePath);
        }

        public void Save()
        {
            if (this.CurrentProfile == null || this.Tox == null)
            {
                Logger.Log(LogLevel.Warning, "Tried to save profile but there is no profile in use!");
                return;
            }

            try
            {
                if (!Directory.Exists(ProfileDataPath))
                {
                    Directory.CreateDirectory(ProfileDataPath);
                }

                if (this.Tox.GetData().Save(this.CurrentProfile.Path))
                {
                    Logger.Log(LogLevel.Info, "Saved profile to file");
                }
                else
                {
                    Logger.Log(LogLevel.Error, "Could not save profile");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Could not save profile: " + ex.ToString());
            }
        }

        public void SaveData(string newUserName = null, string newUserStatusMessage = null)
        {
            if (this.CurrentProfile == null || this.Tox == null)
            {
                Logger.Log(LogLevel.Warning, "Tried to save profile but there is no profile in use!");
                return;
            }

            try
            {
                if (!Directory.Exists(ProfileDataPath))
                {
                    Directory.CreateDirectory(ProfileDataPath);
                }

                string oldProfilePath = null;

                if (!string.IsNullOrEmpty(newUserName))
                {
                    string newProfilePath = Path.Combine(ProfileDataPath, newUserName + ProfileInfo.DotExt);

                    if (File.Exists(newProfilePath))
                    {
                        Logger.Log(LogLevel.Warning, "Attempt to override existing profile: " + newUserName);
                        return;
                    }

                    oldProfilePath = this.CurrentProfile.Path;
                    this.CurrentProfile = this.CreateNew(newUserName, newUserStatusMessage);
                }

                using (FileStream fs = new FileStream(this.CurrentProfile.Path, FileMode.Create))
                {
                    byte[] data = Tox.GetData().Bytes;
                    fs.Write(data, 0, data.Length);

                    Logger.Log(LogLevel.Info, "Saved profile to disk");
                }

                if (!string.IsNullOrEmpty(oldProfilePath))
                {
                    File.Delete(oldProfilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Could not save profile: " + ex.ToString());
            }
        }

        public static IEnumerable<ProfileInfo> GetAllProfiles()
        {
            try
            {
                return Directory.GetFiles(ProfileDataPath, SearchFilePattern, SearchOption.TopDirectoryOnly).
                        Where(s => s.EndsWith(ProfileInfo.DotExt)).
                        Select(p => new ProfileInfo(p));
            }
            catch
            {
                return new List<ProfileInfo>();
            }
        }

        public void SwitchTo(ProfileInfo profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException("profile");
            }

            ToxOptions options = ToxOptions.Default;
            options.Ipv6Enabled = Config.Instance.EnableIpv6;
            options.UdpEnabled = Config.Instance.EnableUdp;

            ToxData data = ToxData.FromDisk(profile.Path);
            if (data == null)
            {
                throw new Exception("Could not load profile.");
            }

            Tox tox = new Tox(options, data);
            ToxAv toxAv = new ToxAv(tox);
            try
            {
                this.InitManagers(tox, toxAv);
            }
            catch
            {
                toxAv.Dispose();
                tox.Dispose();
                throw;
            }

            if (this.Tox != null)
            {
                this.Tox.Dispose();
                this.Tox = null;
            }

            if (this.ToxAv != null)
            {
                this.ToxAv.Dispose();
                this.ToxAv = null;
            }

            this.Tox = tox;
            this.ToxAv = toxAv;

            this.Tox.OnFriendRequestReceived += this.OnToxFriendRequestReceived;
            this.Tox.OnFriendMessageReceived += this.OnToxFriendMessageReceived;
            this.Tox.OnFriendNameChanged += this.OnToxFriendNameChanged;
            this.Tox.OnFriendStatusMessageChanged += this.OnToxFriendStatusMessageChanged;
            this.Tox.OnFriendStatusChanged += this.OnToxFriendStatusChanged;
            this.Tox.OnFriendTypingChanged += this.OnToxFriendTypingChanged;
            this.Tox.OnFriendConnectionStatusChanged += this.OnToxFriendConnectionStatusChanged;
            this.Tox.OnReadReceiptReceived += this.OnToxReadReceiptReceived;

            this.Tox.OnFriendLossyPacketReceived += this.OnToxFriendLossyPacketReceived;
            this.Tox.OnFriendLosslessPacketReceived += this.OnToxFriendLosslessPacketReceived;

            this.Tox.OnGroupInvite += this.OnToxGroupInvite;
            this.Tox.OnGroupAction += this.OnToxGroupAction;
            this.Tox.OnGroupMessage += this.OnToxGroupMessage;
            this.Tox.OnGroupNamelistChange += this.OnToxGroupNamelistChange;
            this.Tox.OnGroupTitleChanged += this.OnToxGroupTitleChanged;

            this.Tox.OnFileSendRequestReceived += this.OnToxFileSendRequestReceived;
            this.Tox.OnFileControlReceived += this.OnToxFileControlReceived;
            this.Tox.OnFileChunkReceived += this.OnToxFileChunkReceived;
            this.Tox.OnFileChunkRequested += this.OnToxFileChunkRequested;

            this.CurrentProfile = profile;
        }

        public bool Login()
        {
            if (this.ConnectionManager == null || this.Tox == null || this.ToxAv == null)
            {
                return false;
            }

            Logger.Log(LogLevel.Info, "About to login to tox network");

            this.ConnectionManager.DoBootstrap();
            this.Tox.Start();
            this.ToxAv.Start();

            return true;
        }

        public void Logout()
        {
            if (this.ToxAv != null)
            {
                this.ToxAv.Dispose();
                this.ToxAv = null;
            }

            if (this.Tox != null)
            {
                this.Tox.Dispose();
                this.Tox = null;
            }

            Logger.Log(LogLevel.Info, "Tox network logout");
        }

        private void InitManagers(Tox tox, ToxAv toxAv)
        {
            this.ConnectionManager = this.InitManager(this.ConnectionManager, tox, toxAv);
            this.CallManager = this.InitManager(this.CallManager, tox, toxAv);
        }

        private T InitManager<T>(T mgr, Tox tox, ToxAv toxAv) where T : IToxManager, new()
        {
            if (mgr == null)
            {
                mgr = new T();
            }

            mgr.InitManager(tox, toxAv);
            return mgr;
        }

        private void OnToxFriendRequestReceived(object sender, ToxEventArgs.FriendRequestEventArgs e)
        {
            FriendRequestInfo request = new FriendRequestInfo(e.PublicKey.ToString(), e.Message);
            MainForm.Instance.NotifyToxFriendRequestReceived(request);
        }

        private void OnToxFriendMessageReceived(object sender, ToxEventArgs.FriendMessageEventArgs e)
        {
            MainForm.Instance.NotifyToxFriendMessageReceived(e);
        }

        private void OnToxFriendNameChanged(object sender, ToxEventArgs.NameChangeEventArgs e)
        {
            MainForm.Instance.NotifyToxFriendNameChanged(e);
        }

        private void OnToxFriendStatusMessageChanged(object sender, ToxEventArgs.StatusMessageEventArgs e)
        {
            MainForm.Instance.NotifyToxFriendStatusMessageChanged(e);
        }

        private void OnToxFriendStatusChanged(object sender, ToxEventArgs.StatusEventArgs e)
        {
            MainForm.Instance.NotifyToxFriendStatusChanged(e);
        }

        private void OnToxFriendTypingChanged(object sender, ToxEventArgs.TypingStatusEventArgs e)
        {
            MainForm.Instance.NotifyToxFriendTypingChanged(e);
        }

        private void OnToxFriendConnectionStatusChanged(object sender, ToxEventArgs.FriendConnectionStatusEventArgs e)
        {
            MainForm.Instance.NotifyToxFriendConnectionStatusChanged(e);
        }

        private void OnToxReadReceiptReceived(object sender, ToxEventArgs.ReadReceiptEventArgs e)
        {
            Logger.Log(LogLevel.Warning, "ToxReadReceiptReceived not supported, canceled");
        }

        private void OnToxFriendLossyPacketReceived(object sender, ToxEventArgs.FriendPacketEventArgs e)
        {
            Logger.Log(LogLevel.Warning, "ToxFriendLossyPacketReceived not supported, canceled");
        }

        private void OnToxFriendLosslessPacketReceived(object sender, ToxEventArgs.FriendPacketEventArgs e)
        {
            Logger.Log(LogLevel.Warning, "ToxFriendLosslessPacketReceived mot supported, canceled");
        }

        private void OnToxGroupInvite(object sender, ToxEventArgs.GroupInviteEventArgs e)
        {
            Logger.Log(LogLevel.Warning, "ToxGroupInvite not supported, ignored");
        }

        private void OnToxGroupAction(object sender, ToxEventArgs.GroupActionEventArgs e)
        {
            Logger.Log(LogLevel.Warning, "ToxGroupAction not supported, ignored");
        }

        private void OnToxGroupMessage(object sender, ToxEventArgs.GroupMessageEventArgs e)
        {
            Logger.Log(LogLevel.Warning, "ToxGroupMessage not supported, ignored");
        }

        private void OnToxGroupNamelistChange(object sender, ToxEventArgs.GroupNamelistChangeEventArgs e)
        {
            Logger.Log(LogLevel.Warning, "ToxGroupNamelistChange not supported, ignored");
        }

        private void OnToxGroupTitleChanged(object sender, ToxEventArgs.GroupTitleEventArgs e)
        {
            Logger.Log(LogLevel.Warning, "ToxGroupTitleChanged not supported, ignored");
        }

        private void OnToxFileSendRequestReceived(object sender, ToxEventArgs.FileSendRequestEventArgs e)
        {
            this.Tox.FileControl(e.FriendNumber, e.FileNumber, ToxFileControl.Cancel);
            Logger.Log(LogLevel.Warning, "ToxFileSendRequestReceived not supported, request canceled");
        }

        private void OnToxFileControlReceived(object sender, ToxEventArgs.FileControlEventArgs e)
        {
            this.Tox.FileControl(e.FriendNumber, e.FileNumber, ToxFileControl.Cancel);
            Logger.Log(LogLevel.Warning, "ToxFileControlReceived not supported, command canceled");
        }

        private void OnToxFileChunkReceived(object sender, ToxEventArgs.FileChunkEventArgs e)
        {
            this.Tox.FileControl(e.FriendNumber, e.FileNumber, ToxFileControl.Cancel);
            Logger.Log(LogLevel.Warning, "ToxFileChunkReceived not supported, canceled");
        }

        private void OnToxFileChunkRequested(object sender, ToxEventArgs.FileRequestChunkEventArgs e)
        {
            this.Tox.FileControl(e.FriendNumber, e.FileNumber, ToxFileControl.Cancel);
            Logger.Log(LogLevel.Warning, "ToxFileChunkRequested not supported, canceled");
        }
    }
}
