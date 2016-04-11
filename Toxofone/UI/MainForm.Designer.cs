namespace Toxofone.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.topPanel = new System.Windows.Forms.Panel();
            this.phoneBook = new System.Windows.Forms.FlowLayoutPanel();
            this.friendVideoCamera = new System.Windows.Forms.PictureBox();
            this.leftBottomPanel = new System.Windows.Forms.Panel();
            this.controlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.rightBottomPanel = new System.Windows.Forms.Panel();
            this.localVideoCamera = new System.Windows.Forms.PictureBox();
            this.recordingDeviceInfo = new Toxofone.UI.MediaControl.AudioRecordingDeviceInfoControl();
            this.playbackDeviceInfo = new Toxofone.UI.MediaControl.DeviceInfoControl();
            this.ringingDeviceInfo = new Toxofone.UI.MediaControl.DeviceInfoControl();
            this.videoDeviceInfo = new Toxofone.UI.MediaControl.DeviceInfoControl();
            this.currentUserInfo = new Toxofone.UI.UserInfo.CurrentUserInfoControl();
            this.currentUserStatus = new Toxofone.UI.UserInfo.ToxUserStatusControl();
            this.recordingDeviceStatus = new Toxofone.UI.MediaControl.MediaDeviceStatusControl();
            this.playbackDeviceStatus = new Toxofone.UI.MediaControl.MediaDeviceStatusControl();
            this.ringingDeviceStatus = new Toxofone.UI.MediaControl.MediaDeviceStatusControl();
            this.videoDeviceStatus = new Toxofone.UI.MediaControl.MediaDeviceStatusControl();
            this.currentUserSettings = new Toxofone.UI.SvgPictureButton();
            this.testPlaybackSound = new Toxofone.UI.SvgPictureButton();
            this.testRingingSound = new Toxofone.UI.SvgPictureButton();
            this.videoDeviceSettings = new Toxofone.UI.SvgPictureButton();
            this.blockFriendVideo = new Toxofone.UI.SvgPictureButton();
            this.playFriendVideo = new Toxofone.UI.SvgPictureButton();
            this.endFriendCall = new Toxofone.UI.SvgPictureButton();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.friendVideoCamera)).BeginInit();
            this.leftBottomPanel.SuspendLayout();
            this.controlPanel.SuspendLayout();
            this.rightBottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.localVideoCamera)).BeginInit();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.SystemColors.Control;
            this.topPanel.Controls.Add(this.phoneBook);
            this.topPanel.Controls.Add(this.friendVideoCamera);
            this.topPanel.Controls.Add(this.blockFriendVideo);
            this.topPanel.Controls.Add(this.playFriendVideo);
            this.topPanel.Controls.Add(this.endFriendCall);
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(640, 480);
            this.topPanel.TabIndex = 0;
            // 
            // phoneBook
            // 
            this.phoneBook.BackgroundImage = global::Toxofone.Properties.Resources.phone_book_background;
            this.phoneBook.Dock = System.Windows.Forms.DockStyle.Fill;
            this.phoneBook.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.phoneBook.Location = new System.Drawing.Point(0, 0);
            this.phoneBook.Margin = new System.Windows.Forms.Padding(0);
            this.phoneBook.Name = "phoneBook";
            this.phoneBook.Size = new System.Drawing.Size(640, 480);
            this.phoneBook.TabIndex = 1;
            this.phoneBook.WrapContents = false;
            // 
            // friendVideoCamera
            // 
            this.friendVideoCamera.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.friendVideoCamera.BackColor = System.Drawing.Color.Transparent;
            this.friendVideoCamera.BackgroundImage = global::Toxofone.Properties.Resources.old_fashion_phone;
            this.friendVideoCamera.Location = new System.Drawing.Point(0, 0);
            this.friendVideoCamera.Margin = new System.Windows.Forms.Padding(0);
            this.friendVideoCamera.Name = "friendVideoCamera";
            this.friendVideoCamera.Size = new System.Drawing.Size(640, 480);
            this.friendVideoCamera.TabIndex = 0;
            this.friendVideoCamera.TabStop = false;
            // 
            // leftBottomPanel
            // 
            this.leftBottomPanel.Controls.Add(this.controlPanel);
            this.leftBottomPanel.Location = new System.Drawing.Point(0, 480);
            this.leftBottomPanel.Margin = new System.Windows.Forms.Padding(0);
            this.leftBottomPanel.Name = "leftBottomPanel";
            this.leftBottomPanel.Size = new System.Drawing.Size(320, 240);
            this.leftBottomPanel.TabIndex = 1;
            // 
            // controlPanel
            // 
            this.controlPanel.ColumnCount = 3;
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.controlPanel.Controls.Add(this.recordingDeviceInfo, 1, 0);
            this.controlPanel.Controls.Add(this.playbackDeviceInfo, 1, 1);
            this.controlPanel.Controls.Add(this.ringingDeviceInfo, 1, 2);
            this.controlPanel.Controls.Add(this.videoDeviceInfo, 1, 3);
            this.controlPanel.Controls.Add(this.currentUserInfo, 1, 4);
            this.controlPanel.Controls.Add(this.currentUserStatus, 0, 4);
            this.controlPanel.Controls.Add(this.recordingDeviceStatus, 0, 0);
            this.controlPanel.Controls.Add(this.playbackDeviceStatus, 0, 1);
            this.controlPanel.Controls.Add(this.ringingDeviceStatus, 0, 2);
            this.controlPanel.Controls.Add(this.videoDeviceStatus, 0, 3);
            this.controlPanel.Controls.Add(this.currentUserSettings, 2, 4);
            this.controlPanel.Controls.Add(this.testPlaybackSound, 2, 1);
            this.controlPanel.Controls.Add(this.testRingingSound, 2, 2);
            this.controlPanel.Controls.Add(this.videoDeviceSettings, 2, 3);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlPanel.Location = new System.Drawing.Point(0, 0);
            this.controlPanel.Margin = new System.Windows.Forms.Padding(0);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.RowCount = 5;
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.controlPanel.Size = new System.Drawing.Size(320, 240);
            this.controlPanel.TabIndex = 0;
            // 
            // rightBottomPanel
            // 
            this.rightBottomPanel.Controls.Add(this.localVideoCamera);
            this.rightBottomPanel.Location = new System.Drawing.Point(320, 480);
            this.rightBottomPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightBottomPanel.Name = "rightBottomPanel";
            this.rightBottomPanel.Size = new System.Drawing.Size(320, 240);
            this.rightBottomPanel.TabIndex = 2;
            // 
            // localVideoCamera
            // 
            this.localVideoCamera.BackColor = System.Drawing.Color.Transparent;
            this.localVideoCamera.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("localVideoCamera.BackgroundImage")));
            this.localVideoCamera.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.localVideoCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.localVideoCamera.Location = new System.Drawing.Point(0, 0);
            this.localVideoCamera.Name = "localVideoCamera";
            this.localVideoCamera.Size = new System.Drawing.Size(320, 240);
            this.localVideoCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.localVideoCamera.TabIndex = 0;
            this.localVideoCamera.TabStop = false;
            // 
            // recordingDeviceInfo
            // 
            this.recordingDeviceInfo.BackColor = System.Drawing.Color.Transparent;
            this.recordingDeviceInfo.DeviceName = "";
            this.recordingDeviceInfo.DeviceType = "RecordingDevice";
            this.recordingDeviceInfo.DeviceVolume = 0;
            this.recordingDeviceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recordingDeviceInfo.Location = new System.Drawing.Point(40, 0);
            this.recordingDeviceInfo.Margin = new System.Windows.Forms.Padding(0);
            this.recordingDeviceInfo.Name = "recordingDeviceInfo";
            this.recordingDeviceInfo.ProgressBarBackColor = System.Drawing.SystemColors.Control;
            this.recordingDeviceInfo.Size = new System.Drawing.Size(240, 48);
            this.recordingDeviceInfo.TabIndex = 5;
            // 
            // playbackDeviceInfo
            // 
            this.playbackDeviceInfo.BackColor = System.Drawing.Color.Transparent;
            this.playbackDeviceInfo.DeviceName = "";
            this.playbackDeviceInfo.DeviceType = "PlaybackDevice";
            this.playbackDeviceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playbackDeviceInfo.Location = new System.Drawing.Point(40, 48);
            this.playbackDeviceInfo.Margin = new System.Windows.Forms.Padding(0);
            this.playbackDeviceInfo.Name = "playbackDeviceInfo";
            this.playbackDeviceInfo.Size = new System.Drawing.Size(240, 48);
            this.playbackDeviceInfo.TabIndex = 6;
            // 
            // ringingDeviceInfo
            // 
            this.ringingDeviceInfo.BackColor = System.Drawing.Color.Transparent;
            this.ringingDeviceInfo.DeviceName = "";
            this.ringingDeviceInfo.DeviceType = "RingingDevice";
            this.ringingDeviceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ringingDeviceInfo.Location = new System.Drawing.Point(40, 96);
            this.ringingDeviceInfo.Margin = new System.Windows.Forms.Padding(0);
            this.ringingDeviceInfo.Name = "ringingDeviceInfo";
            this.ringingDeviceInfo.Size = new System.Drawing.Size(240, 48);
            this.ringingDeviceInfo.TabIndex = 7;
            // 
            // videoDeviceInfo
            // 
            this.videoDeviceInfo.BackColor = System.Drawing.Color.Transparent;
            this.videoDeviceInfo.DeviceName = "";
            this.videoDeviceInfo.DeviceType = "VideoDevice";
            this.videoDeviceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoDeviceInfo.Location = new System.Drawing.Point(40, 144);
            this.videoDeviceInfo.Margin = new System.Windows.Forms.Padding(0);
            this.videoDeviceInfo.Name = "videoDeviceInfo";
            this.videoDeviceInfo.Size = new System.Drawing.Size(240, 48);
            this.videoDeviceInfo.TabIndex = 8;
            // 
            // currentUserInfo
            // 
            this.currentUserInfo.BackColor = System.Drawing.Color.Transparent;
            this.currentUserInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentUserInfo.Location = new System.Drawing.Point(40, 192);
            this.currentUserInfo.Margin = new System.Windows.Forms.Padding(0);
            this.currentUserInfo.Name = "currentUserInfo";
            this.currentUserInfo.Size = new System.Drawing.Size(240, 48);
            this.currentUserInfo.TabIndex = 9;
            this.currentUserInfo.UserName = "";
            this.currentUserInfo.UserStatusMessage = "";
            // 
            // currentUserStatus
            // 
            this.currentUserStatus.BackColor = System.Drawing.Color.Transparent;
            this.currentUserStatus.ConnectionStatus = SharpTox.Core.ToxConnectionStatus.None;
            this.currentUserStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentUserStatus.Location = new System.Drawing.Point(10, 206);
            this.currentUserStatus.Margin = new System.Windows.Forms.Padding(10, 14, 10, 14);
            this.currentUserStatus.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.currentUserStatus.Name = "currentUserStatus";
            this.currentUserStatus.Notification = false;
            this.currentUserStatus.Size = new System.Drawing.Size(20, 20);
            this.currentUserStatus.TabIndex = 15;
            this.currentUserStatus.UserStatus = SharpTox.Core.ToxUserStatus.None;
            // 
            // recordingDeviceStatus
            // 
            this.recordingDeviceStatus.BackColor = System.Drawing.Color.Transparent;
            this.recordingDeviceStatus.DeviceNoneSvgLocator = "res://Toxofone.Resources.Svg.recording_device_gray50_24px.svg";
            this.recordingDeviceStatus.DeviceOffSvgLocator = "res://Toxofone.Resources.Svg.recording_device_off_black_24px.svg";
            this.recordingDeviceStatus.DeviceOnSvgLocator = "res://Toxofone.Resources.Svg.recording_device_black_24px.svg";
            this.recordingDeviceStatus.DeviceStatus = Toxofone.UI.MediaControl.MediaDeviceStatus.None;
            this.recordingDeviceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recordingDeviceStatus.Enabled = false;
            this.recordingDeviceStatus.Location = new System.Drawing.Point(8, 12);
            this.recordingDeviceStatus.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.recordingDeviceStatus.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.recordingDeviceStatus.Name = "recordingDeviceStatus";
            this.recordingDeviceStatus.Size = new System.Drawing.Size(24, 24);
            this.recordingDeviceStatus.TabIndex = 16;
            // 
            // playbackDeviceStatus
            // 
            this.playbackDeviceStatus.BackColor = System.Drawing.Color.Transparent;
            this.playbackDeviceStatus.DeviceNoneSvgLocator = "res://Toxofone.Resources.Svg.playback_device_gray50_24px.svg";
            this.playbackDeviceStatus.DeviceOffSvgLocator = "res://Toxofone.Resources.Svg.playback_device_off_black_24px.svg";
            this.playbackDeviceStatus.DeviceOnSvgLocator = "res://Toxofone.Resources.Svg.playback_device_black_24px.svg";
            this.playbackDeviceStatus.DeviceStatus = Toxofone.UI.MediaControl.MediaDeviceStatus.None;
            this.playbackDeviceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playbackDeviceStatus.Enabled = false;
            this.playbackDeviceStatus.Location = new System.Drawing.Point(8, 60);
            this.playbackDeviceStatus.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.playbackDeviceStatus.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.playbackDeviceStatus.Name = "playbackDeviceStatus";
            this.playbackDeviceStatus.Size = new System.Drawing.Size(24, 24);
            this.playbackDeviceStatus.TabIndex = 17;
            // 
            // ringingDeviceStatus
            // 
            this.ringingDeviceStatus.BackColor = System.Drawing.Color.Transparent;
            this.ringingDeviceStatus.DeviceNoneSvgLocator = "res://Toxofone.Resources.Svg.ringing_device_gray50_24px.svg";
            this.ringingDeviceStatus.DeviceOffSvgLocator = "res://Toxofone.Resources.Svg.ringing_device_off_black_24px.svg";
            this.ringingDeviceStatus.DeviceOnSvgLocator = "res://Toxofone.Resources.Svg.ringing_device_black_24px.svg";
            this.ringingDeviceStatus.DeviceStatus = Toxofone.UI.MediaControl.MediaDeviceStatus.None;
            this.ringingDeviceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ringingDeviceStatus.Enabled = false;
            this.ringingDeviceStatus.Location = new System.Drawing.Point(8, 108);
            this.ringingDeviceStatus.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.ringingDeviceStatus.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ringingDeviceStatus.Name = "ringingDeviceStatus";
            this.ringingDeviceStatus.Size = new System.Drawing.Size(24, 24);
            this.ringingDeviceStatus.TabIndex = 18;
            // 
            // videoDeviceStatus
            // 
            this.videoDeviceStatus.BackColor = System.Drawing.Color.Transparent;
            this.videoDeviceStatus.DeviceNoneSvgLocator = "res://Toxofone.Resources.Svg.video_device_gray50_24px.svg";
            this.videoDeviceStatus.DeviceOffSvgLocator = "res://Toxofone.Resources.Svg.video_device_off_black_24px.svg";
            this.videoDeviceStatus.DeviceOnSvgLocator = "res://Toxofone.Resources.Svg.video_device_black_24px.svg";
            this.videoDeviceStatus.DeviceStatus = Toxofone.UI.MediaControl.MediaDeviceStatus.None;
            this.videoDeviceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoDeviceStatus.Enabled = false;
            this.videoDeviceStatus.Location = new System.Drawing.Point(8, 156);
            this.videoDeviceStatus.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.videoDeviceStatus.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.videoDeviceStatus.Name = "videoDeviceStatus";
            this.videoDeviceStatus.Size = new System.Drawing.Size(24, 24);
            this.videoDeviceStatus.TabIndex = 19;
            // 
            // currentUserSettings
            // 
            this.currentUserSettings.BackColor = System.Drawing.Color.Transparent;
            this.currentUserSettings.DisabledSvgLocator = null;
            this.currentUserSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentUserSettings.EnabledSvgLocator = "res://Toxofone.Resources.Svg.settings_black_24px.svg";
            this.currentUserSettings.Location = new System.Drawing.Point(288, 204);
            this.currentUserSettings.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.currentUserSettings.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.currentUserSettings.MouseOverSvgLocator = null;
            this.currentUserSettings.Name = "currentUserSettings";
            this.currentUserSettings.Size = new System.Drawing.Size(24, 24);
            this.currentUserSettings.TabIndex = 21;
            // 
            // testPlaybackSound
            // 
            this.testPlaybackSound.BackColor = System.Drawing.Color.Transparent;
            this.testPlaybackSound.DisabledSvgLocator = "res://Toxofone.Resources.Svg.play_circle_outline_gray50_24px.svg";
            this.testPlaybackSound.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testPlaybackSound.EnabledSvgLocator = "res://Toxofone.Resources.Svg.play_circle_outline_black_24px.svg";
            this.testPlaybackSound.Location = new System.Drawing.Point(288, 60);
            this.testPlaybackSound.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.testPlaybackSound.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.testPlaybackSound.MouseOverSvgLocator = null;
            this.testPlaybackSound.Name = "testPlaybackSound";
            this.testPlaybackSound.Size = new System.Drawing.Size(24, 24);
            this.testPlaybackSound.TabIndex = 23;
            // 
            // testRingingSound
            // 
            this.testRingingSound.BackColor = System.Drawing.Color.Transparent;
            this.testRingingSound.DisabledSvgLocator = "res://Toxofone.Resources.Svg.play_circle_outline_gray50_24px.svg";
            this.testRingingSound.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testRingingSound.EnabledSvgLocator = "res://Toxofone.Resources.Svg.play_circle_outline_black_24px.svg";
            this.testRingingSound.Location = new System.Drawing.Point(288, 108);
            this.testRingingSound.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.testRingingSound.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.testRingingSound.MouseOverSvgLocator = null;
            this.testRingingSound.Name = "testRingingSound";
            this.testRingingSound.Size = new System.Drawing.Size(24, 24);
            this.testRingingSound.TabIndex = 25;
            // 
            // videoDeviceSettings
            // 
            this.videoDeviceSettings.BackColor = System.Drawing.Color.Transparent;
            this.videoDeviceSettings.DisabledSvgLocator = "res://Toxofone.Resources.Svg.settings_gray50_24px.svg";
            this.videoDeviceSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoDeviceSettings.EnabledSvgLocator = "res://Toxofone.Resources.Svg.settings_black_24px.svg";
            this.videoDeviceSettings.Location = new System.Drawing.Point(288, 156);
            this.videoDeviceSettings.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.videoDeviceSettings.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.videoDeviceSettings.MouseOverSvgLocator = null;
            this.videoDeviceSettings.Name = "videoDeviceSettings";
            this.videoDeviceSettings.Size = new System.Drawing.Size(24, 24);
            this.videoDeviceSettings.TabIndex = 27;
            // 
            // blockFriendVideo
            // 
            this.blockFriendVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blockFriendVideo.BackColor = System.Drawing.Color.Transparent;
            this.blockFriendVideo.DisabledSvgLocator = null;
            this.blockFriendVideo.EnabledSvgLocator = "res://Toxofone.Resources.Svg.block_friend_video_36px.svg";
            this.blockFriendVideo.Location = new System.Drawing.Point(2, 438);
            this.blockFriendVideo.Margin = new System.Windows.Forms.Padding(0);
            this.blockFriendVideo.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.blockFriendVideo.MouseOverSvgLocator = "res://Toxofone.Resources.Svg.block_friend_video_dimmed_36px.svg";
            this.blockFriendVideo.Name = "blockFriendVideo";
            this.blockFriendVideo.Size = new System.Drawing.Size(36, 36);
            this.blockFriendVideo.TabIndex = 0;
            // 
            // playFriendVideo
            // 
            this.playFriendVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.playFriendVideo.BackColor = System.Drawing.Color.Transparent;
            this.playFriendVideo.DisabledSvgLocator = null;
            this.playFriendVideo.EnabledSvgLocator = "res://Toxofone.Resources.Svg.play_friend_video_36px.svg";
            this.playFriendVideo.Location = new System.Drawing.Point(2, 438);
            this.playFriendVideo.Margin = new System.Windows.Forms.Padding(0);
            this.playFriendVideo.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.playFriendVideo.MouseOverSvgLocator = "res://Toxofone.Resources.Svg.play_friend_video_dimmed_36px.svg";
            this.playFriendVideo.Name = "playFriendVideo";
            this.playFriendVideo.Size = new System.Drawing.Size(36, 36);
            this.playFriendVideo.TabIndex = 0;
            // 
            // endFriendCall
            // 
            this.endFriendCall.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.endFriendCall.BackColor = System.Drawing.Color.Transparent;
            this.endFriendCall.DisabledSvgLocator = null;
            this.endFriendCall.EnabledSvgLocator = "res://Toxofone.Resources.Svg.call_end_in_circle_red_60px.svg";
            this.endFriendCall.Location = new System.Drawing.Point(572, 412);
            this.endFriendCall.Margin = new System.Windows.Forms.Padding(0);
            this.endFriendCall.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.endFriendCall.MouseOverSvgLocator = "res://Toxofone.Resources.Svg.call_end_in_circle_red_dimmed_60px.svg";
            this.endFriendCall.Name = "endFriendCall";
            this.endFriendCall.Size = new System.Drawing.Size(60, 60);
            this.endFriendCall.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(640, 720);
            this.Controls.Add(this.rightBottomPanel);
            this.Controls.Add(this.leftBottomPanel);
            this.Controls.Add(this.topPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Toxofone";
            this.topPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.friendVideoCamera)).EndInit();
            this.leftBottomPanel.ResumeLayout(false);
            this.controlPanel.ResumeLayout(false);
            this.rightBottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.localVideoCamera)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Panel leftBottomPanel;
        private System.Windows.Forms.Panel rightBottomPanel;
        private System.Windows.Forms.PictureBox localVideoCamera;
        private System.Windows.Forms.FlowLayoutPanel phoneBook;
        private System.Windows.Forms.TableLayoutPanel controlPanel;
        private MediaControl.AudioRecordingDeviceInfoControl recordingDeviceInfo;
        private MediaControl.DeviceInfoControl playbackDeviceInfo;
        private MediaControl.DeviceInfoControl ringingDeviceInfo;
        private MediaControl.DeviceInfoControl videoDeviceInfo;
        private UserInfo.CurrentUserInfoControl currentUserInfo;
        private UserInfo.ToxUserStatusControl currentUserStatus;
        private MediaControl.MediaDeviceStatusControl recordingDeviceStatus;
        private MediaControl.MediaDeviceStatusControl playbackDeviceStatus;
        private MediaControl.MediaDeviceStatusControl ringingDeviceStatus;
        private MediaControl.MediaDeviceStatusControl videoDeviceStatus;
        private SvgPictureButton currentUserSettings;
        private SvgPictureButton testPlaybackSound;
        private SvgPictureButton testRingingSound;
        private SvgPictureButton videoDeviceSettings;
        private System.Windows.Forms.PictureBox friendVideoCamera;
        private SvgPictureButton endFriendCall;
        private SvgPictureButton blockFriendVideo;
        private SvgPictureButton playFriendVideo;
    }
}

