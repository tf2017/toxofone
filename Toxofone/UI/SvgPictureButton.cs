namespace Toxofone.UI
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class SvgPictureButton : UserControl
    {
        private string enabledSvgLocator;
        private string disabledSvgLocator;
        private string mouseOverSvgLocator;
        private Color mouseOverBackColor;
        private Color mouseOutBackColor;

        public SvgPictureButton()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public event EventHandler PictureClick;
            
        public string EnabledSvgLocator
        {
            get { return this.enabledSvgLocator; }
            set
            {
                this.enabledSvgLocator = value;
                this.UpdateControl();
            }
        }

        public string DisabledSvgLocator
        {
            get { return this.disabledSvgLocator; }
            set
            {
                this.disabledSvgLocator = value;
                this.UpdateControl();
            }
        }

        public string MouseOverSvgLocator
        {
            get { return this.mouseOverSvgLocator; }
            set
            {
                this.mouseOverSvgLocator = value;
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
            this.mouseOverBackColor = Color.Transparent;
            this.mouseOutBackColor = this.pictureBox.BackColor;
            this.pictureBox.Layout += this.OnPictureBoxLayout;
            this.pictureBox.Click += this.OnPictureBoxClick;
            this.pictureBox.MouseEnter += this.OnPictureBoxMouseEnter;
            this.pictureBox.MouseLeave += this.OnPictureBoxMouseLeave;
            this.UpdateControl();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.UpdateControl();
        }

        private void OnPictureBoxLayout(object sender, LayoutEventArgs e)
        {
            this.UpdateControl();
        }

        private void OnPictureBoxClick(object sender, EventArgs e)
        {
            if (!this.Enabled)
            {
                return;
            }

            EventHandler handler = this.PictureClick;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnPictureBoxMouseEnter(object sender, EventArgs e)
        {
            if (!this.Enabled)
            {
                return;
            }

            EventHandler handler = this.PictureClick;
            if (handler != null)
            {
                if (!string.IsNullOrEmpty(this.mouseOverSvgLocator))
                {
                    this.UpdateControl();
                }
                else if (this.mouseOverBackColor != this.pictureBox.BackColor)
                {
                    this.mouseOutBackColor = this.pictureBox.BackColor;
                    this.pictureBox.BackColor = this.mouseOverBackColor;
                }
            }
        }

        private void OnPictureBoxMouseLeave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.mouseOverSvgLocator))
            {
                this.UpdateControl();
            }
            else if (this.pictureBox.BackColor != this.mouseOutBackColor)
            {
                this.pictureBox.BackColor = this.mouseOutBackColor;
            }
        }

        private void UpdateControl()
        {
            string svgLocator = null;

            bool entered = this.pictureBox.ClientRectangle.Contains(this.pictureBox.PointToClient(Cursor.Position));
            if (!this.Enabled)
            {
                svgLocator = this.disabledSvgLocator;
            }
            else if (entered && !string.IsNullOrEmpty(this.mouseOverSvgLocator))
            {
                svgLocator = this.mouseOverSvgLocator;
            }

            if (string.IsNullOrEmpty(svgLocator))
            {
                svgLocator = this.enabledSvgLocator;
            }

            UIUtils.DrawWithSvg(this.pictureBox, svgLocator);
        }
    }
}
