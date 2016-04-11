namespace Toxofone.Devices
{
    public sealed class DeviceManager
    {
        private static DeviceManager instance;

        private DeviceManager()
        {
        }

        public static DeviceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DeviceManager();
                }

                return instance;
            }
        }

        public DeviceInfo RecordingDevice { get; set; }

        public DeviceInfo PlaybackDevice { get; set; }

        public DeviceInfo RingingDevice { get; set; }

        public DeviceInfo VideoDevice { get; set; }
    }
}
