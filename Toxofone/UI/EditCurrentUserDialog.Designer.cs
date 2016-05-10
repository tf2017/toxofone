namespace Toxofone.UI
{
    partial class EditCurrentUserDialog
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.statusMessageLabel = new System.Windows.Forms.Label();
            this.statusMessageTextBox = new System.Windows.Forms.TextBox();
            this.buttonsPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.buttonsSeparator = new System.Windows.Forms.Label();
            this.toxIdLabel = new System.Windows.Forms.Label();
            this.toxIdStaticText = new System.Windows.Forms.Label();
            this.toxIdStaticText2 = new System.Windows.Forms.Label();
            this.copyToClipboardButton = new System.Windows.Forms.Button();
            this.copiedToClipboardLabel = new System.Windows.Forms.Label();
            this.buttonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(17, 9);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(41, 20);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Name";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.nameLabel.UseCompatibleTextRendering = true;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(17, 33);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(567, 22);
            this.nameTextBox.TabIndex = 1;
            this.nameTextBox.WordWrap = false;
            // 
            // statusMessageLabel
            // 
            this.statusMessageLabel.AutoSize = true;
            this.statusMessageLabel.Location = new System.Drawing.Point(17, 68);
            this.statusMessageLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statusMessageLabel.Name = "statusMessageLabel";
            this.statusMessageLabel.Size = new System.Drawing.Size(101, 20);
            this.statusMessageLabel.TabIndex = 2;
            this.statusMessageLabel.Text = "Status message";
            this.statusMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.statusMessageLabel.UseCompatibleTextRendering = true;
            // 
            // statusMessageTextBox
            // 
            this.statusMessageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusMessageTextBox.Location = new System.Drawing.Point(17, 94);
            this.statusMessageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.statusMessageTextBox.Name = "statusMessageTextBox";
            this.statusMessageTextBox.Size = new System.Drawing.Size(567, 22);
            this.statusMessageTextBox.TabIndex = 3;
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonsPanel.BackColor = System.Drawing.SystemColors.Control;
            this.buttonsPanel.Controls.Add(this.cancelButton);
            this.buttonsPanel.Controls.Add(this.okButton);
            this.buttonsPanel.Controls.Add(this.buttonsSeparator);
            this.buttonsPanel.Location = new System.Drawing.Point(0, 210);
            this.buttonsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.Size = new System.Drawing.Size(602, 57);
            this.buttonsPanel.TabIndex = 4;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(466, 15);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(120, 28);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(330, 15);
            this.okButton.Margin = new System.Windows.Forms.Padding(4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(120, 28);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseCompatibleTextRendering = true;
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // buttonsSeparator
            // 
            this.buttonsSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonsSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonsSeparator.Location = new System.Drawing.Point(0, 0);
            this.buttonsSeparator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.buttonsSeparator.Name = "buttonsSeparator";
            this.buttonsSeparator.Size = new System.Drawing.Size(602, 2);
            this.buttonsSeparator.TabIndex = 8;
            // 
            // toxIdLabel
            // 
            this.toxIdLabel.Location = new System.Drawing.Point(17, 138);
            this.toxIdLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toxIdLabel.Name = "toxIdLabel";
            this.toxIdLabel.Size = new System.Drawing.Size(54, 26);
            this.toxIdLabel.TabIndex = 4;
            this.toxIdLabel.Text = "Tox ID:";
            this.toxIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toxIdLabel.UseCompatibleTextRendering = true;
            // 
            // toxIdStaticText
            // 
            this.toxIdStaticText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toxIdStaticText.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.toxIdStaticText.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toxIdStaticText.Location = new System.Drawing.Point(78, 138);
            this.toxIdStaticText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toxIdStaticText.Name = "toxIdStaticText";
            this.toxIdStaticText.Size = new System.Drawing.Size(342, 26);
            this.toxIdStaticText.TabIndex = 5;
            this.toxIdStaticText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toxIdStaticText.UseCompatibleTextRendering = true;
            // 
            // toxIdStaticText2
            // 
            this.toxIdStaticText2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toxIdStaticText2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.toxIdStaticText2.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toxIdStaticText2.Location = new System.Drawing.Point(78, 164);
            this.toxIdStaticText2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toxIdStaticText2.Name = "toxIdStaticText2";
            this.toxIdStaticText2.Size = new System.Drawing.Size(342, 26);
            this.toxIdStaticText2.TabIndex = 6;
            this.toxIdStaticText2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toxIdStaticText2.UseCompatibleTextRendering = true;
            // 
            // copyToClipboardButton
            // 
            this.copyToClipboardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.copyToClipboardButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.copyToClipboardButton.Location = new System.Drawing.Point(465, 138);
            this.copyToClipboardButton.Margin = new System.Windows.Forms.Padding(4);
            this.copyToClipboardButton.Name = "copyToClipboardButton";
            this.copyToClipboardButton.Size = new System.Drawing.Size(120, 28);
            this.copyToClipboardButton.TabIndex = 7;
            this.copyToClipboardButton.Text = "Copy Tox ID";
            this.copyToClipboardButton.UseCompatibleTextRendering = true;
            this.copyToClipboardButton.UseVisualStyleBackColor = true;
            // 
            // copiedToClipboardLabel
            // 
            this.copiedToClipboardLabel.Font = new System.Drawing.Font("Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.copiedToClipboardLabel.Location = new System.Drawing.Point(425, 138);
            this.copiedToClipboardLabel.Name = "copiedToClipboardLabel";
            this.copiedToClipboardLabel.Size = new System.Drawing.Size(29, 28);
            this.copiedToClipboardLabel.TabIndex = 8;
            this.copiedToClipboardLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // EditCurrentUserDialog
            // 
            this.AcceptButton = this.cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(602, 267);
            this.Controls.Add(this.copiedToClipboardLabel);
            this.Controls.Add(this.copyToClipboardButton);
            this.Controls.Add(this.toxIdStaticText2);
            this.Controls.Add(this.toxIdStaticText);
            this.Controls.Add(this.toxIdLabel);
            this.Controls.Add(this.buttonsPanel);
            this.Controls.Add(this.statusMessageTextBox);
            this.Controls.Add(this.statusMessageLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.nameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditCurrentUserDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Current user";
            this.buttonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label statusMessageLabel;
        private System.Windows.Forms.TextBox statusMessageTextBox;
        private System.Windows.Forms.Panel buttonsPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label buttonsSeparator;
        private System.Windows.Forms.Label toxIdLabel;
        private System.Windows.Forms.Label toxIdStaticText;
        private System.Windows.Forms.Label toxIdStaticText2;
        private System.Windows.Forms.Button copyToClipboardButton;
        private System.Windows.Forms.Label copiedToClipboardLabel;
    }
}