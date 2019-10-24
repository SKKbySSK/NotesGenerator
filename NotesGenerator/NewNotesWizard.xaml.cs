using Microsoft.Win32;
using SugaEngine;
using SugaEngine.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NotesGenerator
{
    /// <summary>
    /// NewNotesWizard.xaml の相互作用ロジック
    /// </summary>
    public partial class NewNotesWizard : Window
    {
        public NewNotesWizard()
        {
            InitializeComponent();
        }

        private void newFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "音楽ファイル|*.mp3;*.wav;*.flac;*.m4a|全てのファイル|*.*";
            ofd.FileName = "";
            if (ofd.ShowDialog() ?? false)
            {
                var window = new MainWindow();
                if (window.SetSong(ofd.FileName))
                {
                    window.Show();
                    Close();
                }
            }
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Suga Songファイル|*.sgsong";
            ofd.FileName = "";
            if (ofd.ShowDialog() ?? false)
            {
                Music music = Notes.Deserialize(ofd.FileName);
                string audio = System.IO.Path.GetDirectoryName(ofd.FileName) + music.Song;

                var window = new MainWindow();
                if (window.SetSong(audio))
                {
                    foreach (Note note in music.Notes)
                        window.TempNotes.Add(note);
                    window.Show();
                    Close();
                }
            }
        }
    }
}
