namespace Toxofone.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using SharpTox.Core;

    public sealed class Config
    {
        private const string ConfigFileName = "config.ini";

        private static readonly object syncLock = new object();

        private static Config instance;

        private Config()
        {
            this.AutoConnectNames = new List<string>();
        }

        public static Config Instance
        {
            get
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new Config();
                    }

                    return instance;
                }
            }
        }

        public bool EnableUdp { get; set; } = true;
        public bool EnableIpv6 { get; set; } = true;

        public int? StartupLogLevel { get; set; }
        public int? StartupLayout { get; set; }
        public int? RecordingDeviceNumber { get; set; }
        public int? PlaybackDeviceNumber { get; set; }
        public int? RingingDeviceNumber { get; set; }
        public string VideoDeviceName { get; set; }
        public IList<string> AutoConnectNames { get; private set; }

        public ToxNameService[] NameServices { get; } = new ToxNameService[]
        {
            new ToxNameService { Domain = "utox.org", PublicKey = "D3154F65D28A5B41A05D4AC7E4B39C6B1C233CC857FB365C56E8392737462A12" },
            new ToxNameService { Domain = "toxme.io", PublicKey = "1A39E7A5D5FA9CF155C751570A32E625698A60A55F6D88028F949F66144F4F25" }
        };

        public ToxConfigNode[] Nodes { get; } = new ToxConfigNode[]
        {
            new ToxConfigNode()
            {
                Address = "144.76.60.215",
                Port = 33445,
                PublicKey = "04119E835DF3E78BACF0F84235B300546AF8B936F035185E2A8E9E0A67C8924F"
            },

            new ToxConfigNode()
            {
                Address = "23.226.230.47",
                Port = 33445,
                PublicKey = "A09162D68618E742FFBCA1C2C70385E6679604B2D80EA6E84AD0996A1AC8A074"
            },

            new ToxConfigNode()
            {
                Address = "178.62.250.138",
                Port = 33445,
                PublicKey = "788236D34978D1D5BD822F0A5BEBD2C53C64CC31CD3149350EE27D4D9A2F9B6B"
            },

            new ToxConfigNode()
            {
                Address = "130.133.110.14",
                Port = 33445,
                PublicKey = "461FA3776EF0FA655F1A05477DF1B3B614F7D6B124F7DB1DD4FE3C08B03B640F"
            },

            new ToxConfigNode()
            {
                Address = "205.185.116.116",
                Port = 33445,
                PublicKey = "A179B09749AC826FF01F37A9613F6B57118AE014D4196A0E1105A98F93A54702"
            },
             
            new ToxConfigNode()
            {
                Address = "198.98.51.198",
                Port = 33445,
                PublicKey = "1D5A5F2F5D6233058BF0259B09622FB40B482E4FA0931EB8FD3AB8E7BF7DAF6F"
            },

            new ToxConfigNode()
            {
                Address = "108.61.165.198",
                Port = 33445,
                PublicKey = "8E7D0B859922EF569298B4D261A8CCB5FEA14FB91ED412A7603A585A25698832"
            },

            new ToxConfigNode()
            {
                Address = "194.249.212.109",
                Port = 33445,
                PublicKey = "3CEE1F054081E7A011234883BC4FC39F661A55B73637A5AC293DDF1251D9432B"
            },

            new ToxConfigNode()
            {
                Address = "185.25.116.107",
                Port = 443,
                PublicKey = "DA4E4ED4B697F2E9B000EEFE3A34B554ACD3F45F5C96EAEA2516DD7FF9AF7B43"
            },

            new ToxConfigNode()
            {
                Address = "192.99.168.140",
                Port = 33445,
                PublicKey = "6A4D0607A296838434A6A7DDF99F50EF9D60A2C510BBF31FE538A25CB6B4652F"
            },

            new ToxConfigNode()
            {
                Address = "95.215.46.114",
                Port = 33445,
                PublicKey = "5823FB947FF24CF83DDFAC3F3BAA18F96EA2018B16CC08429CB97FA502F40C23"
            },

            new ToxConfigNode()
            {
                Address = "5.189.176.217",
                Port = 33445,
                PublicKey = "2B2137E094F743AC8BD44652C55F41DFACC502F125E99E4FE24D40537489E32F"
            },

            new ToxConfigNode()
            {
                Address = "148.251.23.146",
                Port = 33445,
                PublicKey = "7AED21F94D82B05774F697B209628CD5A9AD17E0C073D9329076A4C28ED28147"
            },

            new ToxConfigNode()
            {
                Address = "104.223.122.15",
                Port = 33445,
                PublicKey = "0FB96EEBFB1650DDB52E70CF773DDFCABE25A95CC3BB50FC251082E4B63EF82A"
            },

            new ToxConfigNode()
            {
                Address = "78.47.114.252",
                Port = 33445,
                PublicKey = "1C5293AEF2114717547B39DA8EA6F1E331E5E358B35F9B6B5F19317911C5F976"
            },

            new ToxConfigNode()
            {
                Address = "95.31.20.151",
                Port = 33445,
                PublicKey = "9CA69BB74DE7C056D1CC6B16AB8A0A38725C0349D187D8996766958584D39340"
            },

            new ToxConfigNode()
            {
                Address = "51.254.84.212",
                Port = 33445,
                PublicKey = "AEC204B9A4501412D5F0BB67D9C81B5DB3EE6ADA64122D32A3E9B093D544327D"
            },

            new ToxConfigNode()
            {
                Address = "5.135.59.163",
                Port = 33445,
                PublicKey = "2D320F971EF2CA18004416C2AAE7BA52BF7949DB34EA8E2E21AF67BD367BE211"
            },

            new ToxConfigNode()
            {
                Address = "185.58.206.164",
                Port = 33445,
                PublicKey = "24156472041E5F220D1FA11D9DF32F7AD697D59845701CDD7BE7D1785EB9DB39"
            },

            new ToxConfigNode()
            {
                Address = "82.211.31.116",
                Port = 33445,
                PublicKey = "AF97B76392A6474AF2FD269220FDCF4127D86A42EF3A242DF53A40A268A2CD7C"
            },

            new ToxConfigNode()
            {
                Address = "128.199.199.197",
                Port = 33445,
                PublicKey = "B05C8869DBB4EDDD308F43C1A974A20A725A36EACCA123862FDE9945BF9D3E09"
            },

            new ToxConfigNode()
            {
                Address = "103.230.156.174",
                Port = 33445,
                PublicKey = "5C4C7A60183D668E5BD8B3780D1288203E2F1BAE4EEF03278019E21F86174C1D"
            },

            new ToxConfigNode()
            {
                Address = "91.121.66.124",
                Port = 33445,
                PublicKey = "4E3F7D37295664BBD0741B6DBCB6431D6CD77FC4105338C2FC31567BF5C8224A"
            },

            new ToxConfigNode()
            {
                Address = "92.54.84.70",
                Port = 443,
                PublicKey = "5625A62618CB4FCA70E147A71B29695F38CC65FF0CBD68AD46254585BE564802"
            },

            new ToxConfigNode()
            {
                Address = "195.93.190.6",
                Port = 33445,
                PublicKey = "FB4CE0DDEFEED45F26917053E5D24BDDA0FA0A3D83A672A9DA2375928B37023D"
            }
        };

        public void Reload()
        {
            try
            {
                this.ClearAll();

                string configFilePath = Path.Combine(ProfileManager.ProfileDataPath, ConfigFileName);

                if (File.Exists(configFilePath))
                {
                    using (FileStream fs = new FileStream(configFilePath, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            this.ReadFrom(sr);
                        }
                    }

                    Logger.Log(LogLevel.Info, "Reloaded config from file");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Could not reload config: " + ex.ToString());
            }
        }

        public void Save()
        {
            try
            {
                if (!Directory.Exists(ProfileManager.ProfileDataPath))
                {
                    Directory.CreateDirectory(ProfileManager.ProfileDataPath);
                }

                string configFilePath = Path.Combine(ProfileManager.ProfileDataPath, ConfigFileName);

                using (FileStream fs = new FileStream(configFilePath, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        this.WriteTo(sw);
                    }
                }

                FileInfo fi = new FileInfo(configFilePath);
                if (fi.Exists && fi.Length == 0)
                {
                    // do not keep empty config file
                    fi.Delete();
                    return;
                }

                Logger.Log(LogLevel.Info, "Saved config to file");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Could not save config: " + ex.ToString());
            }
        }

        private void ClearAll()
        {
            this.StartupLogLevel = null;
            this.StartupLayout = null;
            this.RecordingDeviceNumber = null;
            this.PlaybackDeviceNumber = null;
            this.RingingDeviceNumber = null;
            this.VideoDeviceName = null;
            this.AutoConnectNames.Clear();
        }

        private void ReadFrom(StreamReader sr)
        {
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                int nameValueDelimPos = line.IndexOf('=');
                if (nameValueDelimPos < 0)
                {
                    // invalid 'name = value' string format, ignored
                    continue;
                }

                string name = line.Substring(0, nameValueDelimPos).Trim();
                string valueStr = line.Substring(nameValueDelimPos + 1).Trim();

                if (name.Length == 0)
                {
                    // no name supplied, ignored
                    continue;
                }

                if (string.Compare(name, "startup.loglevel", true) == 0)
                {
                    int valueInt = 0;
                    if (int.TryParse(valueStr, out valueInt))
                    {
                        this.StartupLogLevel = valueInt;
                    }
                }
                else if (string.Compare(name, "startup.layout", true) == 0)
                {
                    int valueInt = 0;
                    if (int.TryParse(valueStr, out valueInt))
                    {
                        this.StartupLayout = valueInt;
                    }
                }
                else if (string.Compare(name, "recording.device", true) == 0)
                {
                    int valueInt = 0;
                    if (int.TryParse(valueStr, out valueInt))
                    {
                        this.RecordingDeviceNumber = valueInt;
                    }
                }
                else if (string.Compare(name, "playback.device", true) == 0)
                {
                    int valueInt = 0;
                    if (int.TryParse(valueStr, out valueInt))
                    {
                        this.PlaybackDeviceNumber = valueInt;
                    }
                }
                else if (string.Compare(name, "ringing.device", true) == 0)
                {
                    int valueInt = 0;
                    if (int.TryParse(valueStr, out valueInt))
                    {
                        this.RingingDeviceNumber = valueInt;
                    }
                }
                else if (string.Compare(name, "video.device", true) == 0)
                {
                    this.VideoDeviceName = valueStr;
                }
                else if (string.Compare(name, "autoconnect.name", true) == 0)
                {
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        this.AutoConnectNames.Add(valueStr);
                    }
                }
            }
        }

        private void WriteTo(StreamWriter sw)
        {
            if (this.StartupLogLevel.HasValue)
            {
                sw.WriteLine(string.Format("startup.loglevel = {0}", this.StartupLogLevel.Value));
            }
            if (this.StartupLayout.HasValue)
            {
                sw.WriteLine(string.Format("startup.layout = {0}", this.StartupLayout.Value));
            }
            if (this.RecordingDeviceNumber.HasValue)
            {
                sw.WriteLine(string.Format("recording.device = {0}", this.RecordingDeviceNumber.Value));
            }
            if (this.PlaybackDeviceNumber.HasValue)
            {
                sw.WriteLine(string.Format("playback.device = {0}", this.PlaybackDeviceNumber.Value));
            }
            if (this.RingingDeviceNumber.HasValue)
            {
                sw.WriteLine(string.Format("ringing.device = {0}", this.RingingDeviceNumber.Value));
            }
            if (!string.IsNullOrEmpty(this.VideoDeviceName))
            {
                sw.WriteLine(string.Format("video.device = {0}", this.VideoDeviceName));
            }
            if (this.AutoConnectNames.Count > 0)
            {
                foreach (string name in this.AutoConnectNames)
                {
                    sw.WriteLine(string.Format("autoconnect.name = {0}", name));
                }
            }
        }
    }

    public class ToxNameService
    {
        public string Domain { get; set; }

        public string PublicKey { get; set; }
    }

    public class ToxConfigNode
    {
        public string PublicKey { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }
    }
}
