namespace Toxofone.UI.PhoneBook
{
    using System;
    using System.Windows.Forms;

    public partial class FriendRequestPhoneBookEntryControl : UserControl
    {
        public FriendRequestPhoneBookEntryControl()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public delegate void FriendRequestAccepted(FriendRequestInfo friendRequest);

        public event FriendRequestAccepted OnFriendRequestAccepted;

        public delegate void FriendRequestCanceled(FriendRequestInfo friendRequest);

        public event FriendRequestCanceled OnFriendRequestCanceled;

        public string RequestPublicKey
        {
            get { return this.requestPublicKey.Text; }
            set { this.requestPublicKey.Text = value; }
        }

        public string RequestMessage
        {
            get { return this.requestMessage.Text; }
            set { this.requestMessage.Text = value; }
        }

        private void UpdateComponent()
        {
            this.acceptFriendRequest.PictureClick += this.OnAcceptFriendRequestClick;
            this.cancelFriendRequest.PictureClick += this.OnCancelFriendRequestClick;
        }

        private void OnAcceptFriendRequestClick(object sender, EventArgs e)
        {
            FriendRequestAccepted handler = this.OnFriendRequestAccepted;
            if (handler != null)
            {
                handler(new FriendRequestInfo(this.RequestPublicKey, this.RequestMessage));
            }
        }

        private void OnCancelFriendRequestClick(object sender, EventArgs e)
        {
            FriendRequestCanceled handler = this.OnFriendRequestCanceled;
            if (handler != null)
            {
                handler(new FriendRequestInfo(this.RequestPublicKey, this.RequestMessage));
            }
        }
    }
}
