namespace Toxofone.Devices
{
    using System;
    using NAudio.Wave;

    public sealed class RingingDevice : IDisposable
    {
        private volatile bool enabled;

        private BufferedWaveProvider audioPlaybackProvider;
        private WaveOutEvent audioPlayer;

        public RingingDevice(bool enabled)
        {
            this.enabled = enabled;

            if (DeviceManager.Instance.RingingDevice != null)
            {
                if (DeviceManager.Instance.RingingDevice.Number < WaveOut.DeviceCount)
                {
                    var capabilities = WaveOut.GetCapabilities(DeviceManager.Instance.RingingDevice.Number);
                    this.SetPlaybackSettings(new WaveFormat());
                }
            }
        }

        public bool IsEnabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        public WaveFormat PlaybackFormat
        {
            get
            {
                if (this.audioPlayer == null)
                {
                    return null;
                }

                return this.audioPlayer.OutputWaveFormat;
            }
        }

        public void Dispose()
        {
            if (this.audioPlayer != null)
            {
                this.audioPlayer.Stop();
                this.audioPlayer.Dispose();
                this.audioPlayer = null;
            }
            if (this.audioPlaybackProvider != null)
            {
                this.audioPlaybackProvider.ClearBuffer();
                this.audioPlaybackProvider = null;
            }
        }

        public bool SetPlaybackSettings(WaveFormat waveOutFormat)
        {
            if (waveOutFormat == null)
            {
                throw new ArgumentNullException("waveOutFormat");
            }

            if (this.audioPlayer != null)
            {
                this.audioPlayer.Stop();
                this.audioPlayer.Dispose();
                this.audioPlayer = null;
            }

            if (DeviceManager.Instance.RingingDevice == null)
            {
                return false;
            }
            if (DeviceManager.Instance.RingingDevice.Number >= WaveOut.DeviceCount)
            {
                return false;
            }

            this.audioPlaybackProvider = new BufferedWaveProvider(waveOutFormat);
            this.audioPlaybackProvider.DiscardOnBufferOverflow = true;

            this.audioPlayer = new WaveOutEvent();
            this.audioPlayer.DeviceNumber = DeviceManager.Instance.RingingDevice.Number;
            this.audioPlayer.Init(this.audioPlaybackProvider);

            Logger.Log(LogLevel.Info, string.Format("Changed ringing config to  : encoding: {0}, sampling rate: {1}, channels: {2}", waveOutFormat.Encoding, waveOutFormat.SampleRate, waveOutFormat.Channels));

            return true;
        }

        public bool PlayAudioFrame(byte[] audioData)
        {
            if (audioData == null)
            {
                return false;
            }

            if (this.audioPlaybackProvider == null)
            {
                return false;
            }
            if (this.audioPlayer == null)
            {
                return false;
            }
            if (!this.enabled)
            {
                return false;
            }

            this.audioPlaybackProvider.ClearBuffer();
            this.audioPlaybackProvider.AddSamples(audioData, 0, audioData.Length);
            this.audioPlayer.Play();

            return true;
        }

        public bool StopAudio()
        {
            if (this.audioPlaybackProvider == null)
            {
                return false;
            }

            this.audioPlayer.Stop();
            this.audioPlaybackProvider.ClearBuffer();

            return true;
        }
    }
}
