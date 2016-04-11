namespace Toxofone.UI.PhoneBook
{
    partial class EmptyPhoneBookEntryControl
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
            this.addEntry = new Toxofone.UI.SvgPictureButton();
            this.phoneBookEntryControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // phoneBookEntryControls
            // 
            this.phoneBookEntryControls.ColumnCount = 2;
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.phoneBookEntryControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 600F));
            this.phoneBookEntryControls.Controls.Add(this.addEntry, 0, 0);
            this.phoneBookEntryControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.phoneBookEntryControls.Location = new System.Drawing.Point(0, 0);
            this.phoneBookEntryControls.Margin = new System.Windows.Forms.Padding(0);
            this.phoneBookEntryControls.Name = "phoneBookEntryControls";
            this.phoneBookEntryControls.RowCount = 1;
            this.phoneBookEntryControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.phoneBookEntryControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.phoneBookEntryControls.Size = new System.Drawing.Size(640, 48);
            this.phoneBookEntryControls.TabIndex = 0;
            // 
            // addEntry
            // 
            this.addEntry.BackColor = System.Drawing.Color.Transparent;
            this.addEntry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addEntry.Location = new System.Drawing.Point(8, 12);
            this.addEntry.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.addEntry.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.addEntry.Name = "addEntry";
            this.addEntry.Size = new System.Drawing.Size(24, 24);
            this.addEntry.EnabledSvgLocator = "res://Toxofone.Resources.Svg.add_circle_outline_black_24px.svg";
            this.addEntry.TabIndex = 0;
            // 
            // EmptyPhoneBookEntryControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.phoneBookEntryControls);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "EmptyPhoneBookEntryControl";
            this.Size = new System.Drawing.Size(640, 48);
            this.phoneBookEntryControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel phoneBookEntryControls;
        private SvgPictureButton addEntry;
    }
}
