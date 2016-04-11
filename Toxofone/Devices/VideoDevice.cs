namespace Toxofone.Devices
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Video;
    using AForge.Video.DirectShow;

    public sealed class VideoDevice : IDisposable
    {
        public const int PreferredCameraWidth = 640;
        public const int PreferredCameraHeight = 480;

        private volatile bool enabled;

        private VideoCaptureDevice captureDevice;

        public VideoDevice(bool enabled)
        {
            this.enabled = enabled;

            if (DeviceManager.Instance.VideoDevice != null)
            {
                FilterInfoCollection availCameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo camera in availCameras)
                {
                    if (string.CompareOrdinal(camera.Name, DeviceManager.Instance.VideoDevice.Name) == 0)
                    {
                        this.SetVideoSettings(camera);
                        break;
                    }
                }
            }
        }

        public bool IsEnabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        public bool IsRecording { get; private set; }

        public delegate void VideoDeviceFrameAvailable(Bitmap frame);

        public event VideoDeviceFrameAvailable OnFrameAvailable;

        public void Dispose()
        {
            if (this.captureDevice != null)
            {
                if (this.captureDevice.IsRunning)
                {
                    this.captureDevice.SignalToStop();
                    this.captureDevice.WaitForStop();
                    this.IsRecording = false;
                }

                this.captureDevice.NewFrame -= this.OnCaptureDeviceNewFrame;
                this.captureDevice = null;
            }
        }

        public void SetVideoSettings(FilterInfo device)
        {
            // aforge might throw an exception if it finds an unsupported format
            // why does it not just exclude it from the list and fail silently? joost mag het weten
            try
            {
                this.captureDevice = new VideoCaptureDevice(device.MonikerString);

                VideoCapabilities[] deviceCaps = this.captureDevice.VideoCapabilities;
                if (deviceCaps != null && deviceCaps.Length != 0)
                {
                    // just pick the setting with appropriated resolution

                    VideoCapabilities preferCaps = null;
                    for (int i = 0; i < deviceCaps.Length; i++)
                    {
                        VideoCapabilities videoCaps = deviceCaps[i];
                        if (videoCaps.FrameSize.Width == PreferredCameraWidth &&
                            videoCaps.FrameSize.Height == PreferredCameraHeight)
                        {
                            preferCaps = videoCaps;
                            break;
                        }
                    }
                    if (preferCaps == null)
                    {
                        preferCaps = deviceCaps[0];
                    }

                    this.captureDevice.VideoResolution = preferCaps;
                }

                this.captureDevice.ProvideSnapshots = false;
                this.captureDevice.NewFrame += this.OnCaptureDeviceNewFrame;

                this.IsRecording = false;

                Logger.Log(LogLevel.Info, string.Format("Changed video config to    : camera source: {0}, resolution: {1}x{2}",
                    device.Name, this.captureDevice.VideoResolution.FrameSize.Width, this.captureDevice.VideoResolution.FrameSize.Height));
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.Message);
            }
        }

        public bool DisplayPropertyWindow(IntPtr handle)
        {
            try
            {
                if (this.captureDevice != null)
                {
                    this.captureDevice.DisplayPropertyPage(handle);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.Message);
                return false;
            }
        }

        public void StartRecording()
        {
            if (this.captureDevice != null && !this.captureDevice.IsRunning)
            {
                this.captureDevice.Start();
                this.IsRecording = true;
            }
        }

        public void StopRecording()
        {
            if (this.captureDevice != null && this.captureDevice.IsRunning)
            {
                this.captureDevice.Stop();
                this.IsRecording = false;
            }
        }

        private void OnCaptureDeviceNewFrame(object sender, NewFrameEventArgs e)
        {
            VideoDeviceFrameAvailable handler = this.OnFrameAvailable;
            if (handler != null && this.enabled)
            {
                try
                {
                    using (Bitmap bmp = e.Frame.Clone(new Rectangle(Point.Empty, e.Frame.Size), PixelFormat.Format32bppArgb))
                    {
                        handler(bmp);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex.Message);
                }
            }
        }
    }
}
