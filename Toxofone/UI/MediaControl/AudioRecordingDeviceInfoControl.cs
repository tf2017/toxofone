namespace Toxofone.UI.MediaControl
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class AudioRecordingDeviceInfoControl : UserControl
    {
        private string deviceType;

        public AudioRecordingDeviceInfoControl()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public delegate void SelectedAudioRecordingDeviceChanged(int deviceNumber);

        public event SelectedAudioRecordingDeviceChanged OnSelectedDeviceChanged;

        public string DeviceName
        {
            get { return this.deviceName.Text; }
            set
            {
                if (string.CompareOrdinal(this.deviceName.Text, value) == 0)
                {
                    return;  // to prevent endless loop
                }

                this.deviceName.Text = value;
            }
        }

        public int DeviceVolume
        {
            get { return this.deviceVolume.Value; }
            set
            {
                value = Math.Max(value, this.deviceVolume.Minimum);
                value = Math.Min(value, this.deviceVolume.Maximum);
                this.deviceVolume.Value = value;
            }
        }

        public Color ProgressBarBackColor
        {
            get { return this.deviceVolume.BackColor; }
            set { this.deviceVolume.BackColor = (value == Color.Transparent) ? SystemColors.Control : value; }
        }

        public string DeviceType
        {
            get { return this.deviceType; }
            set { this.deviceType = value; }
        }

        public void Add(string deviceName)
        {
            this.deviceName.Items.Add(deviceName);
        }

        private void UpdateComponent()
        {
            this.deviceName.SelectedIndexChanged += this.OnSelectedIndexChanged;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedAudioRecordingDeviceChanged handler = this.OnSelectedDeviceChanged;
            if (handler != null)
            {
                handler(this.deviceName.SelectedIndex);
            }
        }
    }
}
