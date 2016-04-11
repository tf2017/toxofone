namespace Toxofone.UI
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.IO.Pipes;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using SharpTox.Core;
    using SharpTox.Av;
    using NAudio.Wave;
    using AForge.Video.DirectShow;
    using Toxofone.Devices;
    using Toxofone.Managers;
    using Toxofone.PowerInfo;
    using Toxofone.UI.PhoneBook;
    using Toxofone.UI.MediaControl;

    public enum FormLayout
    {
        None = 0,
        PortraitDown = 1,
        LandscapeRight = 2,
        LandscapeLeft = 3
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class MainForm : Form
    {
        private const int PortraitClientWidth = 640;
        private const int PortraitClientHeight = 720;
        private const int LandscapeClientWidth = 960;
        private const int LandscapeClientHeight = 480;

        private const int FixedPhoneBookSize = 10;

        private static object staticSyncLock = new object();
        private static MainForm instance;

        private bool disposed;

        private readonly object syncLock = new object();
        private Thread pipeMessageListener;

        private bool formShown;
        private FormLayout formLayout;

        private IntPtr hMonitorOn;
        private bool? monitorOn;

        private System.Windows.Forms.Timer callTimer;
        private CallTimerInfo callTimerInfo;

        private volatile bool localVideoEnabled;
        private volatile Image localVideoCameraLatestFrame;

        private volatile bool friendVideoEnabled;
        private volatile Image friendVideoCameraLatestFrame;

        internal MainForm()
        {
            this.InitializeComponent();
            this.UpdateComponent();
        }

        public static MainForm Instance
        {
            get
            {
                lock (staticSyncLock)
                {
                    if (instance == null)
                    {
                        throw new InvalidOperationException("MainForm.Instance is not set");
                    }

                    if (instance.IsDisposed)
                    {
                        throw new InvalidOperationException("MainForm.Instance is already disposed");
                    }

                    return instance;
                }
            }

            internal set
            {
                lock (staticSyncLock)
                {
                    if (value != null)
                    {
                        if (value.IsDisposed)
                        {
                            throw new InvalidOperationException("New MainForm instance is already disposed");
                        }

                        if (instance != null)
                        {
                            throw new InvalidOperationException("MainForm.Instance is already set");
                        }
                    }

                    instance = value;
                }
            }
        }

        public FormLayout FormLayout
        {
            get { return this.formLayout; }
            set
            {
                if (this.formLayout == value)
                {
                    return;
                }

                this.formLayout = value;

                if (this.formShown)
                {
                    this.RotateForm(this.formLayout);
                }

                Config.Instance.StartupLayout = Convert.ToInt32(this.formLayout);
                Config.Instance.Save();
            }
        }

        public void NotifyToxConnectionStatusChanged(ToxEventArgs.ConnectionStatusEventArgs e)
        {
            this.SafeInvokeAction<ToxEventArgs.ConnectionStatusEventArgs>(this.OnToxConnectionStatusChanged, e);
        }

        public void NotifyToxFriendMessageReceived(ToxEventArgs.FriendMessageEventArgs e)
        {
            this.SafeInvokeAction<ToxEventArgs.FriendMessageEventArgs>(this.OnToxFriendMessageReceived, e);
        }

        public void NotifyToxFriendNameChanged(ToxEventArgs.NameChangeEventArgs e)
        {
            this.SafeInvokeAction<ToxEventArgs.NameChangeEventArgs>(this.OnToxFriendNameChanged, e);
        }

        public void NotifyToxFriendStatusMessageChanged(ToxEventArgs.StatusMessageEventArgs e)
        {
            this.SafeInvokeAction<ToxEventArgs.StatusMessageEventArgs>(this.OnToxFriendStatusMessageChanged, e);
        }

        public void NotifyToxFriendStatusChanged(ToxEventArgs.StatusEventArgs e)
        {
            this.SafeInvokeAction<ToxEventArgs.StatusEventArgs>(this.OnToxFriendStatusChanged, e);
        }

        public void NotifyToxFriendTypingChanged(ToxEventArgs.TypingStatusEventArgs e)
        {
            this.SafeInvokeAction<ToxEventArgs.TypingStatusEventArgs>(this.OnToxFriendTypingChanged, e);
        }

        public void NotifyToxFriendConnectionStatusChanged(ToxEventArgs.FriendConnectionStatusEventArgs e)
        {
            this.SafeInvokeAction<ToxEventArgs.FriendConnectionStatusEventArgs>(this.OnToxFriendConnectionStatusChanged, e);
        }

        public void NotifyToxFriendRequestReceived(FriendRequestInfo request)
        {
            this.SafeInvokeAction<FriendRequestInfo>(this.OnToxFriendRequestReceived, request);
        }

        public void NotifyToxAvCallRequestReceived(ToxAvEventArgs.CallRequestEventArgs e)
        {
            this.SafeInvokeAction<ToxAvEventArgs.CallRequestEventArgs>(this.OnToxAvCallRequestReceived, e);
        }

        public void NotifyToxAvCallStateChanged(CallStateInfo callState)
        {
            this.SafeInvokeAction<CallStateInfo>(this.OnToxAvCallStateChanged, callState);
        }

        public void NotifyMicVolumeChanged(float volume)
        {
            this.SafeInvokeAction<float>(this.OnMicVolumeChanged, volume);
        }

        public void NotifyToxAvVideoFrameReceived(VideoFrameInfo videoFrame)
        {
            lock (this.syncLock)
            {
                this.friendVideoCameraLatestFrame = videoFrame.Frame;
                this.friendVideoCamera.Invalidate();
            }
        }

        public void NotifyNewLocalVideoFrameAvailable(Bitmap frame)
        {
            lock (this.syncLock)
            {
                this.localVideoCameraLatestFrame = frame;
                this.localVideoCamera.Invalidate();
            }
        }

        public void NotifyRingingTimeoutElapsed()
        {
            this.SafeInvokeAction(this.OnRingingTimeoutElapsed);
        }

        private void SafeInvokeAction<T>(Action<T> action, T args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<T>((newArgs) => { action(newArgs); }), args);
            }
            else
            {
                action(args);
            }
        }

        private void SafeInvokeAction(Action action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => { action(); }));
            }
            else
            {
                action();
            }
        }

        #region MainForm handlers

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_POWERBROADCAST:
                    this.OnPowerBroadcast(ref m);
                    break;
            }
            base.WndProc(ref m);
        }

        private void UpdateComponent()
        {
            this.formShown = false;
            this.formLayout = FormLayout.None;
            this.hMonitorOn = IntPtr.Zero;

            this.HandleCreated += this.OnMainFormHandleCreated;
            this.Load += this.OnMainFormLoad;
            this.Activated += this.OnMainFormActivated;
            this.Shown += this.OnMainFormShown;
            this.FormClosing += this.OnMainFormClosing;
            this.FormClosed += this.OnMainFormClosed;
            this.Deactivate += this.OnMainFormDeactivate;
            this.HandleDestroyed += this.OnMainFormHandleDestroyed;
            this.Disposed += this.OnMainFormDisposed;

            this.KeyPreview = true;
            this.KeyDown += this.OnMainFormKeyDown;

            this.controlPanel.CellPaint += this.OnControlPanelCellPaint;

            this.blockFriendVideo.PictureClick += this.OnBlockFriendVideoClick;
            this.playFriendVideo.PictureClick += this.OnPlayFriendVideoClick;
            this.endFriendCall.PictureClick += this.OnEndFriendCallClick;
            this.recordingDeviceStatus.PictureClick += this.OnRecordingDeviceStatusClick;
            this.playbackDeviceStatus.PictureClick += this.OnPlaybackDeviceStatusClick;
            this.ringingDeviceStatus.PictureClick += this.OnRingingDeviceStatusClick;
            this.videoDeviceStatus.PictureClick += this.OnVideoDeviceStatusClick;
            this.currentUserStatus.PictureClick += this.OnCurrentUserStatusClick;
            this.testPlaybackSound.PictureClick += this.OnTestPlaybackSoundClick;
            this.testRingingSound.PictureClick += this.OnTestRingingSoundClick;
            this.videoDeviceSettings.PictureClick += this.OnVideoDeviceSettingsClick;
            this.currentUserSettings.PictureClick += this.OnCurrentUserSettingsClick;

            this.friendVideoCamera.Paint += this.OnFriendVideoCameraPaint;
            this.localVideoCamera.Paint += this.OnLocalVideoCameraPaint;

            EmptyPhoneBookEntryControl emptyCtrl = new EmptyPhoneBookEntryControl();
            emptyCtrl.OnAddNewEntry += this.OnAddNewEntry;
            this.phoneBook.Controls.Add(emptyCtrl);

            this.recordingDeviceStatus.DeviceStatus = (DeviceManager.Instance.RecordingDevice != null) ? MediaDeviceStatus.On : MediaDeviceStatus.None;
            this.playbackDeviceStatus.DeviceStatus = (DeviceManager.Instance.PlaybackDevice != null) ? MediaDeviceStatus.On : MediaDeviceStatus.None;
            this.ringingDeviceStatus.DeviceStatus = (DeviceManager.Instance.RingingDevice != null) ? MediaDeviceStatus.On : MediaDeviceStatus.None;
            this.videoDeviceStatus.DeviceStatus = (DeviceManager.Instance.VideoDevice != null) ? MediaDeviceStatus.On : MediaDeviceStatus.None;
            this.currentUserStatus.ConnectionStatus = ToxConnectionStatus.None;
            this.currentUserStatus.UserStatus = ToxUserStatus.None;

            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var deviceCaps = WaveIn.GetCapabilities(i);
                this.recordingDeviceInfo.Add(deviceCaps.ProductName);
            }
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var deviceCaps = WaveOut.GetCapabilities(i);
                this.playbackDeviceInfo.Add(deviceCaps.ProductName);
            }
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var deviceCaps = WaveOut.GetCapabilities(i);
                this.ringingDeviceInfo.Add(deviceCaps.ProductName);
            }

            FilterInfoCollection availCameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo camera in availCameras)
            {
                this.videoDeviceInfo.Add(camera.Name);
            }

            this.recordingDeviceInfo.ProgressBarBackColor = Color.Honeydew;
            this.recordingDeviceInfo.DeviceVolume = 0;
            this.recordingDeviceInfo.DeviceName = (DeviceManager.Instance.RecordingDevice != null) ? DeviceManager.Instance.RecordingDevice.Name : string.Empty;
            this.recordingDeviceInfo.OnSelectedDeviceChanged += this.OnRecordingDeviceChanged;
            this.playbackDeviceInfo.DeviceName = (DeviceManager.Instance.PlaybackDevice != null) ? DeviceManager.Instance.PlaybackDevice.Name : string.Empty;
            this.playbackDeviceInfo.OnSelectedDeviceChanged += this.OnPlaybackDeviceChanged;
            this.ringingDeviceInfo.DeviceName = (DeviceManager.Instance.RingingDevice != null) ? DeviceManager.Instance.RingingDevice.Name : string.Empty;
            this.ringingDeviceInfo.OnSelectedDeviceChanged += this.OnRingingDeviceChanged;
            this.videoDeviceInfo.DeviceName = (DeviceManager.Instance.VideoDevice != null) ? DeviceManager.Instance.VideoDevice.Name : string.Empty;
            this.videoDeviceInfo.OnSelectedDeviceChanged += this.OnVideoDeviceChanged;
            this.currentUserInfo.UserName = ProfileManager.Instance.Tox.Name;
            this.currentUserInfo.UserStatusMessage = ProfileManager.Instance.Tox.StatusMessage;

            this.testPlaybackSound.Enabled = (DeviceManager.Instance.PlaybackDevice != null);
            this.testRingingSound.Enabled = (DeviceManager.Instance.RingingDevice != null);
            this.videoDeviceSettings.Enabled = (DeviceManager.Instance.VideoDevice != null);

            // ensure PhoneBook visible on start
            this.Text = "Toxofone";
            this.phoneBook.Visible = true;
            this.friendVideoCamera.Visible = false;
            this.blockFriendVideo.Visible = false;
            this.blockFriendVideo.Parent = this.friendVideoCamera;
            this.blockFriendVideo.BackColor = Color.Transparent;
            this.playFriendVideo.Visible = false;
            this.playFriendVideo.Parent = this.friendVideoCamera;
            this.playFriendVideo.BackColor = Color.Transparent;
            this.endFriendCall.Visible = false;
            this.endFriendCall.Parent = this.friendVideoCamera;  // to enable transparent background
            this.endFriendCall.BackColor = Color.Transparent;

            this.pipeMessageListener = new Thread(new ThreadStart(this.PipeMessageListenerProc));
            this.pipeMessageListener.IsBackground = true;
            this.pipeMessageListener.Start();

            this.callTimer = new System.Windows.Forms.Timer();
            this.callTimer.Enabled = false;
            this.callTimer.Interval = 100;
            this.callTimer.Tick += this.OnCallTimerTick;
        }

        private void PipeMessageListenerProc()
        {
            while (true)
            {
                try
                {
                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(App.SingleInstancePipeName,
                        PipeDirection.In, 1, PipeTransmissionMode.Byte))
                    {
                        while (true)
                        {
                            pipeServer.WaitForConnection();

                            using (StreamReader sr = new StreamReader(pipeServer, Encoding.ASCII, false, 1024, true))
                            {
                                String msgStr;
                                while ((msgStr = sr.ReadLine()) != null)
                                {
                                    this.BeginInvoke(new Action(() =>
                                    {
                                        this.Show();
                                        this.Visible = true;
                                        if (this.WindowState == FormWindowState.Minimized)
                                        {
                                            this.WindowState = FormWindowState.Normal;
                                        }
                                        this.Activate();
                                    }));
                                }
                            }

                            pipeServer.Disconnect();
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch
                {
                    continue;
                }
            }
        }

        private void OnMainFormHandleCreated(object sender, EventArgs e)
        {
            this.hMonitorOn = NativeMethods.RegisterPowerSettingNotification(this.Handle,
                ref NativeMethods.GUID_MONITOR_POWER_ON,
                NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);
        }

        private void OnMainFormLoad(object sender, EventArgs e)
        {
            try
            {
                ProfileManager.Instance.Login();

                foreach (int friend in ProfileManager.Instance.Tox.Friends)
                {
                    string name = ProfileManager.Instance.Tox.GetFriendName(friend);
                    string statusMessage = ProfileManager.Instance.Tox.GetFriendStatusMessage(friend);

                    EmptyPhoneBookEntryControl emptyCtrl = this.FindEmptyCtrl();
                    if (emptyCtrl != null)
                    {
                        int emptyCtrlIdx = this.phoneBook.Controls.IndexOf(emptyCtrl);
                        if (emptyCtrlIdx > FixedPhoneBookSize)
                        {
                            Logger.Log(LogLevel.Warning, "Phonebook is full, other phonebook entries ignored");
                            break;
                        }

                        FriendPhoneBookEntryControl friendCtrl = new FriendPhoneBookEntryControl();
                        friendCtrl.FriendNumber = friend;
                        friendCtrl.FriendName = string.IsNullOrEmpty(name) ? ProfileManager.Instance.Tox.GetFriendPublicKey(friend).ToString() : name;
                        friendCtrl.FriendStatusMessage = statusMessage;
                        friendCtrl.OnFriendInfoDoubleClicked += this.OnFriendInfoDoubleClicked;
                        friendCtrl.OnStartNewCall += this.OnStartNewCall;
                        friendCtrl.OnEndIncomingCall += this.OnEndIncomingCall;
                        friendCtrl.OnRemoveEntry += this.OnRemoveEntry;

                        this.phoneBook.Controls.Remove(emptyCtrl);
                        this.phoneBook.Controls.Add(friendCtrl);
                        this.phoneBook.Controls.Add(emptyCtrl);
                    }
                }

                bool localRecordingEnabled = ProfileManager.Instance.CallManager.RestartRecording();
                bool localPlaybackEnabled = ProfileManager.Instance.CallManager.RestartPlayback();
                bool localRingingEnabled = ProfileManager.Instance.CallManager.RestartRinging();
                this.localVideoEnabled = ProfileManager.Instance.CallManager.RestartLocalVideo();
                this.friendVideoEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);
            }
        }

        private void OnMainFormActivated(object sender, EventArgs e)
        {
            // do nothing
        }

        private void OnMainFormShown(object sender, EventArgs e)
        {
            if (this.formLayout == FormLayout.None)
            {
                this.formLayout = FormLayout.PortraitDown;
            }

            this.formShown = true;
            this.RotateForm(this.formLayout);
        }

        private void RotateForm(FormLayout toOrientation)
        {
            // adjust form size and location to make ClientSize equals of predefened values
            int deltaX = 0;
            int deltaY = 0;
            switch (toOrientation)
            {
                case FormLayout.PortraitDown:
                    deltaX = PortraitClientWidth - this.ClientSize.Width;
                    deltaY = PortraitClientHeight - this.ClientSize.Height;
                    break;

                case FormLayout.LandscapeRight:
                case FormLayout.LandscapeLeft:
                    deltaX = LandscapeClientWidth - this.ClientSize.Width;
                    deltaY = LandscapeClientHeight - this.ClientSize.Height;
                    break;

                default:
                    return;
            }

            this.SuspendLayout();

            this.Location = new Point(this.Location.X - deltaX / 2, this.Location.Y - deltaY / 2);
            this.Size = new Size(this.Size.Width + deltaX, this.Size.Height + deltaY);

            // rearrange controls to reflect new layout
            switch (toOrientation)
            {
                case FormLayout.PortraitDown:
                    this.topPanel.Location = new Point(0, 0);  // phonebook/friend video
                    this.leftBottomPanel.Location = new Point(0, this.topPanel.Height);  // controls
                    this.rightBottomPanel.Location = new Point(this.leftBottomPanel.Width, this.topPanel.Height);  // local video
                    break;

                case FormLayout.LandscapeRight:
                    this.topPanel.Location = new Point(0, 0);  // phonebook/friend video
                    this.rightBottomPanel.Location = new Point(this.topPanel.Width, 0);  // local video
                    this.leftBottomPanel.Location = new Point(this.topPanel.Width, this.rightBottomPanel.Height);  // controls
                    break;

                case FormLayout.LandscapeLeft:
                    this.rightBottomPanel.Location = new Point(0, 0);
                    this.leftBottomPanel.Location = new Point(0, this.rightBottomPanel.Height);
                    this.topPanel.Location = new Point(this.rightBottomPanel.Width, 0);
                    break;
            }
            this.PerformLayout();

            Logger.Log(LogLevel.Info, string.Format("ClientSize: {0}x{1}, orientation: {2}", this.ClientSize.Width, this.ClientSize.Height, toOrientation.ToString()));
        }

        private void OnMainFormClosing(object sender, FormClosingEventArgs e)
        {
            // do nothing
        }

        private void OnMainFormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                ProfileManager.Instance.CallManager.Kill();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);
            }
        }

        private void OnMainFormDeactivate(object sender, EventArgs e)
        {
            // do nothing
        }

        private void OnMainFormHandleDestroyed(object sender, EventArgs e)
        {
            if (this.hMonitorOn != IntPtr.Zero)
            {
                NativeMethods.UnregisterPowerSettingNotification(this.hMonitorOn);
                this.hMonitorOn = IntPtr.Zero;
            }
        }

        private void OnMainFormDisposed(object sender, EventArgs e)
        {
            if (this.disposed)
            {
                return;
            }

            ProfileManager.Instance.CallManager.Kill();

            if (this.hMonitorOn != IntPtr.Zero)
            {
                NativeMethods.UnregisterPowerSettingNotification(this.hMonitorOn);
                this.hMonitorOn = IntPtr.Zero;
            }

            this.disposed = true;
        }

        private void OnPowerBroadcast(ref Message m)
        {
            bool monitorOnHasValue = this.monitorOn.HasValue;
            bool monitorOnValue = this.monitorOn.HasValue ? this.monitorOn.Value : false;

            if ((int)m.WParam == NativeMethods.PBT_POWERSETTINGCHANGE)
            {
                // Respond to a change in the power settings.
                NativeMethods.POWERBROADCAST_SETTING ps =
                    (NativeMethods.POWERBROADCAST_SETTING)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.POWERBROADCAST_SETTING));

                // Handle a change to the power plan.
                if (ps.DataLength == Marshal.SizeOf(typeof(Int32)))
                {
                    IntPtr pData = (IntPtr)((int)m.LParam + Marshal.SizeOf(ps));
                    Int32 iData = (Int32)Marshal.PtrToStructure(pData, typeof(Int32));
                    if (ps.PowerSetting == NativeMethods.GUID_MONITOR_POWER_ON)
                    {
                        switch (iData)
                        {
                            case 0:
                                this.monitorOn = false;
                                break;
                            case 1:
                                this.monitorOn = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (this.monitorOn.HasValue != monitorOnHasValue)
            {
                this.OnMonitorPowerChanged(this.monitorOn.Value);
            }
            else if (this.monitorOn.HasValue && this.monitorOn.Value != monitorOnValue)
            {
                this.OnMonitorPowerChanged(this.monitorOn.Value);
            }
        }

        private void OnControlPanelCellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            switch (e.Row)
            {
                case 0: // recording device info
                case 1: // playback device info
                case 2: // ringing device info
                case 3: // video device info
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(225, 244, 225)))
                    {
                        e.Graphics.FillRectangle(brush, e.CellBounds);
                    }
                    break;

                case 4: // current user info
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(244, 244, 208)))
                    {
                        e.Graphics.FillRectangle(brush, e.CellBounds);
                    }
                    break;

                default:
                    break;
            }
        }

        private void OnBlockFriendVideoClick(object sender, EventArgs e)
        {
            // try to disable friend video
            if (!ProfileManager.Instance.CallManager.EnableFriendVideo(false))
            {
                return;
            }

            this.blockFriendVideo.SendToBack();
            this.blockFriendVideo.Visible = false;
            this.playFriendVideo.Visible = true;
            this.playFriendVideo.BringToFront();

            lock (this.syncLock)
            {
                this.friendVideoEnabled = false;
                this.friendVideoCameraLatestFrame = null;
                this.friendVideoCamera.Invalidate();
            }
        }

        private void OnPlayFriendVideoClick(object sender, EventArgs e)
        {
            // try to enable friend video
            if (!ProfileManager.Instance.CallManager.EnableFriendVideo(true))
            {
                return;
            }

            this.playFriendVideo.SendToBack();
            this.playFriendVideo.Visible = false;
            this.blockFriendVideo.Visible = true;
            this.blockFriendVideo.BringToFront();

            lock (this.syncLock)
            {
                this.friendVideoEnabled = true;
            }
        }

        private void OnEndFriendCallClick(object sender, EventArgs e)
        {
            if (this.endFriendCall.Tag != null && this.endFriendCall.Tag is int)
            {
                int friendNumber = (int)this.endFriendCall.Tag;
                this.EndCall(friendNumber);
            }
        }

        private void OnRecordingDeviceStatusClick(object sender, EventArgs e)
        {
            if (this.recordingDeviceStatus.DeviceStatus == MediaDeviceStatus.On)
            {
                this.recordingDeviceStatus.DeviceStatus = MediaDeviceStatus.Off;
            }
            else if (this.recordingDeviceStatus.DeviceStatus == MediaDeviceStatus.Off)
            {
                this.recordingDeviceStatus.DeviceStatus = MediaDeviceStatus.On;
            }

            bool recordingDeviceEnabled = (this.recordingDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            this.recordingDeviceInfo.DeviceVolume = 0;

            ProfileManager.Instance.CallManager.EnableRecording(recordingDeviceEnabled);
        }

        private void OnPlaybackDeviceStatusClick(object sender, EventArgs e)
        {
            if (this.playbackDeviceStatus.DeviceStatus == MediaDeviceStatus.On)
            {
                this.playbackDeviceStatus.DeviceStatus = MediaDeviceStatus.Off;
            }
            else if (this.playbackDeviceStatus.DeviceStatus == MediaDeviceStatus.Off)
            {
                this.playbackDeviceStatus.DeviceStatus = MediaDeviceStatus.On;
            }

            bool playbackDeviceEnabled = (this.playbackDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            this.testPlaybackSound.Enabled = playbackDeviceEnabled;

            ProfileManager.Instance.CallManager.EnablePlayback(playbackDeviceEnabled);
        }

        private void OnRingingDeviceStatusClick(object sender, EventArgs e)
        {
            if (this.ringingDeviceStatus.DeviceStatus == MediaDeviceStatus.On)
            {
                this.ringingDeviceStatus.DeviceStatus = MediaDeviceStatus.Off;
            }
            else if (this.ringingDeviceStatus.DeviceStatus == MediaDeviceStatus.Off)
            {
                this.ringingDeviceStatus.DeviceStatus = MediaDeviceStatus.On;
            }

            bool ringingDeviceEnabled = (this.ringingDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            this.testRingingSound.Enabled = ringingDeviceEnabled;

            ProfileManager.Instance.CallManager.EnableRinging(ringingDeviceEnabled);
        }

        private void OnVideoDeviceStatusClick(object sender, EventArgs e)
        {
            if (this.videoDeviceStatus.DeviceStatus == MediaDeviceStatus.On)
            {
                this.videoDeviceStatus.DeviceStatus = MediaDeviceStatus.Off;
            }
            else if (this.videoDeviceStatus.DeviceStatus == MediaDeviceStatus.Off)
            {
                this.videoDeviceStatus.DeviceStatus = MediaDeviceStatus.On;
            }

            bool videoDeviceEnabled = (this.videoDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            this.videoDeviceSettings.Enabled = videoDeviceEnabled;

            this.localVideoEnabled = ProfileManager.Instance.CallManager.EnableLocalVideo(videoDeviceEnabled);
            this.localVideoCameraLatestFrame = null;
            this.localVideoCamera.Invalidate();
        }

        private void OnCurrentUserStatusClick(object sender, EventArgs e)
        {
            switch (this.currentUserStatus.UserStatus)
            {
                case ToxUserStatus.None:
                case ToxUserStatus.Away:
                    this.currentUserStatus.UserStatus = ToxUserStatus.Busy;
                    break;
                case ToxUserStatus.Busy:
                    this.currentUserStatus.UserStatus = ToxUserStatus.None;
                    break;
                default:
                    break;
            }
            ProfileManager.Instance.Tox.Status = this.currentUserStatus.UserStatus;
        }

        private void OnMonitorPowerChanged(bool monitorOn)
        {
            switch (this.currentUserStatus.UserStatus)
            {
                case ToxUserStatus.None:
                    if (!monitorOn)
                        this.currentUserStatus.UserStatus = ToxUserStatus.Away;
                    break;
                case ToxUserStatus.Away:
                    if (monitorOn)
                        this.currentUserStatus.UserStatus = ToxUserStatus.None;
                    break;
                case ToxUserStatus.Busy:
                default:
                    break;
            }
            ProfileManager.Instance.Tox.Status = this.currentUserStatus.UserStatus;
        }

        private void OnRecordingDeviceChanged(int deviceIndex)
        {
            if (deviceIndex >= 0 && deviceIndex < WaveIn.DeviceCount)
            {
                Config.Instance.RecordingDeviceNumber = deviceIndex;
                Config.Instance.Save();

                var deviceCaps = WaveIn.GetCapabilities(deviceIndex);
                DeviceManager.Instance.RecordingDevice = new DeviceInfo(deviceIndex, deviceCaps.ProductName);

                switch (this.recordingDeviceStatus.DeviceStatus)
                {
                    case MediaDeviceStatus.None:
                        this.recordingDeviceStatus.DeviceStatus = MediaDeviceStatus.On;
                        break;
                    case MediaDeviceStatus.On:
                        break;
                    case MediaDeviceStatus.Off:
                        break;
                }
            }
            else
            {
                Config.Instance.RecordingDeviceNumber = null;
                Config.Instance.Save();

                DeviceManager.Instance.RecordingDevice = null;

                this.recordingDeviceStatus.DeviceStatus = MediaDeviceStatus.None;
            }

            bool recordingDeviceEnabled = (this.recordingDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            this.recordingDeviceInfo.DeviceVolume = 0;

            ProfileManager.Instance.CallManager.RestartRecording(recordingDeviceEnabled);
        }

        private void OnPlaybackDeviceChanged(int deviceIndex)
        {
            if (deviceIndex >= 0 && deviceIndex < WaveOut.DeviceCount)
            {
                Config.Instance.PlaybackDeviceNumber = deviceIndex;
                Config.Instance.Save();

                var deviceCaps = WaveOut.GetCapabilities(deviceIndex);
                DeviceManager.Instance.PlaybackDevice = new DeviceInfo(deviceIndex, deviceCaps.ProductName);

                switch (this.playbackDeviceStatus.DeviceStatus)
                {
                    case MediaDeviceStatus.None:
                        this.playbackDeviceStatus.DeviceStatus = MediaDeviceStatus.On;
                        break;
                    case MediaDeviceStatus.On:
                        break;
                    case MediaDeviceStatus.Off:
                        break;
                }
            }
            else
            {
                Config.Instance.PlaybackDeviceNumber = null;
                Config.Instance.Save();

                DeviceManager.Instance.PlaybackDevice = null;

                this.playbackDeviceStatus.DeviceStatus = MediaDeviceStatus.None;
            }

            bool playbackDeviceEnabled = (this.playbackDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            this.testPlaybackSound.Enabled = playbackDeviceEnabled;

            ProfileManager.Instance.CallManager.RestartPlayback(playbackDeviceEnabled);
        }

        private void OnRingingDeviceChanged(int deviceIndex)
        {
            if (deviceIndex >= 0 && deviceIndex < WaveOut.DeviceCount)
            {
                Config.Instance.RingingDeviceNumber = deviceIndex;
                Config.Instance.Save();

                var deviceCaps = WaveOut.GetCapabilities(deviceIndex);
                DeviceManager.Instance.RingingDevice = new DeviceInfo(deviceIndex, deviceCaps.ProductName);

                switch (this.ringingDeviceStatus.DeviceStatus)
                {
                    case MediaDeviceStatus.None:
                        this.ringingDeviceStatus.DeviceStatus = MediaDeviceStatus.On;
                        break;
                    case MediaDeviceStatus.On:
                        break;
                    case MediaDeviceStatus.Off:
                        break;
                }
            }
            else
            {
                Config.Instance.RingingDeviceNumber = null;
                Config.Instance.Save();

                DeviceManager.Instance.RingingDevice = null;

                this.ringingDeviceStatus.DeviceStatus = MediaDeviceStatus.None;
            }

            bool ringingDeviceEnabled = (this.ringingDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            this.testRingingSound.Enabled = ringingDeviceEnabled;

            ProfileManager.Instance.CallManager.RestartRinging(ringingDeviceEnabled);
        }

        private void OnVideoDeviceChanged(int deviceIndex)
        {
            string deviceName = this.videoDeviceInfo.FindDeviceName(deviceIndex);

            FilterInfoCollection availCameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FilterInfo selectedCamera = null;
            foreach (FilterInfo camera in availCameras)
            {
                if (string.CompareOrdinal(camera.Name, deviceName) == 0)
                {
                    selectedCamera = camera;
                    break;
                }
            }
            if (selectedCamera != null)
            {
                Config.Instance.VideoDeviceName = selectedCamera.Name;
                Config.Instance.Save();

                DeviceManager.Instance.VideoDevice = new DeviceInfo(deviceIndex, selectedCamera.Name);

                switch (this.videoDeviceStatus.DeviceStatus)
                {
                    case MediaDeviceStatus.None:
                        this.videoDeviceStatus.DeviceStatus = MediaDeviceStatus.On;
                        break;
                    case MediaDeviceStatus.On:
                        break;
                    case MediaDeviceStatus.Off:
                        break;
                }
            }
            else
            {
                Config.Instance.VideoDeviceName = string.Empty;
                Config.Instance.Save();

                DeviceManager.Instance.VideoDevice = null;

                this.videoDeviceStatus.DeviceStatus = MediaDeviceStatus.None;
            }

            bool videoDeviceEnabled = (this.videoDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            this.videoDeviceSettings.Enabled = videoDeviceEnabled;

            this.localVideoEnabled = ProfileManager.Instance.CallManager.RestartLocalVideo(videoDeviceEnabled);
            this.localVideoCameraLatestFrame = null;
            this.localVideoCamera.Invalidate();
        }

        private void OnTestPlaybackSoundClick(object sender, EventArgs e)
        {
            ProfileManager.Instance.CallManager.PlayTestPlaybackSoundOnce();
        }

        private void OnTestRingingSoundClick(object sender, EventArgs e)
        {
            ProfileManager.Instance.CallManager.PlayRingingSoundOnce();
        }

        private void OnVideoDeviceSettingsClick(object sender, EventArgs e)
        {
            try
            {
                ProfileManager.Instance.CallManager.DisplayPropertyWindow(this.Handle);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);
            }
        }

        private void OnCurrentUserSettingsClick(object sender, EventArgs e)
        {
            string currentUserName = ProfileManager.Instance.Tox.Name;
            string currentUserStatusMessage = ProfileManager.Instance.Tox.StatusMessage;
            string currentUserToxId = ProfileManager.Instance.Tox.Id.ToString();
            string newUserName = string.Empty;
            string newUserStatusMessage = string.Empty;

            using (EditCurrentUserDialog dlg = new EditCurrentUserDialog())
            {
                dlg.UserName = currentUserName;
                dlg.UserStatusMessage = currentUserStatusMessage;
                dlg.UserToxId = currentUserToxId;

                dlg.StartPosition = FormStartPosition.CenterParent;
                DialogResult dlgResult = dlg.ShowDialog(this);
                if (dlgResult != DialogResult.OK)
                {
                    return;
                }

                newUserName = dlg.UserName;
                newUserStatusMessage = dlg.UserStatusMessage;
            }

            bool needToSaveData = false;
            if (string.CompareOrdinal(newUserName, currentUserName) != 0)
            {
                ProfileManager.Instance.Tox.Name = newUserName;
                this.currentUserInfo.UserName = ProfileManager.Instance.Tox.Name;
                needToSaveData = true; 
            }
            else
            {
                newUserName = null;
            }
            if (string.CompareOrdinal(newUserStatusMessage, currentUserStatusMessage) != 0)
            {
                ProfileManager.Instance.Tox.StatusMessage = newUserStatusMessage;
                this.currentUserInfo.UserStatusMessage = ProfileManager.Instance.Tox.StatusMessage;
                needToSaveData = true;
            }
            if (needToSaveData)
            {
                ProfileManager.Instance.SaveData(newUserName, newUserStatusMessage);
            }
        }

        private void OnFriendVideoCameraPaint(object sender, PaintEventArgs e)
        {
            lock (this.syncLock)
            {
                if (!this.friendVideoEnabled)
                {
                    e.Graphics.DrawImage(this.friendVideoCamera.BackgroundImage,
                        new Rectangle(0, 0, this.friendVideoCamera.Width, this.friendVideoCamera.Height),
                        new Rectangle(0, 0, this.friendVideoCamera.BackgroundImage.Width, this.friendVideoCamera.BackgroundImage.Height),
                        GraphicsUnit.Pixel);
                }
                else if (this.friendVideoCameraLatestFrame != null)
                {
                    // Draw the latest image from the active camera
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawImage(this.friendVideoCameraLatestFrame,
                        new Rectangle(0, 0, this.friendVideoCamera.Width, this.friendVideoCamera.Height),
                        new Rectangle(0, 0, this.friendVideoCameraLatestFrame.Width, this.friendVideoCameraLatestFrame.Height),
                        GraphicsUnit.Pixel);
                }
            }
        }

        private void OnLocalVideoCameraPaint(object sender, PaintEventArgs e)
        {
            lock (this.syncLock)
            {
                if (!this.localVideoEnabled)
                {
                    e.Graphics.DrawImage(this.localVideoCamera.BackgroundImage,
                        new Rectangle(0, 0, this.localVideoCamera.Width, this.localVideoCamera.Height),
                        new Rectangle(0, 0, this.localVideoCamera.BackgroundImage.Width, this.localVideoCamera.BackgroundImage.Height),
                        GraphicsUnit.Pixel);
                }
                else if (this.localVideoCameraLatestFrame != null)
                {
                    // Draw the latest image from the active camera
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawImage(this.localVideoCameraLatestFrame,
                        new Rectangle(0, 0, this.localVideoCamera.Width, this.localVideoCamera.Height),
                        new Rectangle(0, 0, this.localVideoCameraLatestFrame.Width, this.localVideoCameraLatestFrame.Height),
                        GraphicsUnit.Pixel);
                }
            }
        }

        private void OnAddNewEntry()
        {
            try
            {
                EmptyPhoneBookEntryControl emptyCtrl = this.FindEmptyCtrl();
                if (emptyCtrl != null)
                {
                    int emptyCtrlIdx = this.phoneBook.Controls.IndexOf(emptyCtrl);
                    if (emptyCtrlIdx > FixedPhoneBookSize)
                    {
                        Logger.Log(LogLevel.Warning, "Phonebook is full");
                        MessageBox.Show(this, "Phonebook is full. You can not add a new entry.", "Add friend");
                        return;
                    }

                    string currentUserName = ProfileManager.Instance.Tox.Name;
                    string toxId = string.Empty;
                    string message = string.Format("{0} here! Could you tox with me?", currentUserName);

                    using (AddFriendDialog dlg = new AddFriendDialog())
                    {
                        dlg.ToxId = toxId;
                        dlg.Message = message;

                        dlg.StartPosition = FormStartPosition.CenterParent;
                        DialogResult dlgResult = dlg.ShowDialog(this);
                        if (dlgResult != DialogResult.OK)
                        {
                            return;
                        }

                        toxId = dlg.ToxId;
                        message = dlg.Message.Trim();
                    }

                    var error = ToxErrorFriendAdd.Ok;
                    int friendNumber = ProfileManager.Instance.Tox.AddFriend(new ToxId(toxId), message, out error);
                    if (error != ToxErrorFriendAdd.Ok)
                    {
                        MessageBox.Show(this, error.ToString(), "Add friend");
                        return;
                    }

                    FriendPhoneBookEntryControl friendCtrl = new FriendPhoneBookEntryControl();
                    friendCtrl.FriendNumber = friendNumber;
                    friendCtrl.FriendName = toxId;
                    friendCtrl.FriendStatusMessage = "Friend request sent";
                    friendCtrl.OnFriendInfoDoubleClicked += this.OnFriendInfoDoubleClicked;
                    friendCtrl.OnStartNewCall += this.OnStartNewCall;
                    friendCtrl.OnEndIncomingCall += this.OnEndIncomingCall;
                    friendCtrl.OnRemoveEntry += this.OnRemoveEntry;

                    this.phoneBook.Controls.Remove(emptyCtrl);
                    this.phoneBook.Controls.Add(friendCtrl);
                    this.phoneBook.Controls.Add(emptyCtrl);

                    ProfileManager.Instance.SaveData();
                }
                else
                {
                    Logger.Log(LogLevel.Error, "Empty phonebook entry not found");
                    MessageBox.Show(this, "Internal error: empty phonebook entry not found", "Add friend");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);
                MessageBox.Show(this, ex.Message, "Add friend");
            }
        }

        private void OnFriendRequestAccepted(FriendRequestInfo request)
        {
            FriendRequestPhoneBookEntryControl friendRequestCtrl = this.FindFriendRequestCtrl(request);
            if (friendRequestCtrl != null)
            {
                var error = ToxErrorFriendAdd.Ok;
                int friendNumber = ProfileManager.Instance.Tox.AddFriendNoRequest(new ToxKey(ToxKeyType.Public, request.PublicKey), out error);
                if (error != ToxErrorFriendAdd.Ok)
                {
                    Logger.Log(LogLevel.Error, "Failed to add friend: " + error);
                    return;
                }

                FriendPhoneBookEntryControl friendCtrl = new FriendPhoneBookEntryControl();
                friendCtrl.FriendNumber = friendNumber;
                friendCtrl.FriendName = ProfileManager.Instance.Tox.GetFriendPublicKey(friendNumber).ToString();
                friendCtrl.FriendStatusMessage = null;
                friendCtrl.OnFriendInfoDoubleClicked += this.OnFriendInfoDoubleClicked;
                friendCtrl.OnStartNewCall += this.OnStartNewCall;
                friendCtrl.OnEndIncomingCall += this.OnEndIncomingCall;
                friendCtrl.OnRemoveEntry += this.OnRemoveEntry;

                int friendRequestCtrlIdx = this.phoneBook.Controls.IndexOf(friendRequestCtrl);
                this.phoneBook.Controls.Remove(friendRequestCtrl);
                this.phoneBook.Controls.Add(friendCtrl);
                this.phoneBook.Controls.SetChildIndex(friendCtrl, friendRequestCtrlIdx);
            }
        }

        private void OnFriendRequestCanceled(FriendRequestInfo request)
        {
            FriendRequestPhoneBookEntryControl friendRequestCtrl = this.FindFriendRequestCtrl(request);
            if (friendRequestCtrl != null)
            {
                this.phoneBook.Controls.Remove(friendRequestCtrl);
            }
        }

        private void OnFriendInfoDoubleClicked(int friendNumber)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(friendNumber);
            if (friendCtrl != null)
            {
                friendCtrl.FriendMessage = string.Empty;
                friendCtrl.FriendAction = string.Empty;
                friendCtrl.FriendTypingStatus = false;
            }
        }

        private void OnMainFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                FriendPhoneBookEntryControl friendCtrl = this.FindFriendWithIncomingCall();
                if (friendCtrl != null)
                {
                    this.AcceptCallRequest(friendCtrl);
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Back)
            {
                if (this.endFriendCall.Visible && this.endFriendCall.Tag != null && this.endFriendCall.Tag is int)
                {
                    int friendNumber = (int)this.endFriendCall.Tag;
                    this.EndCall(friendNumber);
                    e.Handled = true;
                    return;
                }

                FriendPhoneBookEntryControl friendCtrl = this.FindFriendWithCall();
                if (friendCtrl != null)
                {
                    this.EndCall(friendCtrl.FriendNumber);
                    e.Handled = true;
                    return;
                }
            }
            else if (e.Control && e.KeyCode == Keys.Down)
            {
                this.FormLayout = FormLayout.PortraitDown;
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.Right)
            {
                this.FormLayout = FormLayout.LandscapeRight;
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.Left)
            {
                this.FormLayout = FormLayout.LandscapeLeft;
                e.Handled = true;
            }
        }

        private void OnStartNewCall(int friendNumber)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(friendNumber);
            if (friendCtrl != null)
            {
                if ((friendCtrl.CallState & CallState.None) != 0)
                {
                    this.SendCallRequest(friendCtrl);
                }
                else if ((friendCtrl.CallState & CallState.IncomingCall) != 0)
                {
                    this.AcceptCallRequest(friendCtrl);
                }
            }
        }

        private void OnEndIncomingCall(int friendNumber)
        {
            this.EndCall(friendNumber);
        }

        private void OnRingingTimeoutElapsed()
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendWithIncomingCall();
            if (friendCtrl != null)
            {
                this.EndCall(friendCtrl.FriendNumber);
            }
        }

        private void OnRemoveEntry(int friendNumber)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(friendNumber);
            if (friendCtrl != null)
            {
                String friendName = ProfileManager.Instance.Tox.GetFriendName(friendNumber);

                if (string.IsNullOrEmpty(friendName))
                {
                    friendName = friendCtrl.FriendName;
                }
                if (string.IsNullOrEmpty(friendName))
                {
                    friendName = "<no name>";
                }
                if (friendName.Length > 24)
                {
                    friendName = friendName.Substring(0, 24) + "...";
                }

                using (RemoveFriendDialog dlg = new RemoveFriendDialog())
                {
                    try
                    {
                        dlg.RtfMessage = string.Format(@"{{\rtf1\ansi Are you sure you want to remove\line \b {0} \b0  from your phone book?}}", friendName);
                    }
                    catch
                    {
                        dlg.Message = string.Format("Are you sure you want to remove\n{0} from your phone book?", friendName);
                    }

                    dlg.StartPosition = FormStartPosition.CenterParent;
                    DialogResult dlgResult = dlg.ShowDialog(this);
                    if (dlgResult != DialogResult.OK)
                    {
                        return;
                    }
                }

                ToxErrorFriendDelete error;
                bool result = ProfileManager.Instance.Tox.DeleteFriend(friendNumber, out error);
                if (result)
                {
                    this.phoneBook.Controls.Remove(friendCtrl);
                    ProfileManager.Instance.SaveData();
                }
                else
                {
                    Logger.Log(LogLevel.Error, string.Format("Error occured while deleting friend {0}: {1}", friendName, error.ToString()));
                    MessageBox.Show(this, string.Format("Error occured while deleting friend {0}: {1}", friendName, error.ToString()), "Remove friend");
                }
            }
        }

        private void SendCallRequest(FriendPhoneBookEntryControl friendCtrl)
        {
            // send audio/video call request
            bool enableRecording = (this.recordingDeviceStatus.DeviceStatus == MediaDeviceStatus.On);
            bool enablePlayback = (this.playbackDeviceStatus.DeviceStatus == MediaDeviceStatus.On);
            bool enableLocalVideo = (this.videoDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            if (ProfileManager.Instance.CallManager.StartCall(friendCtrl.FriendNumber, enableRecording, enablePlayback, enableLocalVideo, true))
            {
                friendCtrl.CallState = CallState.OutgoingCall | CallState.OutgoingVideo;
            }
        }

        private void AcceptCallRequest(FriendPhoneBookEntryControl friendCtrl)
        {
            // accept audio/video call request
            bool enableRecording = (this.recordingDeviceStatus.DeviceStatus == MediaDeviceStatus.On);
            bool enablePlayback = (this.playbackDeviceStatus.DeviceStatus == MediaDeviceStatus.On);
            bool enableLocalVideo = (this.videoDeviceStatus.DeviceStatus == MediaDeviceStatus.On);

            if (ProfileManager.Instance.CallManager.Answer(friendCtrl.FriendNumber, enableRecording, enablePlayback, enableLocalVideo, true))
            {
                friendCtrl.CallState = CallState.CallInProgress | CallState.IncomingVideo | CallState.OutgoingVideo;

                bool enableFriendVideo = ((friendCtrl.CallState & CallState.IncomingVideo) != 0);
                this.MakeFriendVideoCameraVisible(enableFriendVideo, friendCtrl.FriendNumber, friendCtrl.FriendName);
            }
        }

        private void EndCall(int friendNumber)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(friendNumber);
            if (friendCtrl != null)
            {
                if ((friendCtrl.CallState & CallState.IncomingCall) != 0 ||
                    (friendCtrl.CallState & CallState.OutgoingCall) != 0 ||
                    (friendCtrl.CallState & CallState.CallInProgress) != 0)
                {
                    // end audio/video call
                    if (ProfileManager.Instance.CallManager.Hangup(friendCtrl.FriendNumber))
                    {
                        friendCtrl.CallState = CallState.None;
                        
                        this.MakePhoneBookVisible();
                    }
                }
            }
        }

        private void MakePhoneBookVisible()
        {
            if (!this.phoneBook.Visible)
            {
                this.playFriendVideo.SendToBack();
                this.playFriendVideo.Visible = false;
                this.blockFriendVideo.SendToBack();
                this.blockFriendVideo.Visible = false;
                this.endFriendCall.SendToBack();
                this.endFriendCall.Visible = false;
                this.endFriendCall.Tag = null;
                this.friendVideoCamera.SendToBack();
                this.friendVideoCamera.Visible = false;
                this.friendVideoCameraLatestFrame = null;
                this.phoneBook.Visible = true;
                this.phoneBook.BringToFront();
                this.Text = "Toxofone";

                if (this.callTimer != null)
                {
                    this.callTimer.Stop();
                    this.callTimer.Enabled = false;
                    this.callTimerInfo = null;
                }
            }
        }

        private void MakeFriendVideoCameraVisible(bool enableFriendVideo, int friendNumber, string friendName)
        {
            if (!this.friendVideoCamera.Visible)
            {
                this.phoneBook.SendToBack();
                this.phoneBook.Visible = false;
                this.friendVideoCamera.Visible = true;
                this.friendVideoCamera.BringToFront();
                this.friendVideoCameraLatestFrame = null;
                this.endFriendCall.Visible = true;
                this.endFriendCall.BringToFront();
                this.endFriendCall.Tag = friendNumber;

                if (enableFriendVideo)
                {
                    this.blockFriendVideo.BringToFront();
                    this.blockFriendVideo.Visible = true;
                }

                this.Text = string.Format("Call with {0}", friendName);
                if (this.callTimer != null)
                {
                    this.callTimerInfo = new CallTimerInfo(DateTime.Now, friendName);
                    this.callTimer.Enabled = true;
                    this.callTimer.Start();
                }
            }
        }

        private void OnCallTimerTick(object sender, EventArgs e)
        {
            if (this.callTimerInfo != null)
            {
                this.Text = this.callTimerInfo.GetText();
            }
        }

#endregion

#region Tox/ToxAv event handlers

        private void OnToxConnectionStatusChanged(ToxEventArgs.ConnectionStatusEventArgs e)
        {
            this.currentUserStatus.ConnectionStatus = e.Status;
        }

        private void OnToxFriendRequestReceived(FriendRequestInfo request)
        {
            EmptyPhoneBookEntryControl emptyCtrl = this.FindEmptyCtrl();
            if (emptyCtrl != null)
            {
                int emptyCtrlIdx = this.phoneBook.Controls.IndexOf(emptyCtrl);
                if (emptyCtrlIdx > FixedPhoneBookSize)
                {
                    Logger.Log(LogLevel.Warning, "Phonebook is full, friend request ignored");
                    return;
                }

                FriendRequestPhoneBookEntryControl friendRequestCtrl = new FriendRequestPhoneBookEntryControl();
                friendRequestCtrl.RequestPublicKey = request.PublicKey;
                friendRequestCtrl.RequestMessage = request.Message;
                friendRequestCtrl.OnFriendRequestAccepted += this.OnFriendRequestAccepted;
                friendRequestCtrl.OnFriendRequestCanceled += this.OnFriendRequestCanceled;

                this.phoneBook.Controls.Remove(emptyCtrl);
                this.phoneBook.Controls.Add(friendRequestCtrl);
                this.phoneBook.Controls.Add(emptyCtrl);
            }
        }

        private void OnToxFriendMessageReceived(ToxEventArgs.FriendMessageEventArgs e)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(e.FriendNumber);
            if (friendCtrl != null)
            {
                switch (e.MessageType)
                {
                    case ToxMessageType.Message:
                        friendCtrl.FriendMessage = e.Message;
                        break;
                    case ToxMessageType.Action:
                        friendCtrl.FriendAction = e.Message;
                        break;
                }
            }
        }

        private void OnToxFriendNameChanged(ToxEventArgs.NameChangeEventArgs e)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(e.FriendNumber);
            if (friendCtrl != null)
            {
                friendCtrl.FriendName = e.Name;
            }
        }

        private void OnToxFriendStatusMessageChanged(ToxEventArgs.StatusMessageEventArgs e)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(e.FriendNumber);
            if (friendCtrl != null)
            {
                friendCtrl.FriendStatusMessage = e.StatusMessage;
            }
        }

        private void OnToxFriendStatusChanged(ToxEventArgs.StatusEventArgs e)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(e.FriendNumber);
            if (friendCtrl != null)
            {
                friendCtrl.FriendStatus = e.Status;
            }
        }

        private void OnToxFriendTypingChanged(ToxEventArgs.TypingStatusEventArgs e)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(e.FriendNumber);
            if (friendCtrl != null)
            {
                friendCtrl.FriendTypingStatus = e.IsTyping;
            }
        }

        private void OnToxFriendConnectionStatusChanged(ToxEventArgs.FriendConnectionStatusEventArgs e)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(e.FriendNumber);
            if (friendCtrl != null)
            {
                friendCtrl.FriendConnectionStatus = e.Status;
            }
        }

        private void OnToxAvCallRequestReceived(ToxAvEventArgs.CallRequestEventArgs e)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(e.FriendNumber);
            if (friendCtrl != null)
            {
#if !DEBUG
                this.WindowState = FormWindowState.Minimized;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
#endif

                // check if friend call can be answered automatically
                string friendName = friendCtrl.FriendName;
                if (Config.Instance.AutoConnectNames.IndexOf(friendName) >= 0)
                {
                    // incoming call can be answered right now, without ringing sound
                    this.AcceptCallRequest(friendCtrl);
                }
                else
                {
                    // incoming call received, play ringing sound
                    friendCtrl.CallState = e.VideoEnabled ? (CallState.IncomingCall | CallState.IncomingVideo) : CallState.IncomingCall;
                    ProfileManager.Instance.CallManager.StartRingingSound();
                }
            }
        }

        private void OnToxAvCallStateChanged(CallStateInfo callState)
        {
            FriendPhoneBookEntryControl friendCtrl = this.FindFriendCtrl(callState.FriendNumber);
            if (friendCtrl != null)
            {
                friendCtrl.CallState = callState.CallState;

                if ((callState.CallState & CallState.CallInProgress) != 0)
                {
                    bool enableFriendVideo = ((callState.CallState & CallState.IncomingVideo) != 0);
                    this.MakeFriendVideoCameraVisible(enableFriendVideo, friendCtrl.FriendNumber, friendCtrl.FriendName);
                }
                else
                {
                    this.MakePhoneBookVisible();
                }
            }
            else
            {
                this.MakePhoneBookVisible();
            }
        }

        private void OnMicVolumeChanged(float volume)
        {
            if (this.recordingDeviceStatus.DeviceStatus == MediaDeviceStatus.On)
            {
                this.recordingDeviceInfo.DeviceVolume = (int)volume;
            }
            else
            {
                this.recordingDeviceInfo.DeviceVolume = 0;
            }
        }

        private EmptyPhoneBookEntryControl FindEmptyCtrl()
        {
            EmptyPhoneBookEntryControl emptyCtrl = null;
            foreach (Control ctrl in this.phoneBook.Controls)
            {
                if (ctrl is EmptyPhoneBookEntryControl)
                {
                    emptyCtrl = (EmptyPhoneBookEntryControl)ctrl;
                    break;
                }
            }
            return emptyCtrl;
        }

        private FriendRequestPhoneBookEntryControl FindFriendRequestCtrl(FriendRequestInfo request)
        {
            FriendRequestPhoneBookEntryControl friendRequestCtrl = null;
            foreach (Control ctrl in this.phoneBook.Controls)
            {
                if (ctrl is FriendRequestPhoneBookEntryControl)
                {
                    friendRequestCtrl = (FriendRequestPhoneBookEntryControl)ctrl;

                    if (String.CompareOrdinal(friendRequestCtrl.RequestPublicKey, request.PublicKey) == 0 ||
                        String.CompareOrdinal(friendRequestCtrl.RequestMessage, request.Message) == 0)
                    {
                        break;
                    }

                    friendRequestCtrl = null;
                }
            }
            return friendRequestCtrl;
        }

        private FriendPhoneBookEntryControl FindFriendCtrl(int friendNumber)
        {
            FriendPhoneBookEntryControl friendCtrl = null;
            foreach (Control ctrl in this.phoneBook.Controls)
            {
                if (ctrl is FriendPhoneBookEntryControl)
                {
                    friendCtrl = (FriendPhoneBookEntryControl)ctrl;

                    if (friendCtrl.FriendNumber == friendNumber)
                    {
                        break;
                    }

                    friendCtrl = null;
                }
            }
            return friendCtrl;
        }

        private FriendPhoneBookEntryControl FindFriendWithCall()
        {
            FriendPhoneBookEntryControl friendCtrl = null;
            foreach (Control ctrl in this.phoneBook.Controls)
            {
                if (ctrl is FriendPhoneBookEntryControl)
                {
                    friendCtrl = (FriendPhoneBookEntryControl)ctrl;

                    if ((friendCtrl.CallState & CallState.IncomingCall) != 0 ||
                        (friendCtrl.CallState & CallState.OutgoingCall) != 0 ||
                        (friendCtrl.CallState & CallState.CallInProgress) != 0)
                    {
                        break;
                    }

                    friendCtrl = null;
                }
            }
            return friendCtrl;
        }

        private FriendPhoneBookEntryControl FindFriendWithIncomingCall()
        {
            FriendPhoneBookEntryControl friendCtrl = null;
            foreach (Control ctrl in this.phoneBook.Controls)
            {
                if (ctrl is FriendPhoneBookEntryControl)
                {
                    friendCtrl = (FriendPhoneBookEntryControl)ctrl;

                    if ((friendCtrl.CallState & CallState.IncomingCall) != 0)
                    {
                        break;
                    }

                    friendCtrl = null;
                }
            }
            return friendCtrl;
        }

        private class CallTimerInfo
        {
            public CallTimerInfo(DateTime startTime, string friendName)
            {
                this.StartTime = startTime;
                this.FriendName = friendName;
            }

            public string FriendName { get; private set; }

            public DateTime StartTime { get; private set; }

            public string GetText()
            {
                TimeSpan elapsedTime = DateTime.Now - this.StartTime;
                if (elapsedTime.Hours > 0)
                {
                    return string.Format("Call with {0} {1:hh\\:mm\\:ss}", this.FriendName, elapsedTime);
                }
                else
                {
                    return string.Format("Call with {0} {1:mm\\:ss}", this.FriendName, elapsedTime);
                }
            }
        }

#endregion
    }
}
