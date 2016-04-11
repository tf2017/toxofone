namespace Toxofone.UI.PhoneBook
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using SharpTox.Core;
    using Toxofone.Managers;

    public partial class FriendPhoneBookEntryControl : UserControl
    {
        private static readonly DateTime ZeroDateTime = new DateTime(0L, DateTimeKind.Local);

        private int friendNumber;
        private string friendName;
        private string friendStatusMessage;
        private string latestFriendMessage;
        private string latestFriendAction;
        private DateTime latestFriendActivityDate;
        private bool friendTypingStatus;
        private CallState callState;

        public FriendPhoneBookEntryControl()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public delegate void FriendInfoDoubleClicked(int friendNumber);

        public event FriendInfoDoubleClicked OnFriendInfoDoubleClicked;

        public delegate void StartNewCall(int friendNumber);

        public event StartNewCall OnStartNewCall;

        public delegate void EndIncomingCall(int friendNumber);

        public event EndIncomingCall OnEndIncomingCall;

        public delegate void RemoveEntry(int friendNumber);

        public event RemoveEntry OnRemoveEntry;

        public int FriendNumber
        {
            get { return this.friendNumber; }
            set { this.friendNumber = value; }
        }

        public ToxConnectionStatus FriendConnectionStatus
        {
            get { return this.displayStatus.ConnectionStatus; }
            set
            {
                this.displayStatus.ConnectionStatus = value;
                this.UpdateControl();
            }
        }

        public ToxUserStatus FriendStatus
        {
            get { return this.displayStatus.UserStatus; }
            set
            {
                this.displayStatus.UserStatus = value;
                this.UpdateControl();
            }
        }

        public string FriendName
        {
            get { return this.friendName; }
            set
            {
                this.friendName = value;
                this.UpdateControl();
            }
        }

        public string FriendStatusMessage
        {
            get { return this.friendStatusMessage; }
            set
            {
                this.friendStatusMessage = value;
                this.UpdateControl();
            }
        }

        public bool FriendTypingStatus
        {
            get { return this.friendTypingStatus; }
            set { this.friendTypingStatus = value; }
        }

        public string FriendMessage
        {
            get { return this.latestFriendMessage; }
            set
            {
                this.latestFriendMessage = value;
                this.latestFriendAction = null;
                this.latestFriendActivityDate = (!string.IsNullOrEmpty(this.latestFriendMessage)) ? DateTime.Now : ZeroDateTime;
                this.UpdateControl();
            }
        }

        public string FriendAction
        {
            get { return this.latestFriendAction; }
            set { this.latestFriendAction = value; }
        }

        public CallState CallState
        {
            get { return this.callState; }
            set
            {
                this.callState = value;
                this.UpdateControl();
            }
        }

        private void UpdateComponent()
        {
            this.friendNumber = -1;
            this.friendName = string.Empty;
            this.friendStatusMessage = string.Empty;
            this.latestFriendMessage = null;
            this.latestFriendAction = null;
            this.latestFriendActivityDate = ZeroDateTime;
            this.friendTypingStatus = false;
            this.callState = CallState.None;
            this.displayName.DoubleClick += this.OnFriendInfoDoubleClick;
            this.displayStatusMessage.DoubleClick += this.OnFriendInfoDoubleClick;
            this.startCall.PictureClick += this.OnStartCallClick;
            this.endCall.PictureClick += this.OnEndCallClick;
            this.removeEntry.PictureClick += this.OnRemoveEntryClick;
            this.UpdateControl();
        }

        private void OnFriendInfoDoubleClick(object sender, EventArgs e)
        {
            FriendInfoDoubleClicked handler = this.OnFriendInfoDoubleClicked;
            if (handler != null)
            {
                handler(this.friendNumber);
            }
        }

        private void OnStartCallClick(object sender, EventArgs e)
        {
            StartNewCall handler = this.OnStartNewCall;
            if (handler != null)
            {
                handler(this.friendNumber);
            }
        }

        private void OnEndCallClick(object sender, EventArgs e)
        {
            EndIncomingCall handler = this.OnEndIncomingCall;
            if (handler != null)
            {
                handler(this.friendNumber);
            }
        }

        private void OnRemoveEntryClick(object sender, EventArgs e)
        {
            RemoveEntry handler = this.OnRemoveEntry;
            if (handler != null)
            {
                handler(this.friendNumber);
            }
        }

        private void UpdateControl()
        {
            if ((this.callState & CallState.None) != 0)
            {
                this.BackColor = Color.Transparent;
                this.displayStatus.Notification = false;
                this.displayName.Text = this.friendName;
                this.displayStatusMessage.Text = this.PrepareDisplayStatusMessage();
                this.endCall.Visible = false;
                this.endCall.Enabled = false;
                this.startCall.Enabled = (this.FriendConnectionStatus != ToxConnectionStatus.None);
                this.removeEntry.Enabled = true;
            }
            else if ((this.callState & CallState.IncomingCall) != 0)
            {
                this.BackColor = Color.MistyRose;
                this.displayStatus.Notification = true;
                this.displayName.Text = string.Format("{0} calling ...", this.friendName);
                this.displayStatusMessage.Text = this.PrepareDisplayStatusMessage();
                this.endCall.Enabled = true;
                this.endCall.Visible = true;
                this.startCall.Enabled = true;
                this.removeEntry.Enabled = false;
            }
            else if ((this.callState & CallState.OutgoingCall) != 0)
            {
                this.BackColor = Color.MistyRose;
                this.displayStatus.Notification = true;
                this.displayName.Text = string.Format("Calling to {0} ...", this.friendName);
                this.displayStatusMessage.Text = this.PrepareDisplayStatusMessage();
                this.endCall.Enabled = true;
                this.endCall.Visible = true;
                this.startCall.Enabled = true;
                this.removeEntry.Enabled = false;
            }
            else if ((this.callState & CallState.CallInProgress) != 0)
            {
                this.BackColor = Color.MistyRose;
                this.displayStatus.Notification = false;
                this.displayName.Text = string.Format("Call to {0} in progress ...", this.friendName);
                this.displayStatusMessage.Text = this.PrepareDisplayStatusMessage();
                this.endCall.Enabled = true;
                this.endCall.Visible = true;
                this.startCall.Enabled = true;
                this.removeEntry.Enabled = false;
            }
        }

        private string PrepareDisplayStatusMessage()
        {
            if (!string.IsNullOrEmpty(this.latestFriendMessage))
            {
                return string.Format("[{0}]: {1}", this.latestFriendActivityDate.ToLongTimeString(), this.latestFriendMessage);
            }

            return this.friendStatusMessage;
        }
    }
}
