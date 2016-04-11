namespace Toxofone.Managers
{
    using System;
    using System.Linq;

    public sealed class ProfileInfo
    {
        public const string DotExt = ".tox";

        public ProfileInfo(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            this.Path = path;
            this.Name = path.Split(new[] { "\\" }, StringSplitOptions.None).Last().Replace(DotExt, "");
            this.FileName = this.Name + DotExt;
        }

        public string Name { get; private set; }
        public string Path { get; private set; }
        public string FileName { get; private set; }
    }
}
