namespace Toxofone.UI.UserInfo
{
    using System;
    using System.Windows.Forms;

    public partial class CurrentUserInfoControl : UserControl
    {
        public CurrentUserInfoControl()
        {
            InitializeComponent();
            UpdateComponent();
        }

        private void UpdateComponent()
        {
        }

        public string UserName
        {
            get { return this.userName.Text; }
            set { this.userName.Text = value; }
        }

        public string UserStatusMessage
        {
            get { return this.userStatusMessage.Text; }
            set { this.userStatusMessage.Text = value; }
        }
    }
}
