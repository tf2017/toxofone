namespace Toxofone.UI.UserInfo
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using SharpTox.Core;

    public partial class ToxUserStatusControl : UserControl
    {
        private ToxConnectionStatus connectionStatus;
        private ToxUserStatus userStatus;
        private bool notification;
        private Color mouseOverBackColor;
        private Color mouseOutBackColor;
        private Timer notificationTimer;
        private int notificationCounter;

        public ToxUserStatusControl()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public event EventHandler PictureClick;

        public ToxConnectionStatus ConnectionStatus
        {
            get { return this.connectionStatus; }
            set
            {
                this.connectionStatus = value;
                this.UpdateControl();
            }
        }

        public ToxUserStatus UserStatus
        {
            get { return this.userStatus; }
            set
            {
                this.userStatus = value;
                this.UpdateControl();
            }
        }

        public bool Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value;
                this.notificationCounter = 0;
                this.UpdateControl();

                if (this.notification)
                {
                    this.notificationTimer.Enabled = true;
                    this.notificationTimer.Start();
                }
                else
                {
                    this.notificationTimer.Stop();
                    this.notificationTimer.Enabled = false;
                }
            }
        }

        public Color MouseOverBackColor
        {
            get { return this.mouseOverBackColor; }
            set
            {
                this.mouseOverBackColor = value;
                this.UpdateControl();
            }
        }

        private void UpdateComponent()
        {
            this.connectionStatus = ToxConnectionStatus.None;
            this.userStatus = ToxUserStatus.None;
            this.notification = false;
            this.mouseOverBackColor = Color.Transparent;
            this.mouseOutBackColor = this.pictureBox.BackColor;
            this.pictureBox.Layout += this.OnPictureBoxLayout;
            this.pictureBox.Click += this.OnPictureBoxClick;
            this.pictureBox.MouseEnter += this.OnPictureBoxMouseEnter;
            this.pictureBox.MouseLeave += this.OnPictureBoxMouseLeave;
            this.notificationTimer = new Timer();
            this.notificationTimer.Enabled = false;
            this.notificationTimer.Interval = 250;
            this.notificationTimer.Tick += this.OnNotificationTimerTick;
            this.notificationCounter = 0;
            this.UpdateControl();
        }

        private void OnPictureBoxLayout(object sender, LayoutEventArgs e)
        {
            this.UpdateControl();
        }

        private void OnPictureBoxClick(object sender, EventArgs e)
        {
            EventHandler handler = this.PictureClick;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnPictureBoxMouseEnter(object sender, EventArgs e)
        {
            EventHandler handler = this.PictureClick;
            if (handler != null)
            {
                if (this.mouseOverBackColor != this.pictureBox.BackColor)
                {
                    this.mouseOutBackColor = this.pictureBox.BackColor;
                    this.pictureBox.BackColor = this.mouseOverBackColor;
                }
            }
        }

        private void OnPictureBoxMouseLeave(object sender, EventArgs e)
        {
            if (this.pictureBox.BackColor != this.mouseOutBackColor)
            {
                this.pictureBox.BackColor = this.mouseOutBackColor;
            }
        }

        private void OnNotificationTimerTick(object sender, EventArgs e)
        {
            this.notificationTimer.Stop();
            this.notificationTimer.Enabled = false;

            if (this.notification)
            {
                this.notificationCounter++;
                this.UpdateControl();
                this.notificationTimer.Enabled = true;
                this.notificationTimer.Start();
            }
            else
            {
                this.notificationCounter = 0;
                this.UpdateControl();
            }
        }

        private void UpdateControl()
        {
            string svgResName = "tox_user_status_invisible_20px";

            if (this.connectionStatus == ToxConnectionStatus.None)
            {
                svgResName = "tox_user_status_offline_20px";
            }
            else
            {
                switch (this.userStatus)
                {
                    case ToxUserStatus.None:
                        svgResName = (this.notificationCounter % 2) == 1 ? "tox_user_status_online_notification_20px" : "tox_user_status_online_20px";
                        break;
                    case ToxUserStatus.Busy:
                        svgResName = (this.notificationCounter % 2) == 1 ? "tox_user_status_busy_notification_20px" : "tox_user_status_busy_20px";
                        break;
                    case ToxUserStatus.Away:
                        svgResName = (this.notificationCounter % 2) == 1 ? "tox_user_status_away_notification_20px" : "tox_user_status_away_20px";
                        break;
                    default:
                        break;
                }
            }

            UIUtils.DrawWithSvg(this.pictureBox, string.Format("res://Toxofone.Resources.Svg.{0}.svg", svgResName));
        }
    }
}
