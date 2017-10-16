using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SugaEngine;
using SugaEngine.Export;
using NotesGenerator;

namespace NotesPreviewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MusicPlayer Player { get; set; }
        PlayableMusic Music { get; set; }
        int Range = 100;
        int Appear = 500;

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Send, new EventHandler((sender, e) =>
            {
                if (IsPlayable())
                {
                    TimeSpan current = Player.Position + TimeSpan.FromMilliseconds(Appear);
                    List<PlayableNote> notes = new List<PlayableNote>();
                    foreach (PlayableNote n in Music.PlayableNotes)
                    {
                        if (!n.HasPlayed)
                        {
                            TimeSpan diff = current - n.StartingTime;
                            if (diff.TotalMilliseconds <= Range && -Range <= diff.TotalMilliseconds)
                            {
                                Rectangle rect = GenerateRect(n);
                                RectangleParent.Children.Add(rect);
                                AnimateRect(rect, Appear);
                                n.HasPlayed = true;
                            }
                        }
                    }
                }
            }), Dispatcher);


            Closed += MainWindow_Closed;
            Open(Args.Path);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Player?.Dispose();
        }

        bool IsPlayable()
        {
            return Music != null && Player != null;
        }

        Rectangle GenerateRect(Note Note)
        {
            Rectangle rect = new Rectangle();

            switch (Note.Mode)
            {
                case NoteMode.Continuous:
                    rect.Fill = Brushes.Blue;
                    break;
                case NoteMode.Tap:
                    rect.Fill = Brushes.White;
                    break;
            }

            Grid.SetColumn(rect, Note.Lane);
            rect.Height = 20;
            rect.VerticalAlignment = VerticalAlignment.Top;

            return rect;
        }

        void AnimateRect(Rectangle Rect, int Ms)
        {
            ThicknessAnimation ta = new ThicknessAnimation(new Thickness(0, RectangleParent.ActualHeight, 0, 0), new Duration(TimeSpan.FromMilliseconds(Ms)));

            EventHandler ev = null;
            Action<object, EventArgs> completed = (sender, e) =>
            {
                RectangleParent.Children.Remove(Rect);
                ta.Completed -= ev;
            };
            ev = new EventHandler(completed);

            ta.Completed += ev;
            Rect.BeginAnimation(MarginProperty, ta);
        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Suga Songファイル|*.sgsong";
            ofd.FileName = "";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Open(ofd.FileName);
            }
        }

        void Open(string Path)
        {
            Music music = Notes.Deserialize(Args.Path);

            string audio = System.IO.Path.GetDirectoryName(Args.Path) + music.Song;
            Player = new MusicPlayer(audio);

            Music = new PlayableMusic(music);

            Player.Rate = 1;
            Player.Play();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Appear = (int)e.NewValue;
        }
    }
}
