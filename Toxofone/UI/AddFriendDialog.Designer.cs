namespace Toxofone.UI
{
    partial class AddFriendDialog
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
            this.toxIdLabel = new System.Windows.Forms.Label();
            this.toxIdTextBox = new System.Windows.Forms.TextBox();
            this.messageLabel = new System.Windows.Forms.Label();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.buttonsPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.sendRequestButton = new System.Windows.Forms.Button();
            this.buttonsSeparator = new System.Windows.Forms.Label();
            this.buttonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toxIdLabel
            // 
            this.toxIdLabel.AutoSize = true;
            this.toxIdLabel.Location = new System.Drawing.Point(17, 9);
            this.toxIdLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toxIdLabel.Name = "toxIdLabel";
            this.toxIdLabel.Size = new System.Drawing.Size(216, 20);
            this.toxIdLabel.TabIndex = 0;
            this.toxIdLabel.Text = "Tox ID (76 hexadecimal characters)";
            this.toxIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toxIdLabel.UseCompatibleTextRendering = true;
            // 
            // toxIdTextBox
            // 
            this.toxIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toxIdTextBox.Location = new System.Drawing.Point(17, 33);
            this.toxIdTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.toxIdTextBox.Name = "toxIdTextBox";
            this.toxIdTextBox.Size = new System.Drawing.Size(599, 22);
            this.toxIdTextBox.TabIndex = 1;
            this.toxIdTextBox.WordWrap = false;
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new System.Drawing.Point(17, 68);
            this.messageLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(59, 20);
            this.messageLabel.TabIndex = 2;
            this.messageLabel.Text = "Message";
            this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.messageLabel.UseCompatibleTextRendering = true;
            // 
            // messageTextBox
            // 
            this.messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageTextBox.Location = new System.Drawing.Point(17, 94);
            this.messageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(599, 58);
            this.messageTextBox.TabIndex = 3;
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonsPanel.BackColor = System.Drawing.SystemColors.Control;
            this.buttonsPanel.Controls.Add(this.cancelButton);
            this.buttonsPanel.Controls.Add(this.sendRequestButton);
            this.buttonsPanel.Controls.Add(this.buttonsSeparator);
            this.buttonsPanel.Location = new System.Drawing.Point(0, 171);
            this.buttonsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.Size = new System.Drawing.Size(633, 57);
            this.buttonsPanel.TabIndex = 4;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(496, 15);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(120, 28);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // sendRequestButton
            // 
            this.sendRequestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sendRequestButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.sendRequestButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sendRequestButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.sendRequestButton.Location = new System.Drawing.Point(360, 15);
            this.sendRequestButton.Margin = new System.Windows.Forms.Padding(4);
            this.sendRequestButton.Name = "sendRequestButton";
            this.sendRequestButton.Size = new System.Drawing.Size(120, 28);
            this.sendRequestButton.TabIndex = 5;
            this.sendRequestButton.Text = "Send request";
            this.sendRequestButton.UseCompatibleTextRendering = true;
            this.sendRequestButton.UseVisualStyleBackColor = true;
            // 
            // buttonsSeparator
            // 
            this.buttonsSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonsSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonsSeparator.Location = new System.Drawing.Point(0, 0);
            this.buttonsSeparator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.buttonsSeparator.Name = "buttonsSeparator";
            this.buttonsSeparator.Size = new System.Drawing.Size(633, 2);
            this.buttonsSeparator.TabIndex = 4;
            // 
            // AddFriendDialog
            // 
            this.AcceptButton = this.cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(633, 228);
            this.Controls.Add(this.buttonsPanel);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.toxIdTextBox);
            this.Controls.Add(this.toxIdLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddFriendDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Add friend";
            this.buttonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label toxIdLabel;
        private System.Windows.Forms.TextBox toxIdTextBox;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.Panel buttonsPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button sendRequestButton;
        private System.Windows.Forms.Label buttonsSeparator;
    }
}