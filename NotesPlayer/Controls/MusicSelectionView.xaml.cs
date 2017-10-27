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
        public SelectedEventArgs(string SgSong, Music Music)
        {
            this.Music = Music;
            SgSongPath = SgSong;
        }

        public string SgSongPath { get; }
        public Music Music { get; }
    }

    /// <summary>
    /// MusicSelectionView.xaml の相互作用ロジック
    /// </summary>
    public partial class MusicSelectionView : UserControl
    {
        class MusicItem
        {
            public MusicItem(string SgFilePath)
            {
                Music = SugaEngine.Export.Notes.Deserialize(SgFilePath);
                SgSongPath = SgFilePath;
            }

            public string SgSongPath { get; }
            public Music Music { get; }
        }

        public event EventHandler<SelectedEventArgs> MusicSelected;
        MusicItem current;
        MusicItem Song1i = new MusicItem(AppDomain.CurrentDomain.BaseDirectory + @"Fumen\BeforeTheLive\Before The Live.sgsong");
        MusicItem Song2i = new MusicItem(AppDomain.CurrentDomain.BaseDirectory + @"Fumen\アリス　アイリス\アリス　アイリス.sgsong");
        AudioPlayer player;

        public MusicSelectionView()
        {
            InitializeComponent();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (current != null)
            {
                if (player != null)
                    player.Dispose();

                MusicSelected?.Invoke(this, new SelectedEventArgs(current.SgSongPath, current.Music));
            }
        }

        private void Song2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Song2.Background = (Brush)Resources["SelectedBrush"];
            Song1.Background = (Brush)Resources["DefaultBrush"];
            current = Song2i;

            InitPlayer(AppDomain.CurrentDomain.BaseDirectory + @"Fumen\アリス　アイリス\アリス　アイリス.mp3");
            player.Play(TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(10000));
        }

        private void Song1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Song1.Background = (Brush)Resources["SelectedBrush"];
            Song2.Background = (Brush)Resources["DefaultBrush"];
            current = Song1i;

            InitPlayer(AppDomain.CurrentDomain.BaseDirectory + @"Fumen\BeforeTheLive\Before The Live.wav");
            player.Play(TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(10000));
        }

        void InitPlayer(string Path)
        {
            if (player != null)
                player.Dispose();
            player = new AudioPlayer(Path);
        }
    }
}
