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

        List<MusicItem> Songs = new List<MusicItem>(new MusicItem[]
            {
                new MusicItem(@"C:\Users\skkby\Desktop\Memories\Memories.sgsong"),
                new MusicItem(@"C:\Users\skkby\Desktop\イリュージョニスタ\イリュージョニスタ！.sgsong"),
                new MusicItem(@"C:\Users\skkby\Desktop\星織ユメミライ\1-01 星織ユメミライ.sgsong")
            });

        public MusicSelectionView()
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                const double ItemHeight = 140;
                double eh = ParentSongs.ActualHeight / ((Songs.Count * 2) - 1);
                foreach (MusicItem mi in Songs)
                {
                    MusicView mview = new MusicView();
                    mview.Title = mi.Music.Title;
                    mview.Margin = new Thickness(0, eh, 0, 0);
                    mview.Height = ItemHeight;
                    mview.FontSize = 45;
                    mview.MouseLeftButtonDown += (sender, e) =>
                    {
                        MusicView mv = (MusicView)sender;
                        foreach (MusicView cur in SongStack.Children)
                        {
                            if (cur != mv)
                                cur.Hide();
                            else
                                cur.Show();
                        }
                        current = mi;
                    };
                    SongStack.Children.Add(mview);
                }
            }));
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (current != null)
                MusicSelected?.Invoke(this, new SelectedEventArgs(current.SgSongPath, current.Music));
        }
    }
}
