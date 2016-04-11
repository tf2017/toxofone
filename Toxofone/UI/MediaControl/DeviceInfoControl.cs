namespace Toxofone.UI.MediaControl
{
    using System;
    using System.Windows.Forms;

    public partial class DeviceInfoControl : UserControl
    {
        private string deviceType;

        public DeviceInfoControl()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public delegate void SelectedDeviceChanged(int deviceNumber);

        public event SelectedDeviceChanged OnSelectedDeviceChanged;

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

        public string DeviceType
        {
            get { return this.deviceType; }
            set { this.deviceType = value; }
        }

        public void Add(string deviceName)
        {
            this.deviceName.Items.Add(deviceName);
        }

        public string FindDeviceName(int deviceIndex)
        {
            if (deviceIndex < 0 || deviceIndex >= this.deviceName.Items.Count)
            {
                return null;
            }

            return this.deviceName.Items[deviceIndex].ToString();
        }

        private void UpdateComponent()
        {
            this.deviceName.SelectedIndexChanged += this.OnSelectedIndexChanged;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedDeviceChanged handler = this.OnSelectedDeviceChanged;
            if (handler != null)
            {
                handler(this.deviceName.SelectedIndex);
            }
        }
    }
}
