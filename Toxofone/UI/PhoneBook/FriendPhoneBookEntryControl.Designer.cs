namespace Toxofone.UI.PhoneBook
{
    partial class FriendPhoneBookEntryControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.phoneBookEntryControls = new System.Windows.Forms.TableLayoutPanel();
            this.friendInfo = new System.Windows.Forms.TableLayoutPanel();
            this.displayStatusMessage = new System.Windows.Forms.Label();
            this.displayName = new System.Windows.Forms.Label();
            this.displayStatus = new Toxofone.UI.UserInfo.ToxUserStatusControl();
            this.removeEntry = new Toxofone.UI.SvgPictureButton();
            this.endCall = new Toxofone.UI.SvgPictureButton();
            this.startCall = new Toxofone.UI.SvgPictureButton();
            this.phoneBookEntryControls.SuspendLayout();
            this.friendInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // phoneBookEntryControls
            // 
            this.phoneBookEntryControls.BackColor = System.Drawing.Color.Transparent;
            this.phoneBookEntryControls.ColumnCount = 8;
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 392F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.phoneBookEntryControls.Controls.Add(this.friendInfo, 1, 0);
            this.phoneBookEntryControls.Controls.Add(this.displayStatus, 0, 0);
            this.phoneBookEntryControls.Controls.Add(this.removeEntry, 7, 0);
            this.phoneBookEntryControls.Controls.Add(this.endCall, 3, 0);
            this.phoneBookEntryControls.Controls.Add(this.startCall, 5, 0);
            this.phoneBookEntryControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.phoneBookEntryControls.Location = new System.Drawing.Point(0, 0);
            this.phoneBookEntryControls.Margin = new System.Windows.Forms.Padding(0);
            this.phoneBookEntryControls.Name = "phoneBookEntryControls";
            this.phoneBookEntryControls.RowCount = 1;
            this.phoneBookEntryControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.phoneBookEntryControls.Size = new System.Drawing.Size(640, 48);
            this.phoneBookEntryControls.TabIndex = 0;
            // 
            // friendInfo
            // 
            this.friendInfo.ColumnCount = 1;
            this.friendInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.friendInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.friendInfo.Controls.Add(this.displayStatusMessage, 0, 1);
            this.friendInfo.Controls.Add(this.displayName, 0, 0);
            this.friendInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.friendInfo.Location = new System.Drawing.Point(40, 0);
            this.friendInfo.Margin = new System.Windows.Forms.Padding(0);
            this.friendInfo.Name = "friendInfo";
            this.friendInfo.RowCount = 2;
            this.friendInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.friendInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.friendInfo.Size = new System.Drawing.Size(392, 48);
            this.friendInfo.TabIndex = 0;
            // 
            // displayStatusMessage
            // 
            this.displayStatusMessage.AutoEllipsis = true;
            this.displayStatusMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayStatusMessage.Font = new System.Drawing.Font("Tahoma", 7.2F);
            this.displayStatusMessage.Location = new System.Drawing.Point(0, 28);
            this.displayStatusMessage.Margin = new System.Windows.Forms.Padding(0);
            this.displayStatusMessage.Name = "displayStatusMessage";
            this.displayStatusMessage.Size = new System.Drawing.Size(392, 20);
            this.displayStatusMessage.TabIndex = 1;
            this.displayStatusMessage.UseCompatibleTextRendering = true;
            // 
            // displayName
            // 
            this.displayName.AutoEllipsis = true;
            this.displayName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.displayName.Location = new System.Drawing.Point(0, 0);
            this.displayName.Margin = new System.Windows.Forms.Padding(0);
            this.displayName.Name = "displayName";
            this.displayName.Size = new System.Drawing.Size(392, 28);
            this.displayName.TabIndex = 2;
            this.displayName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.displayName.UseCompatibleTextRendering = true;
            // 
            // displayStatus
            // 
            this.displayStatus.BackColor = System.Drawing.Color.Transparent;
            this.displayStatus.ConnectionStatus = SharpTox.Core.ToxConnectionStatus.None;
            this.displayStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayStatus.Location = new System.Drawing.Point(10, 14);
            this.displayStatus.Margin = new System.Windows.Forms.Padding(10, 14, 10, 14);
            this.displayStatus.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.displayStatus.Name = "displayStatus";
            this.displayStatus.Notification = false;
            this.displayStatus.Size = new System.Drawing.Size(20, 20);
            this.displayStatus.TabIndex = 1;
            this.displayStatus.UserStatus = SharpTox.Core.ToxUserStatus.None;
            // 
            // removeEntry
            // 
            this.removeEntry.BackColor = System.Drawing.Color.Transparent;
            this.removeEntry.DisabledSvgLocator = "res://Toxofone.Resources.Svg.remove_circle_outline_gray50_24px.svg";
            this.removeEntry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.removeEntry.EnabledSvgLocator = "res://Toxofone.Resources.Svg.remove_circle_outline_black_24px.svg";
            this.removeEntry.Location = new System.Drawing.Point(608, 12);
            this.removeEntry.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.removeEntry.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.removeEntry.MouseOverSvgLocator = null;
            this.removeEntry.Name = "removeEntry";
            this.removeEntry.Size = new System.Drawing.Size(24, 24);
            this.removeEntry.TabIndex = 3;
            // 
            // endCall
            // 
            this.endCall.BackColor = System.Drawing.Color.Transparent;
            this.endCall.DisabledSvgLocator = "res://Toxofone.Resources.Svg.call_end_gray50_36px.svg";
            this.endCall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.endCall.EnabledSvgLocator = "res://Toxofone.Resources.Svg.call_end_red_36px.svg";
            this.endCall.Location = new System.Drawing.Point(462, 6);
            this.endCall.Margin = new System.Windows.Forms.Padding(6);
            this.endCall.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.endCall.MouseOverSvgLocator = "res://Toxofone.Resources.Svg.call_end_red_dimmed_36px.svg";
            this.endCall.Name = "endCall";
            this.endCall.Size = new System.Drawing.Size(36, 36);
            this.endCall.TabIndex = 4;
            // 
            // startCall
            // 
            this.startCall.BackColor = System.Drawing.Color.Transparent;
            this.startCall.DisabledSvgLocator = "res://Toxofone.Resources.Svg.call_gray50_36px.svg";
            this.startCall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startCall.EnabledSvgLocator = "res://Toxofone.Resources.Svg.call_green_36px.svg";
            this.startCall.Location = new System.Drawing.Point(534, 6);
            this.startCall.Margin = new System.Windows.Forms.Padding(6);
            this.startCall.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.startCall.MouseOverSvgLocator = "res://Toxofone.Resources.Svg.call_green_dimmed_36px.svg";
            this.startCall.Name = "startCall";
            this.startCall.Size = new System.Drawing.Size(36, 36);
            this.startCall.TabIndex = 5;
            // 
            // FriendPhoneBookEntryControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.phoneBookEntryControls);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FriendPhoneBookEntryControl";
            this.Size = new System.Drawing.Size(640, 48);
            this.phoneBookEntryControls.ResumeLayout(false);
            this.friendInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel phoneBookEntryControls;
        private System.Windows.Forms.TableLayoutPanel friendInfo;
        private UserInfo.ToxUserStatusControl displayStatus;
        private System.Windows.Forms.Label displayStatusMessage;
        private SvgPictureButton removeEntry;
        private SvgPictureButton endCall;
        private SvgPictureButton startCall;
        private System.Windows.Forms.Label displayName;
    }
}
