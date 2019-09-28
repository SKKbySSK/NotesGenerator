using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace NotesPlayer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            App.Current.Startup += Current_Startup;
        }

        private void Current_Startup(object sender, StartupEventArgs e)
        {
            foreach(var arg in e.Args)
            {
                switch (arg.ToLower())
                {
                    case "auto":
                        Instance.FullAutomatic = true;
                        break;
                    case "skip":
                        Instance.Skip = true;
                        break;
                    case "rank":
                        Instance.OverrideRank = true;
                        break;
                    case "debug":
                        Instance.WindowStyle = WindowStyle.SingleBorderWindow;
                        break;
                    default:
                        Instance.Rank = arg;
                        break;
                }
            }
        }
    }
}
