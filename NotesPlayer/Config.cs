using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace NotesPlayer
{
    class Config
    {
        static Config()
        {
            using (var sr = new StreamReader("config.json"))
            {
                Shared = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
            }
        }

        public static Config Shared { get; }

        public string BackgdroundImage { get; set; } = "bg_main.png";

        public string SplashImage { get; set; } = "bg_logo.png";


    }
}
