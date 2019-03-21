using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KDF.Features
{
    public class Data
    {
        public static void SaveConfig(Dictionary<string, string> Config)
        {
            string ProfileString = string.Empty;
            foreach (KeyValuePair<string, string> kvp in Config)
            {
                ProfileString += kvp.Key + "\t" + kvp.Value + "\a";
            }
            ProfileString.TrimEnd('\a');
            string Encrypted = Tools.Encryption.EncryptString(ProfileString);
            if (!Directory.Exists("Profiles"))
                Directory.CreateDirectory("Profiles");
            File.WriteAllText("Profiles\\" + Config["Username"] + ".config", Encrypted);
        }

        public static Dictionary<string, string> LoadConfig(string Username)
        {
            Dictionary<string, string> returns = new Dictionary<string, string>();
            if (Directory.Exists("Profiles") && File.Exists("Profiles\\" + Username + ".config"))
            {
                string profile = File.ReadAllText("Profiles\\" + Username + ".config");
                profile = Tools.Encryption.DecryptString(profile);
                string[] a = profile.Split('\a');
                foreach (string b in a)
                {
                    string[] x = b.Split('\t');
                    if (x.Length == 2)
                        returns.Add(x[0], x[1]);
                }
            }
            return returns;
        }
    }
}
