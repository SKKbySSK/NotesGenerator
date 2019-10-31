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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SugaEngine;

namespace NotesPlayer.Controls
{
    public class SelectedEventArgs : EventArgs
    {
        public SelectedEventArgs(MusicHolder music)
        {
            Music = music;
        }

        public MusicHolder Music { get; set; }
    }

    /// <summary>
    /// MusicSelectionView.xaml の相互作用ロジック
    /// </summary>
    public partial class MusicSelectionView : UserControl
    {
        public event EventHandler<SelectedEventArgs> MusicSelected;
        MusicHolder current;
        AudioPlayer player;

        public MusicSelectionView()
        {
            InitializeComponent();

            string directory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            System.IO.Directory.CreateDirectory(directory);

            foreach(var musicDir in System.IO.Directory.EnumerateDirectories(directory))
            {
                string musicName = System.IO.Path.GetFileName(musicDir);
                string easy = System.IO.Path.Combine(musicDir, "easy.sgsong");
                string normal = System.IO.Path.Combine(musicDir, "normal.sgsong");
                string hard = System.IO.Path.Combine(musicDir, "hard.sgsong");

                try
                {
                    var cell = new MusicCell()
                    {
                        Music = new MusicHolder(easy, normal, hard)
                    };
                    cell.MouseLeftButtonDown += Cell_MouseLeftButtonDown;
                    songsParent.Children.Add(cell);
                }
                catch(System.IO.FileNotFoundException ex)
                {
                    var name = System.IO.Path.GetFileName(ex.FileName);
                    MessageBox.Show($"次のファイルが見つかりませんでした。\n{name}\n以下の楽曲データは無視されます\n{musicName}\n\n\n" +
                        $"フォルダ構造を確認してください\nData/{musicName}/\n" + 
                        $"\teasy.sgsong\n\tnormal.sgsong\n\thard.sgsong\n\t{musicName}.任意の音楽拡張子");
                }
            }
        }

        private void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = (MusicCell)sender;
            cell.Background = (Brush)Resources["SelectedBrush"];
            cell.Background = (Brush)Resources["DefaultBrush"];
            current = cell.Music;

            InitPlayer(current.MusicFile);
            player.Play(TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(10000));
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (current != null)
            {
                if (player != null)
                    player.Dispose();

                MusicSelected?.Invoke(this, new SelectedEventArgs(current));
            }
        }

        void InitPlayer(string Path)
        {
            if (player != null)
                player.Dispose();
            player = new AudioPlayer(Path);
        }
    }
}
