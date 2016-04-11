namespace Toxofone.UI.MediaControl
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public enum MediaDeviceStatus
    {
        None,
        On,
        Off
    }

    public partial class MediaDeviceStatusControl : UserControl
    {
        private MediaDeviceStatus deviceStatus;
        private string deviceNoneSvgLocator;
        private string deviceOnSvgLocator;
        private string deviceOffSvgLocator;
        private Color mouseOverBackColor;
        private Color mouseOutBackColor;

        public MediaDeviceStatusControl()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public event EventHandler PictureClick;

        public MediaDeviceStatus DeviceStatus
        {
            get { return this.deviceStatus; }
            set
            {
                this.deviceStatus = value;
                this.UpdateControl();
            }
        }

        public string DeviceNoneSvgLocator
        {
            get { return this.deviceNoneSvgLocator; }
            set
            {
                this.deviceNoneSvgLocator = value;
                this.UpdateControl();
            }
        }

        public string DeviceOnSvgLocator
        {
            get { return this.deviceOnSvgLocator; }
            set
            {
                this.deviceOnSvgLocator = value;
                this.UpdateControl();
            }
        }

        public string DeviceOffSvgLocator
        {
            get { return this.deviceOffSvgLocator; }
            set
            {
                this.deviceOffSvgLocator = value;
                this.UpdateControl();
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
            this.deviceStatus = MediaDeviceStatus.None;
            this.mouseOverBackColor = Color.Transparent;
            this.mouseOutBackColor = this.pictureBox.BackColor;
            this.pictureBox.Layout += this.OnPictureBoxLayout;
            this.pictureBox.Click += this.OnPictureBoxClick;
            this.pictureBox.MouseEnter += this.OnPictureBoxMouseEnter;
            this.pictureBox.MouseLeave += this.OnPictureBoxMouseLeave;
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

        private void UpdateControl()
        {
            this.Enabled = this.deviceStatus != MediaDeviceStatus.None;

            string svgLocator = null;
            switch (this.deviceStatus)
            {
                case MediaDeviceStatus.None:
                    svgLocator = this.deviceNoneSvgLocator;
                    break;
                case MediaDeviceStatus.On:
                    svgLocator = this.deviceOnSvgLocator;
                    break;
                case MediaDeviceStatus.Off:
                    svgLocator = this.deviceOffSvgLocator;
                    break;
                default:
                    break;
            }
            UIUtils.DrawWithSvg(this.pictureBox, svgLocator);
        }
    }
}
