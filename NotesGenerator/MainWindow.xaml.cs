using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Forms;
using System.Windows.Threading;
using System.IO;
using SugaEngine;
using SugaEngine.Export;
using Reactive.Bindings;
using NAudio.Wave;

namespace NotesGenerator
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Note> TempNotes { get; } = new ObservableCollection<Note>();
        private List<Note> SelectedNotes { get; } = new List<Note>();
        private Form GraphForm { get; set; }

        public ReactiveProperty<bool> DisableControls { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> Recording { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> Preview { get; } = new ReactiveProperty<bool>(false);

        bool changing = false;
        bool removing = false;

        string _song;
        private string SongPath
        {
            get { return _song; }
        }

        private bool SetSong(string Path)
        {
            Preview.Value = false;
            Recording.Value = false;
            GraphForm?.Close();
            GraphForm = null;
            if (Player != null)
            {
                Player.PlaybackStateChanged -= Player_PlaybackStateChanged;
                Player.Dispose();
                Player = null;
            }

            try
            {
                _song = Path;
                if (!string.IsNullOrEmpty(_song))
                {
                    if (Args.UseNullOut)
                        Player = new MusicPlayer(_song, new AsyncNullOut());
                    else
                        Player = new MusicPlayer(_song);

                    Player.PlaybackStateChanged += Player_PlaybackStateChanged;
                    Player.Rate = (float)PRateS.Value;
                    SeekBarS.Maximum = Player.Duration.TotalMilliseconds;
                    SeekBarS.Value = 0;
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(@"音楽ファイルの読み込み、または再生デバイスの初期化に失敗しました。
                    mp3ファイルまたはsgsongファイルであることを確認してください。
                    またはWASAPI排他モードを利用するソフトがある場合はそのソフトを閉じてから読み込んでください
                    -----メッセージ-----
                    " + ex.Message +
                    "\n-----スタックトレース-----\n" + ex.StackTrace);

                return false;
            }
        }

        private void Player_PlaybackStateChanged(object sender, EventArgs e)
        {
            switch (Player.State)
            {
                case NAudio.Wave.PlaybackState.Paused:
                case NAudio.Wave.PlaybackState.Stopped:
                    Preview.Value = false;
                    Recording.Value = false;
                    GraphForm?.Close();
                    GraphForm = null;
                    break;
            }
        }

        private MusicPlayer Player { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Normal,
                new EventHandler((sender, e) =>
                {
                    changing = true;
                    if (Player == null)
                        SeekBarS.Value = 0;
                    else
                        SeekBarS.Value = Player.Position.TotalMilliseconds;
                    changing = false;
                }), Dispatcher);

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            DispatcherTimer preview = new DispatcherTimer(TimeSpan.FromMilliseconds(20), DispatcherPriority.Send,
                new EventHandler((sender, e) =>
                {
                    if (Preview.Value)
                    {
                        sw.Stop();
                        ClearTempoFill();
                        TimeSpan pos = Player.Position;
                        TimeSpan range = sw.Elapsed;
                        foreach (Note note in TempNotes)
                        {
                            TimeSpan a = note.StartingTime - range;
                            TimeSpan b = note.StartingTime + range;

                            if (pos >= a && pos <= b)
                            {
                                var (lab, rect, key) = GetControls(note.Lane);
                                rect.Fill = Brushes.Red;
                            }
                        }
                        sw.Reset();
                        sw.Start();
                    }
                }), Dispatcher);

            Preview.PropertyChanged += (sender, e) => ClearTempoFill();
            Recording.PropertyChanged += (sender, e) => ClearTempoFill();
        }

        private void ClearTempoFill()
        {
            Fr.Fill = Brushes.White;
            Gr.Fill = Brushes.White;
            Hr.Fill = Brushes.White;
            Jr.Fill = Brushes.White;
            Kr.Fill = Brushes.White;
        }

        private void ExportB_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TitleT.Text))
            {
                System.Windows.Forms.MessageBox.Show("タイトルを設定してください");
                return;
            }

            if(string.IsNullOrEmpty(SongPath))
            {
                System.Windows.Forms.MessageBox.Show("音楽ファイルが選択されていません");
                return;
            }

            int bpm = 0;
            if (!int.TryParse(BPMT.Text, out bpm))
            {
                System.Windows.Forms.MessageBox.Show("BPMを正しく取得できませんでした。数字以外の文字列がないか確認してください");
                return;
            }

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "譜面の保存先を選択してください。中のファイルは自動で上書きされます";
            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = SongPath;
                SetSong(null);

                Music music = new Music();
                music.Title = TitleT.Text;
                music.BPM = bpm;
                music.Notes = TempNotes;
                Notes.Serialize(music, path, fbd.SelectedPath);

                SetSong(path);
            }
        }

        private void SongRefB_Click(object sender, RoutedEventArgs e)
        {
            if(TempNotes.Count > 0)
            {
                if (System.Windows.MessageBox.Show("まだ作業中のデータがありますが続行しますか？" +
                    "\n未保存のデータは削除されます",
                    "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "音楽ファイル|*.mp3;*.wav;*.flac;*.m4a|全てのファイル|*.*";
            ofd.FileName = "";
            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TempNotes.Clear();
                if (!SetSong(ofd.FileName))
                    return;

                SongPathT.Text = ofd.FileName;

                RWTag.TagReader reader = new RWTag.TagReader();
                using(FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    RWTag.Tag tag = reader.GetTag(fs, System.IO.Path.GetExtension(ofd.FileName));
                    TitleT.Text = tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);
                }
            }
        }

        private void LoadB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Suga Songファイル|*.sgsong";
            ofd.FileName = "";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Music music = LoadMusicFromFile(ofd.FileName);
                TitleT.Text = music.Title;
                BPMT.Text = music.BPM.ToString();

                TempNotes.Clear();
                foreach (Note note in music.Notes)
                    TempNotes.Add(note);

                string audio = System.IO.Path.GetDirectoryName(ofd.FileName) + music.Song;
                SetSong(audio);

                SongPathT.Text = audio;
            }
        }

        private Music LoadMusicFromFile(string FilePath)
        {
            Music music = Notes.Deserialize(FilePath);
            return music;
        }

        private void PPB_Click(object sender, RoutedEventArgs e)
        {
            if (Player == null) return;
            switch (Player.State)
            {
                case NAudio.Wave.PlaybackState.Paused:
                case NAudio.Wave.PlaybackState.Stopped:
                    Player.Play();
                    break;
                case NAudio.Wave.PlaybackState.Playing:
                    Player.Pause();
                    if (Recording.Value)
                    {
                        Recording.Value = false;
                        DisableControls.Value = false;
                    }
                    break;
            }
        }

        private void Fb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Recording.Value) return;
            (System.Windows.Controls.Label, Rectangle, int) cs = (null, null, -1);

            if (sender == Fb)
                cs = GetControls(Key.F);
            if (sender == Gb)
                cs = GetControls(Key.G);
            if (sender == Hb)
                cs = GetControls(Key.H);
            if (sender == Jb)
                cs = GetControls(Key.J);
            if (sender == Kb)
                cs = GetControls(Key.K);

            cs.Item2.Fill = Brushes.LimeGreen;
        }

        private void Fb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!Recording.Value) return;
            (System.Windows.Controls.Label, Rectangle, int) cs = (null, null, -1);

            if (sender == Fb)
                cs = GetControls(Key.F);
            if (sender == Gb)
                cs = GetControls(Key.G);
            if (sender == Hb)
                cs = GetControls(Key.H);
            if (sender == Jb)
                cs = GetControls(Key.J);
            if (sender == Kb)
                cs = GetControls(Key.K);

            cs.Item2.Fill = Brushes.White;
        }

        (System.Windows.Controls.Label, Rectangle, int) GetControls(Key Key)
        {
            switch (Key)
            {
                case Key.F:
                    return (Fb, Fr, 0);
                case Key.G:
                    return (Gb, Gr, 1);
                case Key.H:
                    return (Hb, Hr, 2);
                case Key.J:
                    return (Jb, Jr, 3);
                case Key.K:
                    return (Kb, Kr, 4);
                default:
                    return (null, null, -1);
            }
        }

        (System.Windows.Controls.Label, Rectangle, Key) GetControls(int Lane)
        {
            switch (Lane)
            {
                case 0:
                    return (Fb, Fr, Key.F);
                case 1:
                    return (Gb, Gr, Key.G);
                case 2:
                    return (Hb, Hr, Key.H);
                case 3:
                    return (Jb, Jr, Key.J);
                case 4:
                    return (Kb, Kr, Key.K);
                default:
                    return (null, null, Key.Escape);
            }
        }

        Key GetKey(UIElement Element)
        {
            if (Element == Fb || Element == Fr)
                return Key.F;
            if (Element == Gb || Element == Gr)
                return Key.G;
            if (Element == Hb || Element == Hr)
                return Key.H;
            if (Element == Jb || Element == Jr)
                return Key.J;
            if (Element == Kb || Element == Kr)
                return Key.K;

            throw new Exception();
        }

        private void RecordB_Click(object sender, RoutedEventArgs e)
        {
            if (Player == null) return;
            DisableControls.Value = true;
            Preview.Value = false;
            Player.Play();
            Recording.Value = true;
        }

        private void StopRecordB_Click(object sender, RoutedEventArgs e)
        {
            if (Player == null) return;
            DisableControls.Value = false;
            Player.Pause();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!Recording.Value) return;
            var (label, rect, lane) = GetControls(e.Key);
            if (rect != null)
            {
                rect.Fill = Brushes.LimeGreen;
                TempNotes.Add(new Note(Player.Position, lane));
            }
        }

        private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!Recording.Value) return;
            var (label, rect, lane) = GetControls(e.Key);
            if (rect != null)
                rect.Fill = Brushes.White;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Player?.Dispose();
        }

        private void PRateS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Player == null) return;
            Player.Rate = (float)PRateS.Value;
        }

        private void SeekBarS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(Player != null)
            {
                PosL.Content = Player.Position.ToString(@"mm\:ss");
                if (!changing)
                    Player.Position = TimeSpan.FromMilliseconds(SeekBarS.Value);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (removing)
                return;

            System.Windows.Controls.ListView lv = (System.Windows.Controls.ListView)e.Source;
            if (lv.SelectedItems.Count > 0)
            {
                SelectedNotes.Clear();
                RemoveNoteItem.IsEnabled = true;
                PreviewNoteItem.IsEnabled = true;
                foreach (Note note in lv.SelectedItems)
                    SelectedNotes.Add(note);

                if(SelectedNotes.Count == 1)
                {
                    LaneC.SelectedIndex = SelectedNotes[0].Lane;
                    millisecT.Text = SelectedNotes[0].StartingTime.TotalMilliseconds.ToString();
                    millisecT.IsEnabled = true;
                }
                else if(SelectedNotes.Count > 1)
                {
                    LaneC.SelectedIndex = -1;
                    millisecT.IsEnabled = false;
                }
            }
            else
            {
                RemoveNoteItem.IsEnabled = false;
                PreviewNoteItem.IsEnabled = false;
            }
        }

        private void RemoveNoteItem_Click(object sender, RoutedEventArgs e)
        {
            removing = true;
            foreach(Note note in SelectedNotes)
            {
                TempNotes.Remove(note);
            }
            removing = false;
        }

        private void PreviewB_Click(object sender, RoutedEventArgs e)
        {
            if (Player == null) return;
            Preview.Value = !Preview.Value;
            DisableControls.Value = Preview.Value;
            if (Preview.Value)
            {
                Player.Play();
            }
            else
            {
                Player.Pause();
            }
        }

        private void PreviewNoteItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNotes.Count == 0 || Player == null) return;
            double pos = SelectedNotes[0].StartingTime.TotalMilliseconds;
            Player.Position = TimeSpan.FromMilliseconds(pos);
            Preview.Value = true;
            DisableControls.Value = Preview.Value;
            Recording.Value = false;
            Player.Play();
        }

        private void UsePreviewerB_Click(object sender, RoutedEventArgs e)
        {
            string path = SongPath;
            SetSong(null);

            Music music = new Music();
            music.Title = "Temporary";
            music.Notes = TempNotes;
            Notes.Serialize(music, path, "Temp");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("NotesPreviewer.exe", @"temp\" + music.Title + ".sgsong")).WaitForExit();

            SetSong(path);
        }

        private void ChartB_Click(object sender, RoutedEventArgs e)
        {
            if(Player != null)
            {
                Form form = new Form();
                System.Windows.Forms.DataVisualization.Charting.Chart chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
                chart.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;

                {
                    var fftc = chart.ChartAreas.Add("FFT");
                    fftc.AxisX.Title = "Frequency[Hz]";
                    fftc.AxisX.Minimum = 0;
                    fftc.AxisY.Title = "Magnitude";
                    fftc.AxisY.Maximum = 15;

                    System.Windows.Forms.DataVisualization.Charting.Series fftseries = new System.Windows.Forms.DataVisualization.Charting.Series();
                    fftseries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    fftseries.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                    fftseries.ChartArea = "FFT";

                    var avc = chart.ChartAreas.Add("Average");
                    avc.AxisX.Maximum = 1;
                    avc.AxisY.Maximum = 15;

                    System.Windows.Forms.DataVisualization.Charting.Series avseries = new System.Windows.Forms.DataVisualization.Charting.Series();
                    avseries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
                    avseries.ChartArea = "Average";

                    //int count = Player.NumberOfFftSamples / 4;
                    //double max = 0, bfreq = Player.WaveFormat.SampleRate / 2 / count;

                    //EventHandler<FFT.FourierEventArgs> ev = (_, fft) =>
                    //{
                    //    Dispatcher.BeginInvoke(new Action(() =>
                    //    {
                    //        if (fftseries != null && fftseries.Points != null)
                    //        {
                    //            fftseries.Points.Clear();

                    //            double av = 0;
                    //            for (int i = 1; count >= i; i++)
                    //            {
                    //                double mag = fft.Samples[i - 1].Magnitude;

                    //                if (mag > max)
                    //                {
                    //                    max = mag;
                    //                }

                    //                fftseries.Points.AddXY(i * bfreq, mag);
                    //                av += mag;
                    //            }
                    //            av /= count;
                    //            avseries.Points.Clear();
                    //            avseries.Points.AddXY(1, av);
                    //        }
                    //    }));
                    //};

                    //Player.FftFinished += ev;

                    //chart.Series.Add(fftseries);
                    //chart.Series.Add(avseries);

                    //form.FormClosed += (_, _2) =>
                    //{
                    //    if (Player != null) Player.FftFinished -= ev;
                    //    GraphForm = null;
                    //    ChartB.IsEnabled = true;
                    //};
                }
                
                form.Controls.Add(chart);

                GraphForm = form;
                ChartB.IsEnabled = false;
                form.Size = new System.Drawing.Size(400, 400);
                form.Show();
            }
        }

        private void FftB_Click(object sender, RoutedEventArgs e)
        {
            Fft window = new Fft(_song, TempNotes);
            window.Show();
        }

        private void ResetRateB_Click(object sender, RoutedEventArgs e)
        {
            PRateS.Value = 1;
        }

        private void UnifyB_Click(object sender, RoutedEventArgs e)
        {
            if(TempNotes.Count > 0)
            {
                new UnifyDialog(TempNotes).ShowDialog();
            }
        }

        private void millisecT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(int.TryParse(millisecT.Text + e.Text, out int res))
            {
                if(SelectedNotes.Count == 1)
                {
                    SelectedNotes[0].StartingTime = TimeSpan.FromMilliseconds(res);
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LaneC.SelectedIndex > -1)
            {
                if (SelectedNotes.Count == 1)
                {
                    SelectedNotes[0].Lane = LaneC.SelectedIndex;
                }
            }
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
