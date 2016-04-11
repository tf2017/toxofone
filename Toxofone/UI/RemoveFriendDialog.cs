namespace Toxofone.UI
{
    using System;
    using System.Windows.Forms;

    public partial class RemoveFriendDialog : Form
    {
        public RemoveFriendDialog()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public string Message
        {
            get { return this.richTextBox.Text; }
            set { this.richTextBox.Text = value; }
        }

        public string RtfMessage
        {
            get { return this.richTextBox.Rtf; }
            set { this.richTextBox.Rtf = value; }
        }

        private void UpdateComponent()
        {
            this.Load += this.OnDialogLoad;
            this.richTextBox.GotFocus += this.OnRichTextBoxGotFocus;
        }

        private void OnDialogLoad(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                this.cancelButton.Focus();
            }));
        }

        private void OnRichTextBoxGotFocus(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                this.okButton.Focus();
            }));
        }
    }
}
