namespace Toxofone.Devices
{
    using System;

    public sealed class DeviceInfo
    {
        public DeviceInfo(int number, string name)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException("number", number, "Value should be non-negative");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            this.Number = number;
            this.Name = name;
        }

        public int Number { get; private set; }

        public string Name { get; private set; }
    }
}
