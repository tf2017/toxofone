namespace Toxofone
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using NAudio;
    using NAudio.Wave;
    using AForge.Video.DirectShow;
    using SharpTox;
    using SharpTox.Av;
    using SharpTox.Core;
    using Svg;
    using Toxofone.Devices;
    using Toxofone.Managers;
    using Toxofone.UI;

    public static class App
    {
        public const string SingleInstanceMutexName = "$TOXOFONE_MUTEX$";

        private static Mutex globalMutex = null;

#if TOXOFONE_PORTABLE
        static App()
        {
            try
            {
                EmbeddedAssembly.LoadAssembly("Toxofone.AForge.dll", "AForge.dll");
                EmbeddedAssembly.LoadAssembly("Toxofone.AForge.Video.dll", "AForge.Video.dll");
                EmbeddedAssembly.LoadAssembly("Toxofone.AForge.Video.DirectShow.dll", "AForge.Video.DirectShow.dll");
                EmbeddedAssembly.LoadAssembly("Toxofone.NAudio.dll", "NAudio.dll");
                EmbeddedAssembly.LoadAssembly("Toxofone.SharpTox.dll", "SharpTox.dll");
                EmbeddedAssembly.LoadAssembly("Toxofone.Svg.dll", "Svg.dll");
                EmbeddedAssembly.LoadDll("Toxofone.libtox.dll", "libtox.dll");
                EmbeddedAssembly.LoadDll("Toxofone.tfutils.dll", "tfutils.dll");
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(App.OnCurrentDomainAssemblyResolve);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Toxofone startup");
                Environment.Exit(-1);
            }
        }

        private static Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.GetAssembly(args.Name);
        }
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // check if another application instance is running currently
            bool result = false;
            Mutex mutex = new Mutex(true, SingleInstanceMutexName, out result);
            if (!result)
            {
                Process current = Process.GetCurrentProcess();

                Logger.Log(LogLevel.Warning, string.Format("Another {0} instance started. Previous instance is about to be activated", current.ProcessName));

                // Try to show and activate previous instance
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        // we have found previous instance
                        App.ActivateInstance(process.MainWindowHandle);

                        try
                        {
                            EventWaitHandle activateEvent = null;
                            if (EventWaitHandle.TryOpenExisting(MainForm.ActivateMessageEventName, out activateEvent))
                            {
                                activateEvent.Set();
                            }
                        }
                        catch
                        {
                            // ignore
                        }

                        Logger.Log(LogLevel.Info, string.Format("Previous {0} instance activated", process.ProcessName));

                        break;
                    }
                }

                Environment.Exit(1);
                return;
            }

            globalMutex = mutex;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.ApplicationExit += App.OnApplicationExit;
            Application.ThreadException += App.OnThreadException;

            try
            {
                NAudioLogger.Instance = new NAudioLoggerImpl();
                AForgeLogger.Instance = new AForgeLoggerImpl();
                SharpToxLogger.Instance = new SharpToxLoggerImpl();
                SvgLogger.Instance = new SvgLoggerImpl();

                Logger.CleanupLogs();
                Config.Instance.Reload();
                ConfigStartupLogLevel();

                Logger.Log(LogLevel.Info, "==================");
                Logger.Log(LogLevel.Info, "Tox version:      " + ToxVersion.Current.ToString());
                Logger.Log(LogLevel.Info, "ToxAv version:    " + ToxAvVersion.Current.ToString());
                Logger.Log(LogLevel.Info, "Toxofone version: " + Assembly.GetExecutingAssembly().GetName().Version);

                int commWaveInId = -1,
                    commWaveOutId = -1,
                    mmWaveOutId = -1;
                NativeMethods.GetWaveInId(NativeMethods.ERole_eCommunications, out commWaveInId);
                NativeMethods.GetWaveOutId(NativeMethods.ERole_eCommunications, out commWaveOutId);
                NativeMethods.GetWaveOutId(NativeMethods.ERole_eMultimedia, out mmWaveOutId);

                DeviceManager.Instance.RecordingDevice = App.PrepareRecordingDevice(Config.Instance.RecordingDeviceNumber, commWaveInId);
                DeviceManager.Instance.PlaybackDevice = App.PreparePlaybackDevice(Config.Instance.PlaybackDeviceNumber, commWaveOutId);
                DeviceManager.Instance.RingingDevice = App.PrepareRingingDevice(Config.Instance.RingingDeviceNumber, mmWaveOutId);
                DeviceManager.Instance.VideoDevice = App.PrepareVideoDevice(Config.Instance.VideoDeviceName);

                using (ProfileManager profileMgr = ProfileManager.Instance)
                {
                    // select first available user profile, this decision was made by design, 
                    // multiple profiles are not supported
                    ProfileInfo userProfile = null;
                    foreach (ProfileInfo profile in ProfileManager.GetAllProfiles())
                    {
                        userProfile = profile;
                        break;
                    }

                    if (userProfile == null)
                    {
                        string newUserName = string.Empty;
                        string newUserStatusMessage = string.Empty;

                        using (EditCurrentUserDialog dlg = new EditCurrentUserDialog())
                        {
                            dlg.Text = "New user";
                            dlg.UserName = string.Empty;
                            dlg.UserStatusMessage = "Toxing on Toxofone";
                            dlg.UserToxId = string.Empty;

                            dlg.StartPosition = FormStartPosition.CenterScreen;
                            DialogResult dlgResult = dlg.ShowDialog();
                            if (dlgResult != DialogResult.OK)
                            {
                                Application.Exit();
                                return;
                            }

                            newUserName = dlg.UserName;
                            newUserStatusMessage = dlg.UserStatusMessage;
                        }

                        userProfile = ProfileManager.Instance.CreateNew(newUserName, newUserStatusMessage);
                    }

                    ProfileManager.Instance.SwitchTo(userProfile);
                    try
                    {
                        Logger.Log(LogLevel.Info, "Tox ID: " + ProfileManager.Instance.Tox.Id.ToString());

                        using (MainForm mainForm = new MainForm())
                        {
                            MainForm.Instance = mainForm;
                            ConfigStartupLayout(mainForm);
                            try
                            {
                                Application.Run(mainForm);
                            }
                            finally
                            {
                                MainForm.Instance = null;
                            }
                        }
                    }
                    finally
                    {
                        ProfileManager.Instance.Logout();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.ToString());
                MessageBox.Show(ex.Message, "Toxofone");
            }
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Log(LogLevel.Error, e.Exception.ToString());
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            Config.Instance.Save();
            ProfileManager.Instance.Save();
        }

        private static void ConfigStartupLogLevel()
        {
            if (Config.Instance.StartupLogLevel.HasValue)
            {
                if (Config.Instance.StartupLogLevel.Value == Convert.ToInt32(LogLevel.Error))
                {
                    Logger.SetLogLevel(LogLevel.Error);
                }
                else if (Config.Instance.StartupLogLevel.Value == Convert.ToInt32(LogLevel.Warning))
                {
                    Logger.SetLogLevel(LogLevel.Warning);
                }
                else if (Config.Instance.StartupLogLevel.Value == Convert.ToInt32(LogLevel.Info))
                {
                    Logger.SetLogLevel(LogLevel.Info);
                }
                else if (Config.Instance.StartupLogLevel.Value == Convert.ToInt32(LogLevel.Verbose))
                {
                    Logger.SetLogLevel(LogLevel.Verbose);
                }
            }
        }

        private static void ConfigStartupLayout(MainForm mainForm)
        {
            if (Config.Instance.StartupLayout.HasValue)
            {
                if (Config.Instance.StartupLayout.Value == Convert.ToInt32(FormLayout.PortraitDown))
                {
                    mainForm.FormLayout = FormLayout.PortraitDown;
                }
                else if (Config.Instance.StartupLayout.Value == Convert.ToInt32(FormLayout.LandscapeRight))
                {
                    mainForm.FormLayout = FormLayout.LandscapeRight;

                }
                else if (Config.Instance.StartupLayout.Value == Convert.ToInt32(FormLayout.LandscapeLeft))
                {
                    mainForm.FormLayout = FormLayout.LandscapeLeft;
                }
            }
        }

        private static DeviceInfo PrepareRecordingDevice(int? configDeviceNumber, int defaultDeviceNumber)
        {
            DeviceInfo di = App.LoadRecordingDeviceInfo(configDeviceNumber);
            if (di == null)
            {
                di = App.SelectPreferredRecordingDevice(defaultDeviceNumber);
            }

            return di;
        }

        private static DeviceInfo PreparePlaybackDevice(int? configDeviceNumber, int defaultDeviceNumber)
        {
            DeviceInfo di = App.LoadPlaybackDeviceInfo(configDeviceNumber);
            if (di == null)
            {
                di = App.SelectPreferredPlaybackDevice(defaultDeviceNumber);
            }

            return di;
        }

        private static DeviceInfo PrepareRingingDevice(int? configDeviceNumber, int defaultDeviceNumber)
        {
            DeviceInfo di = App.LoadRingingDeviceInfo(configDeviceNumber);
            if (di == null)
            {
                di = App.SelectPreferredRingingDevice(defaultDeviceNumber);
            }

            return di;
        }

        private static DeviceInfo PrepareVideoDevice(string configDeviceName)
        {
            DeviceInfo di = App.LoadVideoDeviceInfo(configDeviceName);
            if (di == null)
            {
                di = App.SelectPreferredVideoDevice();
            }

            return di;
        }

        private static DeviceInfo LoadRecordingDeviceInfo(int? configDeviceNumber)
        {
            if (!configDeviceNumber.HasValue)
            {
                return null;
            }

            int deviceNumber = configDeviceNumber.Value;
            if (deviceNumber >= 0 && deviceNumber < WaveIn.DeviceCount)
            {
                var deviceCaps = WaveIn.GetCapabilities(deviceNumber);
                return new DeviceInfo(deviceNumber, deviceCaps.ProductName);
            }

            return null;
        }

        private static DeviceInfo LoadPlaybackDeviceInfo(int? configDeviceNumber)
        {
            if (!configDeviceNumber.HasValue)
            {
                return null;
            }

            int deviceNumber = configDeviceNumber.Value;
            if (deviceNumber >= 0 && deviceNumber < WaveOut.DeviceCount)
            {
                var deviceCaps = WaveOut.GetCapabilities(deviceNumber);
                return new DeviceInfo(deviceNumber, deviceCaps.ProductName);
            }

            return null;
        }

        private static DeviceInfo LoadRingingDeviceInfo(int? configDeviceNumber)
        {
            if (!configDeviceNumber.HasValue)
            {
                return null;
            }

            int deviceNumber = configDeviceNumber.Value;
            if (deviceNumber >= 0 && deviceNumber < WaveOut.DeviceCount)
            {
                var deviceCaps = WaveOut.GetCapabilities(deviceNumber);
                return new DeviceInfo(deviceNumber, deviceCaps.ProductName);
            }

            return null;
        }

        private static DeviceInfo LoadVideoDeviceInfo(string configDeviceName)
        {
            if (string.IsNullOrEmpty(configDeviceName))
            {
                return null;
            }

            int deviceNumber = 0;
            FilterInfoCollection availCameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo camera in availCameras)
            {
                if (string.CompareOrdinal(camera.Name, configDeviceName) == 0)
                {
                    return new DeviceInfo(deviceNumber, camera.Name);
                }

                deviceNumber++;
            }

            return null;
        }

        private static DeviceInfo SelectPreferredRecordingDevice(int defaultDeviceNumber)
        {
            if (WaveIn.DeviceCount == 0)
            {
                return null;
            }

            if (WaveIn.DeviceCount == 1)
            {
                var singleDeviceCaps = WaveIn.GetCapabilities(0);
                return new DeviceInfo(0, singleDeviceCaps.ProductName);
            }

            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var availDeviceCaps = WaveIn.GetCapabilities(i);
                if (i == defaultDeviceNumber)
                {
                    return new DeviceInfo(i, availDeviceCaps.ProductName);
                }
            }

            var firstAvailDeviceCaps = WaveIn.GetCapabilities(0);
            return new DeviceInfo(0, firstAvailDeviceCaps.ProductName);
        }

        private static DeviceInfo SelectPreferredPlaybackDevice(int defaultDeviceNumber)
        {
            if (WaveOut.DeviceCount == 0)
            {
                return null;
            }

            if (WaveOut.DeviceCount == 1)
            {
                var singleDeviceCaps = WaveOut.GetCapabilities(0);
                return new DeviceInfo(0, singleDeviceCaps.ProductName);
            }

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var availDeviceCaps = WaveOut.GetCapabilities(i);
                if (i == defaultDeviceNumber)
                {
                    return new DeviceInfo(i, availDeviceCaps.ProductName);
                }
            }

            var firstAvailDeviceCaps = WaveOut.GetCapabilities(0);
            return new DeviceInfo(0, firstAvailDeviceCaps.ProductName);
        }

        private static DeviceInfo SelectPreferredRingingDevice(int defaultDeviceNumber)
        {
            if (WaveOut.DeviceCount == 0)
            {
                return null;
            }

            if (WaveOut.DeviceCount == 1)
            {
                var singleDeviceCaps = WaveIn.GetCapabilities(0);
                return new DeviceInfo(0, singleDeviceCaps.ProductName);
            }

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var availDeviceCaps = WaveOut.GetCapabilities(i);
                if (i == defaultDeviceNumber)
                {
                    return new DeviceInfo(i, availDeviceCaps.ProductName);
                }
            }

            var firstAvailDeviceCaps = WaveOut.GetCapabilities(0);
            return new DeviceInfo(0, firstAvailDeviceCaps.ProductName);
        }

        private static DeviceInfo SelectPreferredVideoDevice()
        {
            FilterInfoCollection availCameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (availCameras.Count == 0)
            {
                return null;
            }

            if (availCameras.Count == 1)
            {
                FilterInfo singleCamera = availCameras[0];
                return new DeviceInfo(0, singleCamera.Name);
            }

            int deviceNumber = 0;
            foreach (FilterInfo camera in availCameras)
            {
                VideoCaptureDevice captureDevice = new VideoCaptureDevice(camera.MonikerString);
                VideoCapabilities[] deviceCaps = captureDevice.VideoCapabilities;

                if (deviceCaps != null && deviceCaps.Length > 0)
                {
                    for (int i = 0; i < deviceCaps.Length; i++)
                    {
                        VideoCapabilities videoCaps = deviceCaps[i];
                        if (videoCaps.FrameSize.Width == VideoDevice.PreferredCameraWidth &&
                            videoCaps.FrameSize.Height == VideoDevice.PreferredCameraHeight)
                        {
                            return new DeviceInfo(deviceNumber, camera.Name);
                        }
                    }
                }

                deviceNumber++;
            }

            FilterInfo firstAvailCamera = availCameras[0];
            return new DeviceInfo(0, firstAvailCamera.Name);
        }

        private static void ActivateInstance(IntPtr hWnd)
        {
            if (NativeMethods.IsIconic(hWnd))
            {
                NativeMethods.ShowWindowAsync(hWnd, NativeMethods.SW_RESTORE);
            }

            NativeMethods.ShowWindowAsync(hWnd, NativeMethods.SW_SHOW);
            NativeMethods.SetForegroundWindow(hWnd);
        }

        #region P/Invoke

        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            internal static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            internal static extern bool IsIconic(IntPtr hWnd);

            internal const int SW_HIDE = 0;
            internal const int SW_SHOWNORMAL = 1;
            internal const int SW_NORMAL = 1;
            internal const int SW_SHOWMINIMIZED = 2;
            internal const int SW_SHOWMAXIMIZED = 3;
            internal const int SW_MAXIMIZE = 3;
            internal const int SW_SHOWNOACTIVATE = 4;
            internal const int SW_SHOW = 5;
            internal const int SW_MINIMIZE = 6;
            internal const int SW_SHOWMINNOACTIVE = 7;
            internal const int SW_SHOWNA = 8;
            internal const int SW_RESTORE = 9;
            internal const int SW_SHOWDEFAULT = 10;
            internal const int SW_MAX = 10;

            [DllImport("tfutils.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern int GetWaveOutId(Int32 role, out Int32 pWaveOutId);

            [DllImport("tfutils.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern int GetWaveInId(Int32 role, out Int32 pWaveInId);

            internal const int ERole_eConsole = 0;
            internal const int ERole_eMultimedia = (ERole_eConsole + 1);
            internal const int ERole_eCommunications = (ERole_eMultimedia + 1);
            internal const int ERole_enum_count = (ERole_eCommunications + 1);
        }

        #endregion
    }

    internal class NAudioLoggerImpl : NAudioLogger
    {
        public override void LogError(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Error, text, fileName, member, line);
        }

        public override void LogWarning(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Warning, text, fileName, member, line);
        }

        public override void LogInfo(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Info, text, fileName, member, line);
        }

        public override void LogVerbose(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Verbose, text, fileName, member, line);
        }
    }

    internal class AForgeLoggerImpl : AForgeLogger
    {
        public override void LogError(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Error, text, fileName, member, line);
        }

        public override void LogWarning(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Warning, text, fileName, member, line);
        }

        public override void LogInfo(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Info, text, fileName, member, line);
        }

        public override void LogVerbose(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Verbose, text, fileName, member, line);
        }
    }

    internal class SharpToxLoggerImpl : SharpToxLogger
    {
        public override void LogError(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Error, text, fileName, member, line);
        }

        public override void LogWarning(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Warning, text, fileName, member, line);
        }

        public override void LogInfo(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Info, text, fileName, member, line);
        }

        public override void LogVerbose(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Verbose, text, fileName, member, line);
        }
    }

    internal class SvgLoggerImpl : SvgLogger
    {
        public override void LogError(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Error, text, fileName, member, line);
        }

        public override void LogWarning(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Warning, text, fileName, member, line);
        }

        public override void LogInfo(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Info, text, fileName, member, line);
        }

        public override void LogVerbose(string text, string fileName, string member, int line)
        {
            Logger.Log(LogLevel.Verbose, text, fileName, member, line);
        }
    }
}
