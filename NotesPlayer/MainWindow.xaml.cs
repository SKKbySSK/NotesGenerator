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
using ofd = System.Windows.Forms.OpenFileDialog;

namespace NotesPlayer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ofd ofd = new ofd() { Filter = "|*.sgsong" };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PlayerV.SetMusic(ofd.FileName);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PlayerV.StopMusic();
        }

        private void PlayerV_Finished(object sender, PlayerFinishedEventArgs e)
        {
            ResultV.PerfectCount.Value = e.Judged.Where((j) => j.Item1 == NoteJudgement.Perfect).Count();
            ResultV.GreatCount.Value = e.Judged.Where((j) => j.Item1 == NoteJudgement.Great).Count();
            ResultV.HitCount.Value = e.Judged.Where((j) => j.Item1 == NoteJudgement.Hit).Count();
            ResultV.FailedCount.Value = e.Judged.Where((j) => j.Item1 == NoteJudgement.Failed).Count();
            ResultV.Score.Value = e.Score;

            var sb = (System.Windows.Media.Animation.Storyboard)Resources["AnimateToResult"];
            sb.Begin();
        }
    }
}
