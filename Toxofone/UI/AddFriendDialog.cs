namespace Toxofone.UI
{
    using System;
    using System.Windows.Forms;

    public partial class AddFriendDialog : Form
    {
        public AddFriendDialog()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public string ToxId
        {
            get { return this.toxIdTextBox.Text; }
            set { this.toxIdTextBox.Text = value; }
        }

        public string Message
        {
            get { return this.messageTextBox.Text; }
            set { this.messageTextBox.Text = value; }
        }

        private void UpdateComponent()
        {
            this.Load += this.OnDialogLoad;
            this.toxIdTextBox.TextChanged += this.OnToxIdChanged;
            this.sendRequestButton.Enabled = false;
        }

        private void OnDialogLoad(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.ToxId))
                {
                    if (Clipboard.ContainsText(TextDataFormat.Text))
                    {
                        string text = Clipboard.GetText(TextDataFormat.Text).Trim();
                        if (IsValidToxId(text))
                        {
                            this.ToxId = text;
                        }
                    }
                    else if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                    {
                        string unicodeText = Clipboard.GetText(TextDataFormat.UnicodeText).Trim();
                        if (IsValidToxId(unicodeText))
                        {
                            this.ToxId = unicodeText;
                        }
                    }
                }
            }
            catch { }

            this.BeginInvoke(new MethodInvoker(() =>
            {
                this.cancelButton.Focus();
            }));
        }

        private void OnToxIdChanged(object sender, EventArgs e)
        {
            this.sendRequestButton.Enabled = !string.IsNullOrEmpty(this.ToxId);
        }

        private static bool IsValidToxId(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            if (str.Length != 76)
            {
                return false;
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (!IsHexDigit(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsHexDigit(char ch)
        {
            return ("0123456789ABCDEFabcdef".IndexOf(ch) >= 0);
        }
    }
}
