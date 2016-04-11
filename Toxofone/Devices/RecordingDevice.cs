namespace Toxofone.Devices
{
    using System;
    using System.Linq;
    using NAudio.Wave;
    using NAudio.Wave.SampleProviders;
    using Toxofone.Utils;

    public sealed class RecordingDevice : IDisposable
    {
        private volatile bool enabled;

        private BufferedWaveProvider audioRecordingProvider;
        private WaveInEvent audioRecorder;
        private MeteringSampleProvider audioRecordingMeter;

        public RecordingDevice(bool enabled)
        {
            this.enabled = enabled;

            if (DeviceManager.Instance.RecordingDevice != null)
            {
                if (DeviceManager.Instance.RecordingDevice.Number < WaveIn.DeviceCount)
                {
                    var deviceCaps = WaveIn.GetCapabilities(DeviceManager.Instance.RecordingDevice.Number);
                    this.SetRecordingSettings(48000, (deviceCaps.Channels > 2) ? 2 : deviceCaps.Channels);
                }
            }
        }

        public bool IsEnabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        public bool IsRecording { get; private set; }

        public WaveFormat RecordingFormat
        {
            get
            {
                if (this.audioRecorder == null)
                {
                    return null;
                }

                return this.audioRecorder.WaveFormat;
            }
        }

        public delegate void AudioEngineRecordingVolumeChanged(float volume);

        public delegate void AudioEngineMicDataAvailable(short[] data, int sampleRate, int channels);

        public event AudioEngineRecordingVolumeChanged OnMicVolumeChanged;

        public event AudioEngineMicDataAvailable OnMicDataAvailable;

        public void Dispose()
        {
            if (this.audioRecorder != null)
            {
                if (this.IsRecording)
                {
                    this.audioRecorder.StopRecording();
                    this.IsRecording = false;
                }

                this.audioRecorder.Dispose();
                this.audioRecorder = null;
            }
        }

        public bool SetRecordingSettings(int sampleRate, int channels)
        {
            if (this.audioRecorder != null)
            {
                if (this.IsRecording)
                {
                    this.audioRecorder.StopRecording();
                    this.IsRecording = false;
                }

                this.audioRecorder.Dispose();
                this.audioRecorder = null;
            }

            if (DeviceManager.Instance.RecordingDevice == null)
            {
                return false;
            }
            if (DeviceManager.Instance.RecordingDevice.Number >= WaveIn.DeviceCount)
            {
                return false;
            }

            var capabilities = WaveIn.GetCapabilities(DeviceManager.Instance.RecordingDevice.Number);
            var waveSourceFormat = new WaveFormat(sampleRate, channels);

            this.audioRecordingProvider = new BufferedWaveProvider(waveSourceFormat);
            this.audioRecordingProvider.DiscardOnBufferOverflow = true;

            this.audioRecorder = new WaveInEvent();
            this.audioRecorder.BufferMilliseconds = 20;
            this.audioRecorder.WaveFormat = waveSourceFormat;
            this.audioRecorder.DeviceNumber = DeviceManager.Instance.RecordingDevice.Number;
            this.audioRecorder.DataAvailable += this.OnAudioRecordingDataAvailable;

            this.audioRecordingMeter = new MeteringSampleProvider(this.audioRecordingProvider.ToSampleProvider());
            this.audioRecordingMeter.StreamVolume += this.OnAudioRecordingMeterStreamVolume;

            this.IsRecording = false;

            Logger.Log(LogLevel.Info, string.Format("Changed recording config to: sampling rate: {0}, channels: {1}", sampleRate, channels));

            return true;
        }

        public void StartRecording()
        {
            if (this.audioRecorder != null)
            {
                this.audioRecorder.StartRecording();
                this.IsRecording = true;
            }
        }

        public void StopRecording()
        {
            if (this.audioRecorder != null)
            {
                this.audioRecorder.StopRecording();
                this.IsRecording = false;
            }
        }

        private void OnAudioRecordingMeterStreamVolume(object sender, StreamVolumeEventArgs e)
        {
            AudioEngineRecordingVolumeChanged handler = this.OnMicVolumeChanged;
            if (handler != null && this.enabled)
            {
                try
                {
                    handler(e.MaxSampleValues.Average());
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex.Message);
                }
            }
        }

        private void OnAudioRecordingDataAvailable(object sender, WaveInEventArgs e)
        {
            short[] shorts = AudioUtils.BytesToShorts(e.Buffer);

            AudioEngineMicDataAvailable handler = this.OnMicDataAvailable;
            if (handler != null && this.enabled)
            {
                try
                {
                    handler(shorts, this.audioRecorder.WaveFormat.SampleRate, this.audioRecorder.WaveFormat.Channels);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex.Message);
                }
            }

            this.audioRecordingProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
            this.audioRecordingMeter.Read(new float[e.BytesRecorded], 0, e.BytesRecorded);
        }
    }
}
