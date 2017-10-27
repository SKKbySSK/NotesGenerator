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
                    default:
                        Instance.Rank = arg;
                        break;
                }
            }


            const string ID_Info = "INFO";
            const string ID_Notes = "NOTES";
            const string ID_Song = "SONG";
            var Music = SugaEngine.Export.Notes.Deserialize(@"C: \Users\Kaisei Sunaga\Source\Repos\NotesGenerator\NotesPlayer\bin\Debug\Fumen\BeforeTheLive\Before The Live_Normal.sgsong");
            Music.Title = "Before The Live";
            Music.Song = "/Before The Live.wav";

            using (FileStream Output = new FileStream(@"C: \Users\Kaisei Sunaga\Source\Repos\NotesGenerator\NotesPlayer\bin\Debug\Fumen\BeforeTheLive\Before The Live_Normal.sgsong", FileMode.Create, FileAccess.Write))
            {
                BinaryWriter bw = new BinaryWriter(Output);

                bw.Write(ID_Info);
                bw.Write(Music.Title);
                bw.Write(Music.BPM);
                bw.Write(Music.Delay);

                bw.Write(ID_Notes);
                bw.Write(Music.Notes.Count);
                foreach (SugaEngine.Note note in Music.Notes)
                {
                    bw.Write((byte)note.Mode);
                    bw.Write(note.StartingTime.TotalMilliseconds);
                    bw.Write(note.EndingTime.TotalMilliseconds);
                    bw.Write(note.Lane);
                }

                bw.Write(ID_Song);
                bw.Write(Music.Song);
            }
        }
    }
}
