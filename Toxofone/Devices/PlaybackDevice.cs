namespace Toxofone.Devices
{
    using System;
    using System.Collections.Generic;
    using NAudio.Wave;
    using Toxofone.Utils;

    public sealed class PlaybackDevice : IDisposable
    {
        private volatile bool enabled;

        private BufferedWaveProvider audioPlaybackProvider;
        private WaveOutEvent audioPlayer;
        private List<short> audioPlayerBuffer = new List<short>();

        public PlaybackDevice(bool enabled)
        {
            this.enabled = enabled;

            if (DeviceManager.Instance.PlaybackDevice != null)
            {
                if (DeviceManager.Instance.PlaybackDevice.Number < WaveOut.DeviceCount)
                {
                    var deviceCaps = WaveOut.GetCapabilities(DeviceManager.Instance.PlaybackDevice.Number);
                    this.SetPlaybackSettings(48000, (deviceCaps.Channels > 2) ? 2 : deviceCaps.Channels);
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

            if (DeviceManager.Instance.PlaybackDevice == null)
            {
                return false;
            }
            if (DeviceManager.Instance.PlaybackDevice.Number >= WaveOut.DeviceCount)
            {
                return false;
            }

            this.audioPlaybackProvider = new BufferedWaveProvider(waveOutFormat);
            this.audioPlaybackProvider.DiscardOnBufferOverflow = true;

            this.audioPlayer = new WaveOutEvent();
            this.audioPlayer.DeviceNumber = DeviceManager.Instance.PlaybackDevice.Number;
            this.audioPlayer.Init(this.audioPlaybackProvider);

            Logger.Log(LogLevel.Info, string.Format("Changed playback config to : encoding: {0}, sampling rate: {1}, channels: {2}", waveOutFormat.Encoding, waveOutFormat.SampleRate, waveOutFormat.Channels));

            return true;
        }

        public bool SetPlaybackSettings(int sampleRate, int channels)
        {
            if (this.audioPlayer != null)
            {
                this.audioPlayer.Stop();
                this.audioPlayer.Dispose();
                this.audioPlayer = null;
            }

            if (DeviceManager.Instance.PlaybackDevice == null)
            {
                return false;
            }
            if (DeviceManager.Instance.PlaybackDevice.Number >= WaveOut.DeviceCount)
            {
                return false;
            }

            // TODO: what if our friend is sending stereo but our output device only supports mono? write a conversion method for that
            var deviceCaps = WaveOut.GetCapabilities(DeviceManager.Instance.PlaybackDevice.Number);
            var waveOutFormat = new WaveFormat(sampleRate, channels);

            this.audioPlaybackProvider = new BufferedWaveProvider(waveOutFormat);
            this.audioPlaybackProvider.DiscardOnBufferOverflow = true;

            this.audioPlayer = new WaveOutEvent();
            this.audioPlayer.DeviceNumber = DeviceManager.Instance.PlaybackDevice.Number;
            this.audioPlayer.Init(this.audioPlaybackProvider);
            this.audioPlayer.Play();

            Logger.Log(LogLevel.Info, string.Format("Changed playback config to : sampling rate: {0}, channels: {1}", sampleRate, channels));

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

        public bool PlayAudioFrame(short[] audioData, int sampleRate, int channels)
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

            // what is the length of this audio frame?
            int audioLength = ((audioData.Length / channels) * 1000) / sampleRate;

            // what should the length of this frame have been? (we want 20ms to send to the provider)
            int wantedDataLength = ((20 * sampleRate) / 1000) * channels;

            if (wantedDataLength != audioData.Length)
            {
                // if we didn't get the amount of data we wanted, we need to buffer it
                this.audioPlayerBuffer.AddRange(audioData);
                if (this.audioPlayerBuffer.Count == wantedDataLength)
                {
                    short[] shorts = this.audioPlayerBuffer.ToArray();
                    byte[] bytes = AudioUtils.ShortsToBytes(shorts);

                    this.audioPlaybackProvider.AddSamples(bytes, 0, bytes.Length);
                    this.audioPlayerBuffer.Clear();
                }
            }
            else
            {
                byte[] bytes = AudioUtils.ShortsToBytes(audioData);
                this.audioPlaybackProvider.AddSamples(bytes, 0, bytes.Length);
            }

            return true;
        }
    }
}
