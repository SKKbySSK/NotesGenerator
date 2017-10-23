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
using ofd = System.Windows.Forms.OpenFileDialog;
using Reactive.Bindings;

namespace NotesPlayer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MusicPlayer player;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ReactiveProperty<int> Score { get; } = new ReactiveProperty<int>(0);
        public const int MaximumScore = 150000;

        int perfectScore { get; set; } = 0;
        int greatScore { get; set; } = 0;
        int hitScore { get; set; } = 0;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ofd ofd = new ofd();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Score.Value = 0;
                if(player != null)
                {
                    player.Dispose();
                    player = null;
                }
                SugaEngine.Music music = SugaEngine.Export.Notes.Deserialize(ofd.FileName);

                perfectScore = MaximumScore / music.Notes.Count;

                try
                {
                    player = new MusicPlayer(System.IO.Path.GetDirectoryName(ofd.FileName),
                        music, new NAudio.Wave.WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, 50));
                }
                catch (Exception)
                {
                    player = new MusicPlayer(System.IO.Path.GetDirectoryName(ofd.FileName),
                        music, new NAudio.Wave.NullOut());
                }
                Dropper.NotesDispenser = player;
                player.Play();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            player?.Dispose();
            player = null;
        }

        private void Dropper_Judged(object sender, JudgementEventArgs e)
        {
            switch (e.Judgement)
            {
                case NoteJudgement.Perfect:
                    Score.Value += perfectScore;
                    break;
                case NoteJudgement.Great:
                    Score.Value += greatScore;
                    break;
                case NoteJudgement.Hit:
                    Score.Value += hitScore;
                    break;
            }
        }
    }
}
