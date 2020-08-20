using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dailive
{
    public class Settings
    {
        public static UserData USettings = new UserData();
        public static string CachePath => Environment.ExpandEnvironmentVariables(@"%AppData%\Dailive\");
        public static async void Load() {
            if (!Directory.Exists(CachePath))
                Directory.CreateDirectory(CachePath);
            string stFile = CachePath + "\\Settings";
            if (File.Exists(stFile))
            {
                JObject obj = JObject.Parse(await File.ReadAllTextAsync(stFile));
                USettings.Weather_Place = obj["Weather_Place"].ToString();
            }
            else {
                Save();
            }
        }
        public static async void Save() {
            await File.WriteAllTextAsync(CachePath + "\\Settings", JsonConvert.SerializeObject(USettings));
        }
    }

    public class UserData{
        /// <summary>
        /// App:Weather  ip:自动定位  云县  临沧 ...other place set by user.
        /// </summary>
        public string Weather_Place = "ip";
    }
}
