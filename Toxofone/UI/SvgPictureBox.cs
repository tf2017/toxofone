namespace Toxofone.UI
{
    using System;
    using System.Windows.Forms;

    public partial class SvgPictureBox : UserControl
    {
        private string svgLocator;

        public SvgPictureBox()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public string SvgLocator
        {
            get { return this.svgLocator; }
            set
            {
                this.svgLocator = value;
                this.UpdateControl();
            }
        }

        private void UpdateComponent()
        {
            this.pictureBox.Layout += this.OnPictureBoxLayout;
            this.UpdateControl();
        }

        private void OnPictureBoxLayout(object sender, LayoutEventArgs e)
        {
            this.UpdateControl();
        }

        private void UpdateControl()
        {
            UIUtils.DrawWithSvg(this.pictureBox, this.svgLocator);
        }
    }
}
