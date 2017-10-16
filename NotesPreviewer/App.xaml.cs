using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NotesPreviewer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            foreach(string arg in e.Args)
            {
                switch (arg)
                {
                    default:
                        if (!System.IO.File.Exists(arg))
                        {
                            Current.Shutdown();
                            return;
                        }
                        Args.Path = arg;
                        break;
                }
            }
        }
    }
}
