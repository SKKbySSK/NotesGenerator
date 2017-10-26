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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NotesPlayer.Extensions;

namespace NotesPlayer.Controls
{
    public class DifficultyEventArgs : EventArgs
    {
        public DifficultyEventArgs(Difficulty Difficulty)
        {
            this.Difficulty = Difficulty;
        }

        public Difficulty Difficulty { get; }
    }
    /// <summary>
    /// DifficultyView.xaml の相互作用ロジック
    /// </summary>
    public partial class DifficultyView : UserControl
    {
        public event EventHandler<DifficultyEventArgs> Ready;

        Difficulty? difficulty = null;

        bool inputShown = false;
        public DifficultyView()
        {
            InitializeComponent();

            Storyboard normal = ((Storyboard)Resources["HoldAnim"]).Clone();
            foreach(var tl in normal.Children)
            {
                Storyboard.SetTargetName(tl, "NormalB");
            }
            NormalAnim_BeginStoryboard.Storyboard = normal;
            Storyboard easy = ((Storyboard)Resources["HoldAnim"]).Clone();
            foreach (var tl in easy.Children)
            {
                Storyboard.SetTargetName(tl, "EasyB");
            }
            EasyAnim_BeginStoryboard.Storyboard = easy;

            HardR.Opacity = 0;
            HardR.Visibility = Visibility.Hidden;
            NormalR.Opacity = 0;
            NormalR.Visibility = Visibility.Hidden;
            EasyR.Opacity = 0;
            EasyR.Visibility = Visibility.Hidden;
        }

        private void HardB_Clicked(object sender, EventArgs e)
        {
            VisibleRects();
            EasyR.AnimateOpacity(1);
            NormalR.AnimateOpacity(1);
            HardR.Visibility = Visibility.Hidden;
            ShowInputAnimate();
            difficulty = Difficulty.Hard;
        }

        private void NormalB_Clicked(object sender, EventArgs e)
        {
            VisibleRects();
            HardR.AnimateOpacity(1);
            EasyR.AnimateOpacity(1);
            NormalR.Visibility = Visibility.Hidden;
            ShowInputAnimate();
            difficulty = Difficulty.Normal;
        }

        private void EasyB_Clicked(object sender, EventArgs e)
        {
            VisibleRects();
            HardR.AnimateOpacity(1);
            NormalR.AnimateOpacity(1);
            EasyR.Visibility = Visibility.Hidden;
            ShowInputAnimate();
            difficulty = Difficulty.Easy;
        }

        void VisibleRects()
        {
            HardR.Visibility = Visibility.Visible;
            NormalR.Visibility = Visibility.Visible;
            EasyR.Visibility = Visibility.Visible;
        }

        private void HardR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisibleRects();
            NormalR.AnimateOpacity(1);
            EasyR.AnimateOpacity(1);
            HardR.AnimateOpacity(0, Completed: (element) => element.Visibility = Visibility.Hidden);
            difficulty = Difficulty.Hard;
        }

        private void NormalR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisibleRects();
            HardR.AnimateOpacity(1);
            EasyR.AnimateOpacity(1);
            NormalR.AnimateOpacity(0, Completed: (element) => element.Visibility = Visibility.Hidden);
            difficulty = Difficulty.Normal;
        }

        private void EasyR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisibleRects();
            HardR.AnimateOpacity(1);
            NormalR.AnimateOpacity(1);
            EasyR.AnimateOpacity(0, Completed: (element) => element.Visibility = Visibility.Hidden);
            difficulty = Difficulty.Easy;
        }

        void ShowInputAnimate()
        {
            if (inputShown)
                return;
            ((Storyboard)Resources["ShowInput"]).Begin();
            inputShown = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameT.Text))
            {
                MessageBox.Show("名前を入力してください", null, MessageBoxButton.OK);
                return;
            }

            string path = (string)Navigate.Parameters[Constants.NavMusicFileKey];
            switch (difficulty)
            {
                case Difficulty.Easy:
                    path = path.Replace(".sgsong", Constants.EasySgSongName);
                    break;
                case Difficulty.Normal:
                    path = path.Replace(".sgsong", Constants.NormalSgSongName);
                    break;
            }
            Navigate.Parameters[Constants.NavMusicFileKey] = path;
            Navigate.Parameters[Constants.NavMusicKey] = SugaEngine.Export.Notes.Deserialize(path);
            Navigate.Parameters[Constants.NavDifficulty] = difficulty;

            string fname = System.IO.Path.GetFileName(path);
            string uname = NameT.Text.TrimStart(' ', '　').TrimEnd(' ', '　');

            Navigate.Parameters[Constants.NavSgSongPath] = fname;
            Navigate.Parameters[Constants.NavUserNameKey] = uname;

            if (difficulty.HasValue)
                Ready?.Invoke(this, new DifficultyEventArgs(difficulty.Value));
        }
    }
}
