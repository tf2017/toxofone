namespace Toxofone.UI.MediaControl
{
    partial class DeviceInfoControl
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
            this.deviceNameHolder = new System.Windows.Forms.TableLayoutPanel();
            this.deviceName = new System.Windows.Forms.ComboBox();
            this.deviceNameHolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // deviceNameHolder
            // 
            this.deviceNameHolder.BackColor = System.Drawing.Color.Transparent;
            this.deviceNameHolder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.deviceNameHolder.Controls.Add(this.deviceName, 0, 1);
            this.deviceNameHolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceNameHolder.Location = new System.Drawing.Point(0, 0);
            this.deviceNameHolder.Margin = new System.Windows.Forms.Padding(0);
            this.deviceNameHolder.Name = "deviceNameHolder";
            this.deviceNameHolder.RowCount = 3;
            this.deviceNameHolder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.deviceNameHolder.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.deviceNameHolder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.deviceNameHolder.Size = new System.Drawing.Size(200, 48);
            this.deviceNameHolder.TabIndex = 0;
            // 
            // deviceName
            // 
            this.deviceName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceName.FormattingEnabled = true;
            this.deviceName.Location = new System.Drawing.Point(0, 12);
            this.deviceName.Margin = new System.Windows.Forms.Padding(0);
            this.deviceName.Name = "deviceName";
            this.deviceName.Size = new System.Drawing.Size(200, 24);
            this.deviceName.TabIndex = 0;
            // 
            // DeviceInfoControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.deviceNameHolder);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DeviceInfoControl";
            this.Size = new System.Drawing.Size(200, 48);
            this.deviceNameHolder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel deviceNameHolder;
        private System.Windows.Forms.ComboBox deviceName;
    }
}
