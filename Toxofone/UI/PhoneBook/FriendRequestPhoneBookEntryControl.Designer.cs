namespace Toxofone.UI.PhoneBook
{
    partial class FriendRequestPhoneBookEntryControl
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
            this.friendRequestInfo = new System.Windows.Forms.TableLayoutPanel();
            this.requestPublicKey = new System.Windows.Forms.Label();
            this.requestMessage = new System.Windows.Forms.Label();
            this.acceptFriendRequest = new Toxofone.UI.SvgPictureButton();
            this.cancelFriendRequest = new Toxofone.UI.SvgPictureButton();
            this.phoneBookEntryControls.SuspendLayout();
            this.friendRequestInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // phoneBookEntryControls
            // 
            this.phoneBookEntryControls.ColumnCount = 3;
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 560F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.phoneBookEntryControls.Controls.Add(this.friendRequestInfo, 1, 0);
            this.phoneBookEntryControls.Controls.Add(this.acceptFriendRequest, 0, 0);
            this.phoneBookEntryControls.Controls.Add(this.cancelFriendRequest, 2, 0);
            this.phoneBookEntryControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.phoneBookEntryControls.Location = new System.Drawing.Point(0, 0);
            this.phoneBookEntryControls.Margin = new System.Windows.Forms.Padding(0);
            this.phoneBookEntryControls.Name = "phoneBookEntryControls";
            this.phoneBookEntryControls.RowCount = 1;
            this.phoneBookEntryControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.phoneBookEntryControls.Size = new System.Drawing.Size(640, 48);
            this.phoneBookEntryControls.TabIndex = 0;
            // 
            // friendRequestInfo
            // 
            this.friendRequestInfo.ColumnCount = 1;
            this.friendRequestInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.friendRequestInfo.Controls.Add(this.requestPublicKey, 0, 0);
            this.friendRequestInfo.Controls.Add(this.requestMessage, 0, 1);
            this.friendRequestInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.friendRequestInfo.Location = new System.Drawing.Point(40, 0);
            this.friendRequestInfo.Margin = new System.Windows.Forms.Padding(0);
            this.friendRequestInfo.Name = "friendRequestInfo";
            this.friendRequestInfo.RowCount = 2;
            this.friendRequestInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.friendRequestInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.friendRequestInfo.Size = new System.Drawing.Size(560, 48);
            this.friendRequestInfo.TabIndex = 0;
            // 
            // requestPublicKey
            // 
            this.requestPublicKey.AutoEllipsis = true;
            this.requestPublicKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.requestPublicKey.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.requestPublicKey.Location = new System.Drawing.Point(0, 0);
            this.requestPublicKey.Margin = new System.Windows.Forms.Padding(0);
            this.requestPublicKey.Name = "requestPublicKey";
            this.requestPublicKey.Size = new System.Drawing.Size(560, 28);
            this.requestPublicKey.TabIndex = 0;
            this.requestPublicKey.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.requestPublicKey.UseCompatibleTextRendering = true;
            // 
            // requestMessage
            // 
            this.requestMessage.AutoEllipsis = true;
            this.requestMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.requestMessage.Font = new System.Drawing.Font("Tahoma", 7.2F);
            this.requestMessage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.requestMessage.Location = new System.Drawing.Point(0, 28);
            this.requestMessage.Margin = new System.Windows.Forms.Padding(0);
            this.requestMessage.Name = "requestMessage";
            this.requestMessage.Size = new System.Drawing.Size(560, 20);
            this.requestMessage.TabIndex = 1;
            this.requestMessage.UseCompatibleTextRendering = true;
            // 
            // acceptFriendRequest
            // 
            this.acceptFriendRequest.BackColor = System.Drawing.Color.Transparent;
            this.acceptFriendRequest.DisabledSvgLocator = null;
            this.acceptFriendRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.acceptFriendRequest.EnabledSvgLocator = "res://Toxofone.Resources.Svg.check_circle_dark_green_24px.svg";
            this.acceptFriendRequest.Location = new System.Drawing.Point(8, 12);
            this.acceptFriendRequest.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.acceptFriendRequest.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.acceptFriendRequest.MouseOverSvgLocator = null;
            this.acceptFriendRequest.Name = "acceptFriendRequest";
            this.acceptFriendRequest.Size = new System.Drawing.Size(24, 24);
            this.acceptFriendRequest.TabIndex = 1;
            // 
            // cancelFriendRequest
            // 
            this.cancelFriendRequest.BackColor = System.Drawing.Color.Transparent;
            this.cancelFriendRequest.DisabledSvgLocator = null;
            this.cancelFriendRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cancelFriendRequest.EnabledSvgLocator = "res://Toxofone.Resources.Svg.cancel_circle_dark_red_24px.svg";
            this.cancelFriendRequest.Location = new System.Drawing.Point(608, 12);
            this.cancelFriendRequest.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.cancelFriendRequest.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cancelFriendRequest.MouseOverSvgLocator = null;
            this.cancelFriendRequest.Name = "cancelFriendRequest";
            this.cancelFriendRequest.Size = new System.Drawing.Size(24, 24);
            this.cancelFriendRequest.TabIndex = 2;
            // 
            // FriendRequestPhoneBookEntryControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.phoneBookEntryControls);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FriendRequestPhoneBookEntryControl";
            this.Size = new System.Drawing.Size(640, 48);
            this.phoneBookEntryControls.ResumeLayout(false);
            this.friendRequestInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel phoneBookEntryControls;
        private System.Windows.Forms.TableLayoutPanel friendRequestInfo;
        private SvgPictureButton acceptFriendRequest;
        private System.Windows.Forms.Label requestPublicKey;
        private System.Windows.Forms.Label requestMessage;
        private SvgPictureButton cancelFriendRequest;
    }
}
