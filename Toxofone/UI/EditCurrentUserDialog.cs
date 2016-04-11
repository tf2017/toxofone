namespace Toxofone.UI
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public partial class EditCurrentUserDialog : Form
    {
        private Color nameTextBoxBackColor;
        private string userToxId;

        public EditCurrentUserDialog()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public string UserName
        {
            get { return this.nameTextBox.Text; }
            set { this.nameTextBox.Text = value; }
        }

        public string UserStatusMessage
        {
            get { return this.statusMessageTextBox.Text; }
            set { this.statusMessageTextBox.Text = value; }
        }

        public string UserToxId
        {
            get { return this.userToxId; }
            set
            {
                this.userToxId = value;
                if (string.IsNullOrEmpty(this.userToxId))
                {
                    this.toxIdStaticText.Text = string.Empty;
                    this.toxIdStaticText2.Text = string.Empty;
                    this.copyToClipboardButton.Enabled = false;
                }
                else
                {
                    if (this.userToxId.Length < 38)
                    {
                        this.toxIdStaticText.Text = this.userToxId;
                        this.toxIdStaticText2.Text = string.Empty;
                    }
                    else
                    {
                        this.toxIdStaticText.Text = this.userToxId.Substring(0, 38);
                        this.toxIdStaticText2.Text = this.userToxId.Substring(38);
                    }
                    this.copyToClipboardButton.Enabled = true;
                }
            }
        }

        private void UpdateComponent()
        {
            this.Load += this.OnDialogLoad;
            this.nameTextBox.TextChanged += this.OnUserNameChanged;
            this.copyToClipboardButton.Click += this.OnCopyToClipboardButtonClick;
            this.nameTextBoxBackColor = this.nameTextBox.BackColor;
            this.okButton.Enabled = false;
            this.copyToClipboardButton.Enabled = false;
        }

        private void OnDialogLoad(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                this.cancelButton.Focus();
            }));
        }

        private void OnUserNameChanged(object sender, EventArgs e)
        {
            bool userNameValid = IsUserNameValid(this.UserName);
            this.okButton.Enabled = userNameValid;
            if (!userNameValid && !string.IsNullOrEmpty(this.UserName))
            {
                this.nameTextBox.BackColor = Color.MistyRose;
            }
            else
            {
                this.nameTextBox.BackColor = this.nameTextBoxBackColor;
            }
        }

        private static bool IsUserNameValid(string userName)
        {
            try
            {
                FileInfo fi = new FileInfo(userName);
                string fileName = Path.GetFileNameWithoutExtension(userName);
                return (string.CompareOrdinal(fileName, userName) == 0);
            }
            catch
            {
                return false;
            }
        }

        private void OnCopyToClipboardButtonClick(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(this.userToxId);
            }
            catch { }

            bool resultOk = false;
            try
            {
                if (string.CompareOrdinal(Clipboard.GetText(), this.userToxId) == 0)
                {
                    resultOk = true;
                }
            }
            catch { }

            if (resultOk)
            {
                this.copiedToClipboardLabel.Text = new string(new char[] { (char)0xD6 });  // Font Symbol-Bold
            }
            else
            {
                Clipboard.SetText(string.Empty);
            }
        }
    }
}
