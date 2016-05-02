namespace Toxofone.Managers
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using SharpTox.Av;
    using SharpTox.Core;
    using NAudio.Wave;
    using Toxofone.Devices;
    using Toxofone.UI;
    using Toxofone.Utils;

    public enum CallState : int
    {
        None = 1 << 0,
        IncomingCall = 1 << 1,
        OutgoingCall = 1 << 2,
        CallInProgress = 1 << 3,
        IncomingVideo = 1 << 4,
        OutgoingVideo = 1 << 5
    }

    public sealed class CallManager : IToxManager
    {
        public const int DefaultAudioBitrate = 64;  // from qTox
        public const int DefaultVideoBitrate = 6144;  // from qTox

        private const string PhoneRingResName = "Toxofone.Resources.phone_ring.wav";
        private const string BellChordResName = "Toxofone.Resources.bell_chord.wav";

        private const double RingingTimeoutSecs = 45.0;

        private readonly object syncLock = new object();

        private readonly WaveFormat ringWaveFormat;
        private readonly TimeSpan ringTotalTime;
        private readonly byte[] ringAudioFrame;
        private readonly Timer ringRepeatTimer;

        private readonly Stopwatch totalRingingSoundTimer;
        private readonly Stopwatch sendAudioFrameTimer;
        private readonly Stopwatch sendVideoFrameTimer;

        private WaveFormat testPlaybackWaveFormat;
        private TimeSpan testPlaybackTotalTime;
        private byte[] testPlaybackAudioFrame;

        private bool cpuHasSsse3;
        private Tox tox;
        private ToxAv toxAv;
        private CallInfo callInfo;

        public CallManager()
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(PhoneRingResName))
            {
                using (WaveFileReader waveRdr = new WaveFileReader(s))
                {
                    this.ringWaveFormat = waveRdr.WaveFormat;
                    this.ringTotalTime = waveRdr.TotalTime;
                    this.ringAudioFrame = new byte[waveRdr.Length];
                    waveRdr.Read(this.ringAudioFrame, 0, this.ringAudioFrame.Length);
                }
            }
            this.ringRepeatTimer = new Timer(this.RingRepeatTimerCallback, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1L));
            this.RingingDevice = new RingingDevice(true);
            this.totalRingingSoundTimer = new Stopwatch();
            this.sendAudioFrameTimer = new Stopwatch();
            this.sendVideoFrameTimer = new Stopwatch();
        }

        public RingingDevice RingingDevice { get; private set; }

        public RecordingDevice RecordingMonitor { get; private set; }
        public PlaybackDevice PlaybackMonitor { get; private set; }
        public VideoDevice VideoMonitor { get; private set; }

        public void InitManager(Tox tox, ToxAv toxAv)
        {
            if (tox == null)
            {
                throw new ArgumentNullException("tox");
            }

            if (toxAv == null)
            {
                throw new ArgumentNullException("toxAv");
            }

            this.cpuHasSsse3 = VideoUtils.CpuHasSsse3;

            this.tox = tox;
            this.tox.OnFriendConnectionStatusChanged += this.OnToxFriendConnectionStatusChanged;

            this.toxAv = toxAv;
            this.toxAv.OnBitrateSuggestion += this.OnToxAvBitrateSuggestion;
            this.toxAv.OnCallRequestReceived += this.OnToxAvCallRequestReceived;
            this.toxAv.OnCallStateChanged += this.OnToxAvCallStateChanged;
            this.toxAv.OnAudioFrameReceived += this.OnToxAvAudioFrameReceived;
            this.toxAv.OnVideoFrameReceived += this.OnToxAvVideoFrameReceived;
            this.toxAv.OnReceivedGroupAudio += this.OnToxAvReceivedGroupAudio;
        }

        public bool RestartRecording(bool enableRecording = true)
        {
            if (this.callInfo != null)
            {
                if (this.callInfo.RestartRecording(enableRecording))
                {
                    this.callInfo.RecordingDevice.OnMicVolumeChanged += this.OnAudioDeviceMicVolumeChanged;
                    this.callInfo.RecordingDevice.OnMicDataAvailable += this.OnAudioDeviceMicDataAvailable;

                    if (this.callInfo.CanSendAudio)
                    {
                        this.callInfo.RecordingDevice.StartRecording();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return this.RestartRecordingMonitor(enableRecording);
            }
        }

        public bool EnableRecording(bool enableRecording)
        {
            if (this.callInfo != null)
            {
                if (!enableRecording)
                {
                    callInfo.EnableRecording(false);

                    if (this.callInfo.RecordingDevice.IsRecording)
                    {
                        this.callInfo.RecordingDevice.StopRecording();
                    }
                }

                ToxAvFriendCallState friendCallState = this.callInfo.FriendCallState;
                if (enableRecording)
                {
                    friendCallState |= ToxAvFriendCallState.ReceivingAudio;
                }
                else
                {
                    friendCallState &= ~ToxAvFriendCallState.ReceivingAudio;
                }
                this.callInfo.FriendCallState = friendCallState;

                if (enableRecording)
                {
                    callInfo.EnableRecording(true);

                    if (!this.callInfo.RecordingDevice.IsRecording)
                    {
                        this.callInfo.RecordingDevice.StartRecording();
                    }
                }

                return true;
            }
            else
            {
                return this.RestartRecordingMonitor(enableRecording);
            }
        }

        public bool RestartPlayback(bool enablePlayback = true)
        {
            if (this.callInfo != null)
            {
                return this.callInfo.RestartPlayback(enablePlayback);
            }
            else
            {
                return this.RestartPlaybackMonitor(enablePlayback);
            }
        }

        public bool EnablePlayback(bool enablePlayback)
        {
            if (this.callInfo != null)
            {
                if (!enablePlayback)
                {
                    this.callInfo.EnablePlayback(false);
                }

                ToxAvCallControl audioControl = enablePlayback ? ToxAvCallControl.UnmuteAudio : ToxAvCallControl.MuteAudio;
                ToxAvErrorCallControl audioError = ToxAvErrorCallControl.Ok;
                if (!this.toxAv.SendControl(this.callInfo.FriendNumber, audioControl, out audioError))
                {
                    Logger.Log(LogLevel.Error, "Could not change audio receiving: " + audioError);
                }

                ToxAvFriendCallState friendCallState = this.callInfo.FriendCallState;
                if (enablePlayback)
                {
                    friendCallState |= ToxAvFriendCallState.SendingAudio;
                }
                else
                {
                    friendCallState &= ~ToxAvFriendCallState.SendingAudio;
                }
                this.callInfo.FriendCallState = friendCallState;

                if (enablePlayback)
                {
                    this.callInfo.EnablePlayback(true);
                }

                return true;
            }
            else
            {
                return this.RestartPlaybackMonitor(enablePlayback);
            }
        }

        public bool RestartRinging(bool enableRinging = true)
        {
            return this.RestartRingingDevice(enableRinging);
        }

        public bool EnableRinging(bool enableRinging)
        {
            return this.RestartRingingDevice(enableRinging);
        }

        public bool RestartLocalVideo(bool enableLocalVideo = true)
        {
            if (this.callInfo != null)
            {
                if (this.callInfo.RestartVideo(enableLocalVideo))
                {
                    this.callInfo.VideoDevice.OnFrameAvailable += this.OnVideoDeviceFrameAvailable;

                    if (this.callInfo.CanSendVideo)
                    {
                        this.callInfo.VideoDevice.StartRecording();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return this.RestartVideoMonitor(enableLocalVideo);
            }
        }

        public bool EnableLocalVideo(bool enableLocalVideo)
        {
            if (this.callInfo != null)
            {
                if (!enableLocalVideo)
                {
                    callInfo.EnableVideo(false);

                    if (this.callInfo.VideoDevice.IsRecording)
                    {
                        this.callInfo.VideoDevice.StopRecording();
                    }
                }

                ToxAvFriendCallState friendCallState = this.callInfo.FriendCallState;
                if (enableLocalVideo)
                {
                    friendCallState |= ToxAvFriendCallState.ReceivingVideo;
                }
                else
                {
                    friendCallState &= ~ToxAvFriendCallState.ReceivingVideo;
                }
                this.callInfo.FriendCallState = friendCallState;

                if (enableLocalVideo)
                {
                    callInfo.EnableVideo(true);

                    if (!this.callInfo.VideoDevice.IsRecording)
                    {
                        this.callInfo.VideoDevice.StartRecording();
                    }
                }

                return true;
            }
            else
            {
                return this.RestartVideoMonitor(enableLocalVideo);
            }
        }

        public bool EnableFriendVideo(bool enableFriendVideo)
        {
            if (this.callInfo != null)
            {
                ToxAvCallControl videoControl = enableFriendVideo ? ToxAvCallControl.ShowVideo : ToxAvCallControl.HideVideo;
                ToxAvErrorCallControl videoError = ToxAvErrorCallControl.Ok;
                if (!this.toxAv.SendControl(this.callInfo.FriendNumber, videoControl, out videoError))
                {
                    Logger.Log(LogLevel.Error, "Could not change video receiving: " + videoError);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public TimeSpan PlayTestPlaybackSoundOnce()
        {
            if (this.PlaybackMonitor == null)
            {
                return TimeSpan.FromTicks(0L);
            }

            if (this.testPlaybackAudioFrame == null)
            {
                using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(BellChordResName))
                {
                    using (WaveFileReader waveRdr = new WaveFileReader(s))
                    {
                        this.testPlaybackWaveFormat = waveRdr.WaveFormat;
                        this.testPlaybackTotalTime = waveRdr.TotalTime;
                        this.testPlaybackAudioFrame = new byte[waveRdr.Length];
                        waveRdr.Read(this.testPlaybackAudioFrame, 0, this.testPlaybackAudioFrame.Length);
                    }
                }
            }

            if (this.testPlaybackWaveFormat != this.PlaybackMonitor.PlaybackFormat)
            {
                this.PlaybackMonitor.SetPlaybackSettings(this.testPlaybackWaveFormat);
            }
            this.PlaybackMonitor.PlayAudioFrame(this.testPlaybackAudioFrame);

            return this.testPlaybackTotalTime;
        }

        public TimeSpan PlayRingingSoundOnce()
        {
            if (this.RingingDevice == null)
            {
                return TimeSpan.FromTicks(0L);
            }

            if (this.ringWaveFormat != this.RingingDevice.PlaybackFormat)
            {
                this.RingingDevice.SetPlaybackSettings(this.ringWaveFormat);
            }
            this.RingingDevice.PlayAudioFrame(this.ringAudioFrame);

            return this.ringTotalTime;
        }

        public void StartRingingSound()
        {
            if (this.RingingDevice != null)
            {
                TimeSpan ringTotalTime = this.PlayRingingSoundOnce();
                if (ringTotalTime.Ticks > 0)
                {
                    // start ringing
                    this.ringRepeatTimer.Change(TimeSpan.FromMilliseconds(ringTotalTime.TotalMilliseconds * 1.75), TimeSpan.FromMilliseconds(-1L));
                    this.totalRingingSoundTimer.Reset();
                    this.totalRingingSoundTimer.Start();
                }
            }
        }

        public void StopRingingSound()
        {
            if (this.RingingDevice != null)
            {
                this.RingingDevice.StopAudio();
            }

            this.ringRepeatTimer.Change(TimeSpan.FromMilliseconds(-1L), TimeSpan.FromMilliseconds(-1L));
            this.totalRingingSoundTimer.Stop();
        }

        public bool Answer(int friendNumber, bool enableRecording, bool enablePlayback, bool enableLocalVideo, bool enableFriendVideo)
        {
            // stop ringing
            this.StopRingingSound();

            if (this.callInfo != null)
            {
                Logger.Log(LogLevel.Warning, "Tried to answer a call but there is already one in progress");
                return false;
            }

            ToxAvErrorAnswer error = ToxAvErrorAnswer.Ok;
            if (!this.toxAv.Answer(friendNumber, DefaultAudioBitrate, DefaultVideoBitrate, out error))
            {
                Logger.Log(LogLevel.Error, "Could not answer call for friend: " + error);
                return false;
            }

            ToxAvCallControl audioControl = enablePlayback ? ToxAvCallControl.UnmuteAudio : ToxAvCallControl.MuteAudio;
            ToxAvErrorCallControl audioError = ToxAvErrorCallControl.Ok;
            if (!this.toxAv.SendControl(friendNumber, audioControl, out audioError))
            {
                Logger.Log(LogLevel.Error, "Could not change audio receiving: " + audioError);
            }

            ToxAvCallControl videoControl = enableFriendVideo ? ToxAvCallControl.ShowVideo : ToxAvCallControl.HideVideo;
            ToxAvErrorCallControl videoError = ToxAvErrorCallControl.Ok;
            if (!this.toxAv.SendControl(friendNumber, videoControl, out videoError))
            {
                Logger.Log(LogLevel.Error, "Could not change video receiving: " + videoError);
            }

            // disable local audio/video monitors
            this.RestartRecordingMonitor(false);
            this.RestartPlaybackMonitor(false);
            this.RestartVideoMonitor(false);

            ToxAvFriendCallState friendCallState = ToxAvFriendCallState.Paused;
            if (enableRecording)
            {
                friendCallState |= ToxAvFriendCallState.ReceivingAudio;
            }
            if (enablePlayback)
            {
                friendCallState |= ToxAvFriendCallState.SendingAudio;
            }
            if (enableLocalVideo)
            {
                friendCallState |= ToxAvFriendCallState.ReceivingVideo;
            }
            if (enableFriendVideo)
            {
                friendCallState |= ToxAvFriendCallState.SendingVideo;
            }

            this.callInfo = new CallInfo(friendNumber, enableRecording, enablePlayback, enableLocalVideo, enableFriendVideo);
            this.callInfo.FriendCallState = friendCallState;
            this.callInfo.RecordingDevice.OnMicVolumeChanged += this.OnAudioDeviceMicVolumeChanged;
            this.callInfo.RecordingDevice.OnMicDataAvailable += this.OnAudioDeviceMicDataAvailable;
            this.callInfo.VideoDevice.OnFrameAvailable += this.OnVideoDeviceFrameAvailable;

            var audioBitrateError = ToxAvErrorSetBitrate.Ok;
            bool audioBitrateSet = this.toxAv.SetAudioBitrate(friendNumber, DefaultAudioBitrate, out audioBitrateError);
            if (!audioBitrateSet)
            {
                Logger.Log(LogLevel.Error, string.Format("Could not set audio bitrate to {0}, error: {1}", DefaultAudioBitrate, audioBitrateError));
            }

            var videoBitrateError = ToxAvErrorSetBitrate.Ok;
            bool videoBitrateSet = this.toxAv.SetVideoBitrate(friendNumber, DefaultVideoBitrate, out videoBitrateError);
            if (!videoBitrateSet)
            {
                Logger.Log(LogLevel.Error, string.Format("Could not set video bitrate to {0}, error: {1}", DefaultVideoBitrate, videoBitrateError));
            }

            if (audioBitrateSet)
            {
                this.callInfo.AudioBitrate = DefaultAudioBitrate;
            }

            if (videoBitrateSet)
            {
                this.callInfo.VideoBitrate = DefaultVideoBitrate;
            }

            if (enableRecording)
            {
                this.callInfo.RecordingDevice.StartRecording();
            }
            if (enableLocalVideo)
            {
                this.callInfo.VideoDevice.StartRecording();
            }

            return true;
        }

        public bool Hangup(int friendNumber)
        {
            // stop ringing
            this.StopRingingSound();

            ToxAvErrorCallControl error = ToxAvErrorCallControl.Ok;
            if (!this.toxAv.SendControl(friendNumber, ToxAvCallControl.Cancel, out error))
            {
                Logger.Log(LogLevel.Error, "Could not cancel a call for friend: " + error);
                return false;
            }

            bool enableRecording = true;
            bool enablePlayback = true;
            bool enableVideo = true;

            if (this.callInfo != null)
            {
                enableRecording = this.callInfo.RecordingDevice.IsEnabled;
                enablePlayback = this.callInfo.PlaybackDevice.IsEnabled;
                enableVideo = this.callInfo.VideoDevice.IsEnabled;

                this.callInfo.FriendCallState = ToxAvFriendCallState.Finished;
                this.callInfo.Dispose();
                this.callInfo = null;
            }

            // enable local audio/video monitors
            this.RestartRecordingMonitor(enableRecording);
            this.RestartPlaybackMonitor(enablePlayback);
            this.RestartVideoMonitor(enableVideo);

            return true;
        }

        public bool StartCall(int friendNumber, bool enableAudioRecording, bool enableAudioPlayback, bool enableLocalVideo, bool enableFriendVideo)
        {
            if (this.callInfo != null)
            {
                Logger.Log(LogLevel.Warning, "Tried to send a call request but there is already one in progress");
                return false;
            }

            var error = ToxAvErrorCall.Ok;
            if (!this.toxAv.Call(friendNumber, DefaultAudioBitrate, DefaultVideoBitrate, out error))
            {
                Logger.Log(LogLevel.Error, "Could not send call request to friend: " + error);
                return false;
            }

            this.callInfo = new CallInfo(friendNumber, enableAudioRecording, enableAudioPlayback, enableLocalVideo, enableFriendVideo);
            this.callInfo.RecordingDevice.OnMicVolumeChanged += this.OnAudioDeviceMicVolumeChanged;
            this.callInfo.RecordingDevice.OnMicDataAvailable += this.OnAudioDeviceMicDataAvailable;
            this.callInfo.VideoDevice.OnFrameAvailable += this.OnVideoDeviceFrameAvailable;

            return true;
        }

        public void Kill()
        {
            // stop ringing
            this.StopRingingSound();

            if (this.callInfo != null)
            {
                ToxAvErrorCallControl error = ToxAvErrorCallControl.Ok;
                if (!this.toxAv.SendControl(this.callInfo.FriendNumber, ToxAvCallControl.Cancel, out error))
                {
                    Logger.Log(LogLevel.Error, "Could not cancel a call for friend: " + error);
                }

                this.callInfo.FriendCallState = ToxAvFriendCallState.Finished;
                this.callInfo.Dispose();
                this.callInfo = null;
            }

            this.RestartRecordingMonitor(false);
            this.RestartPlaybackMonitor(false);
            this.RestartRingingDevice(false);
            this.RestartVideoMonitor(false);
        }

        public void DisplayPropertyWindow(IntPtr handle)
        {
            if (this.callInfo != null && this.callInfo.VideoDevice != null)
            {
                this.callInfo.VideoDevice.DisplayPropertyWindow(handle);
            }
            else if (this.VideoMonitor != null)
            {
                this.VideoMonitor.DisplayPropertyWindow(handle);
            }
        }

        private bool RestartRecordingMonitor(bool enableRecording)
        {
            lock (this.syncLock)
            {
                if (this.RecordingMonitor != null)
                {
                    this.RecordingMonitor.Dispose();
                    this.RecordingMonitor = null;
                }

                if (!enableRecording)
                {
                    return false;
                }

                this.RecordingMonitor = new RecordingDevice(true);
                this.RecordingMonitor.OnMicVolumeChanged += this.OnAudioDeviceMicVolumeChanged;
                this.RecordingMonitor.OnMicDataAvailable += this.OnAudioDeviceMicDataAvailable;
                this.RecordingMonitor.StartRecording();

                return true;
            }
        }

        private bool RestartPlaybackMonitor(bool enablePlayback)
        {
            lock (this.syncLock)
            {
                if (this.PlaybackMonitor != null)
                {
                    this.PlaybackMonitor.Dispose();
                    this.PlaybackMonitor = null;
                }

                if (!enablePlayback)
                {
                    return false;
                }

                this.PlaybackMonitor = new PlaybackDevice(true);

                return true;
            }
        }

        private bool RestartRingingDevice(bool enableRinging)
        {
            lock (this.syncLock)
            {
                if (this.RingingDevice != null)
                {
                    this.RingingDevice.Dispose();
                    this.RingingDevice = null;
                }

                if (!enableRinging)
                {
                    return false;
                }

                this.RingingDevice = new RingingDevice(true);

                return true;
            }
        }

        private bool RestartVideoMonitor(bool enableVideo)
        {
            lock (this.syncLock)
            {
                if (this.VideoMonitor != null)
                {
                    this.VideoMonitor.Dispose();
                    this.VideoMonitor = null;
                }

                if (!enableVideo)
                {
                    return false;
                }

                this.VideoMonitor = new VideoDevice(true);
                this.VideoMonitor.OnFrameAvailable += this.OnVideoDeviceFrameAvailable;
                this.VideoMonitor.StartRecording();

                return true;
            }
        }

        private void RingRepeatTimerCallback(object state)
        {
            this.totalRingingSoundTimer.Stop();

            if (totalRingingSoundTimer.Elapsed.TotalSeconds >= RingingTimeoutSecs)
            {
                this.StopRingingSound();
                MainForm.Instance.NotifyRingingTimeoutElapsed();
                return;
            }

            TimeSpan ringTotalTime = this.PlayRingingSoundOnce();
            if (ringTotalTime.Ticks > 0)
            {
                // start another ringing cycle
                this.ringRepeatTimer.Change(TimeSpan.FromMilliseconds(ringTotalTime.TotalMilliseconds * 1.75), TimeSpan.FromMilliseconds(-1L));
                this.totalRingingSoundTimer.Start();
            }
        }

        #region Tox audio/video event handlers

        private void OnToxFriendConnectionStatusChanged(object sender, ToxEventArgs.FriendConnectionStatusEventArgs e)
        {
            Logger.Log(LogLevel.Verbose, "status: " + e.Status);

            if (e.Status != ToxConnectionStatus.None)
            {
                return;
            }

            if (this.callInfo != null && this.callInfo.FriendNumber == e.FriendNumber)
            {
                this.OnToxAvCallStateChanged(null, new ToxAvEventArgs.CallStateEventArgs(e.FriendNumber, ToxAvFriendCallState.Finished));
            }
        }

        private void OnToxAvBitrateSuggestion(object sender, ToxAvEventArgs.BitrateStatusEventArgs e)
        {
            Logger.Log(LogLevel.Verbose, string.Format("Applying ToxAV suggestion: {0} for audio, {1} for video", e.AudioBitrate, e.VideoBitrate));

            var audioBitrateError = ToxAvErrorSetBitrate.Ok;
            bool audioBitrateSet = this.toxAv.SetAudioBitrate(e.FriendNumber, e.AudioBitrate, out audioBitrateError);
            if (!audioBitrateSet)
            {
                Logger.Log(LogLevel.Error, string.Format("Could not set audio bitrate to {0}, error: {1}", e.AudioBitrate, audioBitrateError));
            }

            var videoBitrateError = ToxAvErrorSetBitrate.Ok;
            bool videoBitrateSet = this.toxAv.SetVideoBitrate(e.FriendNumber, e.VideoBitrate, out videoBitrateError);
            if (!videoBitrateSet)
            {
                Logger.Log(LogLevel.Error, string.Format("Could not set video bitrate to {0}, error: {1}", e.VideoBitrate, videoBitrateError));
            }

            if (this.callInfo != null)
            {
                if (audioBitrateSet)
                {
                    this.callInfo.AudioBitrate = e.AudioBitrate;
                }

                if (videoBitrateSet)
                {
                    this.callInfo.VideoBitrate = e.VideoBitrate;
                }
            }
        }

        private void OnToxAvCallRequestReceived(object sender, ToxAvEventArgs.CallRequestEventArgs e)
        {
            Logger.Log(LogLevel.Verbose, "state: " + e.State);

            if (this.callInfo != null)
            {
                // TODO: notify the user there's yet another call incoming
                this.toxAv.SendControl(e.FriendNumber, ToxAvCallControl.Cancel);
                return;
            }

            try
            {
                MainForm.Instance.NotifyToxAvCallRequestReceived(e);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);
            }
        }

        private void OnToxAvCallStateChanged(object sender, ToxAvEventArgs.CallStateEventArgs e)
        {
            Logger.Log(LogLevel.Verbose, "state: " + e.State);

            CallState friendCallState = CallState.CallInProgress;

            if ((e.State & ToxAvFriendCallState.Finished) != 0 || 
                (e.State & ToxAvFriendCallState.Error) != 0)
            {
                bool enableRecording = true;
                bool enablePlayback = true;
                bool enableVideo = true;

                if (this.callInfo != null)
                {
                    enableRecording = this.callInfo.RecordingDevice.IsEnabled;
                    enablePlayback = this.callInfo.PlaybackDevice.IsEnabled;
                    enableVideo = this.callInfo.VideoDevice.IsEnabled;

                    this.callInfo.FriendCallState = e.State;
                    this.callInfo.Dispose();
                    this.callInfo = null;
                }

                friendCallState = CallState.None;

                // enable local audio/video monitors
                this.RestartRecordingMonitor(enableRecording);
                this.RestartPlaybackMonitor(enablePlayback);
                this.RestartVideoMonitor(enableVideo);

                // stop ringing
                this.StopRingingSound();
            }
            else if ((e.State & ToxAvFriendCallState.ReceivingAudio) != 0 ||
                (e.State & ToxAvFriendCallState.ReceivingVideo) != 0 ||
                (e.State & ToxAvFriendCallState.SendingAudio) != 0 ||
                (e.State & ToxAvFriendCallState.SendingVideo) != 0)
            {
                // disable local audio/video monitors
                this.RestartRecordingMonitor(false);
                this.RestartPlaybackMonitor(false);
                this.RestartVideoMonitor(false);

                this.callInfo.FriendCallState = e.State;

                if (this.callInfo.AudioBitrate <= 0)
                {
                    var audioBitrateError = ToxAvErrorSetBitrate.Ok;
                    bool audioBitrateSet = this.toxAv.SetAudioBitrate(e.FriendNumber, DefaultAudioBitrate, out audioBitrateError);
                    if (!audioBitrateSet)
                    {
                        Logger.Log(LogLevel.Error, string.Format("Could not set audio bitrate to {0}, error: {1}", DefaultAudioBitrate, audioBitrateError));
                    }
                    else
                    {
                        this.callInfo.AudioBitrate = DefaultAudioBitrate;
                    }
                }

                if (this.callInfo.VideoBitrate <= 0)
                {
                    var videoBitrateError = ToxAvErrorSetBitrate.Ok;
                    bool videoBitrateSet = this.toxAv.SetVideoBitrate(e.FriendNumber, DefaultVideoBitrate, out videoBitrateError);
                    if (!videoBitrateSet)
                    {
                        Logger.Log(LogLevel.Error, string.Format("Could not set video bitrate to {0}, error: {1}", DefaultVideoBitrate, videoBitrateError));
                    }
                    else
                    {
                        this.callInfo.VideoBitrate = DefaultVideoBitrate;
                    }
                }

                // start sending whatever from here
                if (!this.callInfo.RecordingDevice.IsRecording)
                {
                    this.callInfo.RecordingDevice.StartRecording();
                }

                if (!this.callInfo.VideoDevice.IsRecording)
                {
                    this.callInfo.VideoDevice.StartRecording();
                }

                if (e.State.HasFlag(ToxAvFriendCallState.SendingVideo))
                {
                    friendCallState |= CallState.IncomingVideo;
                }

                friendCallState |= CallState.OutgoingVideo;

                // stop ringing
                this.StopRingingSound();
            }

            try
            {
                MainForm.Instance.NotifyToxAvCallStateChanged(new CallStateInfo(e.FriendNumber, friendCallState));
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);
            }
        }

        private void OnToxAvAudioFrameReceived(object sender, ToxAvEventArgs.AudioFrameEventArgs e)
        {
            short[] audioData = e.Frame.Data;
            int sampleRate = e.Frame.SamplingRate;
            int channels = e.Frame.Channels;

            if (audioData == null)
            {
                return;
            }

            if (this.callInfo == null || !this.callInfo.CanReceiveAudio)
            {
                return;
            }

            // in case the friend suddenly changed audio config, account for it here
            if (sampleRate != this.callInfo.PlaybackDevice.PlaybackFormat.SampleRate ||
                channels != this.callInfo.PlaybackDevice.PlaybackFormat.Channels)
            {
                this.callInfo.PlaybackDevice.SetPlaybackSettings(sampleRate, channels);
            }

            // send the frame to the audio engine
            this.callInfo.PlaybackDevice.PlayAudioFrame(audioData, sampleRate, channels);
        }

        private void OnToxAvVideoFrameReceived(object sender, ToxAvEventArgs.VideoFrameEventArgs e)
        {
            Bitmap videoFrame = VideoUtils.ToxAvFrameToBitmap(e.Frame, this.cpuHasSsse3);
            if (videoFrame == null)
            {
                return;
            }

            if (this.callInfo == null || !this.callInfo.CanReceiveVideo)
            {
                videoFrame.Dispose();
                return;
            }

            try
            {
                MainForm.Instance.NotifyToxAvVideoFrameReceived(new VideoFrameInfo(e.FriendNumber, videoFrame));
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);
            }
        }

        private void OnToxAvReceivedGroupAudio(object sender, ToxAvEventArgs.GroupAudioDataEventArgs e)
        {
            Logger.Log(LogLevel.Warning, "ToxAvReceivedGroupAudio not supported");
        }

        #endregion

        #region Audio/Video device event handlers

        private void OnAudioDeviceMicDataAvailable(short[] audioData, int sampleRate, int channels)
        {
            if (audioData == null || audioData.Length == 0)
            {
                return;
            }

            if (this.callInfo == null || !this.callInfo.CanSendAudio)
            {
                return;
            }

            var toxAudioFrame = new ToxAvAudioFrame(audioData, sampleRate, channels);
            if (toxAudioFrame == null)
            {
                Logger.Log(LogLevel.Error, "Failed to convert audio data to audio frame");
                return;
            }

            // yes, check again
            if (this.callInfo == null || !this.callInfo.CanSendAudio)
            {
                return;
            }

            if (this.callInfo.AudioBitrate <= 0)
            {
                if (this.toxAv.SetAudioBitrate(this.callInfo.FriendNumber, DefaultAudioBitrate))
                {
                    this.callInfo.AudioBitrate = DefaultAudioBitrate;
                }
            }

            this.sendAudioFrameTimer.Reset();
            this.sendAudioFrameTimer.Start();
            try
            {
                var error = ToxAvErrorSendFrame.Ok;
                do
                {
                    if (!this.toxAv.SendAudioFrame(this.callInfo.FriendNumber, toxAudioFrame, out error))
                    {
                        if (error == ToxAvErrorSendFrame.Sync)
                        {
                            Thread.Sleep(0);
                        }
                        else
                        {
                            Logger.Log(LogLevel.Error, string.Format("Failed to send audio frame {0}/{1}/{2} : {3}", toxAudioFrame.Data.Length, toxAudioFrame.SamplingRate, toxAudioFrame.Channels, error));
                        }
                    }
                }
                while (error == ToxAvErrorSendFrame.Sync && this.sendAudioFrameTimer.ElapsedMilliseconds < 5);

                if (error == ToxAvErrorSendFrame.Sync)
                {
                    Logger.Log(LogLevel.Error, string.Format("Failed to send audio frame {0}/{1}/{2} : Lock busy, dropping frame", toxAudioFrame.Data.Length, toxAudioFrame.SamplingRate, toxAudioFrame.Channels));
                }
            }
            finally
            {
                this.sendAudioFrameTimer.Stop();
            }
        }

        private void OnAudioDeviceMicVolumeChanged(float volume)
        {
            try
            {
                MainForm.Instance.NotifyMicVolumeChanged(volume * 100);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);
            }
        }

        private void OnVideoDeviceFrameAvailable(Bitmap videoFrame)
        {
            if (videoFrame == null || videoFrame.PixelFormat != PixelFormat.Format32bppArgb)
            {
                return;
            }

            try
            {
                Bitmap localVideoFrame = (Bitmap)videoFrame.Clone();
                MainForm.Instance.NotifyNewLocalVideoFrameAvailable(localVideoFrame);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);
            }

            if (this.callInfo == null || !this.callInfo.CanSendVideo)
            {
                return;
            }

            var toxVideoFrame = VideoUtils.BitmapToToxAvFrame(videoFrame, this.cpuHasSsse3);
            if (toxVideoFrame == null)
            {
                Logger.Log(LogLevel.Error, "Failed to convert video data to video frame");
                return;
            }

            // yes, check again
            if (this.callInfo == null || !this.callInfo.CanSendVideo)
            {
                return;
            }

            if (this.callInfo.VideoBitrate <= 0)
            {
                if (this.toxAv.SetVideoBitrate(this.callInfo.FriendNumber, DefaultVideoBitrate))
                {
                    this.callInfo.VideoBitrate = DefaultVideoBitrate;
                }
            }

            this.sendVideoFrameTimer.Reset();
            this.sendVideoFrameTimer.Start();
            try
            {
                var error = ToxAvErrorSendFrame.Ok;
                do
                {
                    if (!this.toxAv.SendVideoFrame(this.callInfo.FriendNumber, toxVideoFrame, out error))
                    {
                        if (error == ToxAvErrorSendFrame.Sync)
                        {
                            Thread.Sleep(0);
                        }
                        else
                        {
                            Logger.Log(LogLevel.Error, string.Format("Failed to send video frame {0}x{1} : {2}", toxVideoFrame.Width, toxVideoFrame.Height, error));
                        }
                    }
                }
                while (error == ToxAvErrorSendFrame.Sync && this.sendVideoFrameTimer.ElapsedMilliseconds < 5);

                if (error == ToxAvErrorSendFrame.Sync)
                {
                    Logger.Log(LogLevel.Error, string.Format("Failed to send video frame {0}x{1} : Lock busy, dropping frame", toxVideoFrame.Width, toxVideoFrame.Height));
                }
            }
            finally
            {
                this.sendVideoFrameTimer.Stop();
            }
        }

        #endregion

        private class CallInfo : IDisposable
        {
            private readonly object syncLock = new object();

            private ToxAvFriendCallState friendCallState;

            public CallInfo(int friendNumber, bool enableRecording, bool enablePlayback, bool enableLocalVideo, bool enableFriendVideo)
            {
                this.FriendNumber = friendNumber;
                this.RecordingDevice = new RecordingDevice(enableRecording);
                this.PlaybackDevice = new PlaybackDevice(enablePlayback);
                this.VideoDevice = new VideoDevice(enableLocalVideo);
                this.friendCallState = ToxAvFriendCallState.Finished;
            }

            public int FriendNumber { get; private set; }
            public RecordingDevice RecordingDevice { get; private set; }
            public PlaybackDevice PlaybackDevice { get; private set; }
            public VideoDevice VideoDevice { get; private set; }

            public int AudioBitrate { get; set; } = -1;
            public int VideoBitrate { get; set; } = -1;

            public ToxAvFriendCallState FriendCallState
            {
                get
                {
                    lock (this.syncLock)
                    {
                        return this.friendCallState;
                    }
                }
                set
                {
                    lock (this.syncLock)
                    {
                        this.friendCallState = value;
                    }
                }
            }

            public bool CanSendAudio
            {
                get
                {
                    lock (this.syncLock)
                    {
                        return ((this.friendCallState & ToxAvFriendCallState.ReceivingAudio) != 0);
                    }
                }
            }

            public bool CanReceiveAudio
            {
                get
                {
                    lock (this.syncLock)
                    {
                        return ((this.friendCallState & ToxAvFriendCallState.SendingAudio) != 0);
                    }
                }
            }

            public bool CanSendVideo
            {
                get
                {
                    lock (this.syncLock)
                    {
                        return ((this.friendCallState & ToxAvFriendCallState.ReceivingVideo) != 0);
                    }
                }
            }

            public bool CanReceiveVideo
            {
                get
                {
                    lock (this.syncLock)
                    {
                        return ((this.friendCallState & ToxAvFriendCallState.SendingVideo) != 0);
                    }
                }
            }

            public bool RestartRecording(bool enableRecording)
            {
                if (this.RecordingDevice != null)
                {
                    this.RecordingDevice.Dispose();
                    this.RecordingDevice = null;
                }

                this.RecordingDevice = new RecordingDevice(enableRecording);

                return true;
            }

            public bool EnableRecording(bool enableRecording)
            {
                if (this.RecordingDevice == null)
                {
                    return false;
                }

                this.RecordingDevice.IsEnabled = enableRecording;

                return this.RecordingDevice.IsEnabled;
            }

            public bool RestartPlayback(bool enablePlayback)
            {
                if (this.PlaybackDevice != null)
                {
                    this.PlaybackDevice.Dispose();
                    this.PlaybackDevice = null;
                }

                this.PlaybackDevice = new PlaybackDevice(enablePlayback);

                return true;
            }

            public bool EnablePlayback(bool enablePlayback)
            {
                if (this.PlaybackDevice == null)
                {
                    return false;
                }

                this.PlaybackDevice.IsEnabled = enablePlayback;

                return this.PlaybackDevice.IsEnabled;
            }

            public bool RestartVideo(bool enableVideo)
            {
                if (this.VideoDevice != null)
                {
                    this.VideoDevice.Dispose();
                    this.VideoDevice = null;
                }

                this.VideoDevice = new VideoDevice(enableVideo);

                return true;
            }

            public bool EnableVideo(bool enableVideo)
            {
                if (this.VideoDevice == null)
                {
                    return false;
                }

                this.VideoDevice.IsEnabled = enableVideo;

                return this.VideoDevice.IsEnabled;
            }

            public void Dispose()
            {
                if (this.RecordingDevice != null)
                {
                    this.RecordingDevice.Dispose();
                    this.RecordingDevice = null;
                }

                if (this.PlaybackDevice != null)
                {
                    this.PlaybackDevice.Dispose();
                    this.PlaybackDevice = null;
                }

                if (this.VideoDevice != null)
                {
                    this.VideoDevice.Dispose();
                    this.VideoDevice = null;
                }
            }
        }
    }
}
