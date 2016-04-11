namespace Toxofone.UI.UserInfo
{
    partial class CurrentUserInfoControl
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
            this.controls = new System.Windows.Forms.TableLayoutPanel();
            this.userName = new System.Windows.Forms.Label();
            this.userStatusMessage = new System.Windows.Forms.Label();
            this.controls.SuspendLayout();
            this.SuspendLayout();
            // 
            // controls
            // 
            this.controls.BackColor = System.Drawing.Color.Transparent;
            this.controls.ColumnCount = 1;
            this.controls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.controls.Controls.Add(this.userName, 0, 0);
            this.controls.Controls.Add(this.userStatusMessage, 0, 1);
            this.controls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controls.Location = new System.Drawing.Point(0, 0);
            this.controls.Margin = new System.Windows.Forms.Padding(0);
            this.controls.Name = "controls";
            this.controls.RowCount = 2;
            this.controls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.controls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.controls.Size = new System.Drawing.Size(224, 48);
            this.controls.TabIndex = 0;
            // 
            // userName
            // 
            this.userName.AutoEllipsis = true;
            this.userName.BackColor = System.Drawing.Color.Transparent;
            this.userName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.userName.Location = new System.Drawing.Point(0, 0);
            this.userName.Margin = new System.Windows.Forms.Padding(0);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(224, 28);
            this.userName.TabIndex = 0;
            this.userName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.userName.UseCompatibleTextRendering = true;
            // 
            // userStatusMessage
            // 
            this.userStatusMessage.AutoEllipsis = true;
            this.userStatusMessage.BackColor = System.Drawing.Color.Transparent;
            this.userStatusMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userStatusMessage.Font = new System.Drawing.Font("Tahoma", 7.2F);
            this.userStatusMessage.Location = new System.Drawing.Point(0, 28);
            this.userStatusMessage.Margin = new System.Windows.Forms.Padding(0);
            this.userStatusMessage.Name = "userStatusMessage";
            this.userStatusMessage.Size = new System.Drawing.Size(224, 20);
            this.userStatusMessage.TabIndex = 1;
            this.userStatusMessage.UseCompatibleTextRendering = true;
            // 
            // CurrentUserInfo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.controls);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CurrentUserInfo";
            this.Size = new System.Drawing.Size(224, 48);
            this.controls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel controls;
        private System.Windows.Forms.Label userName;
        private System.Windows.Forms.Label userStatusMessage;
    }
}
