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
        private Difficulty difficulty;

        public DifficultyView()
        {
            InitializeComponent();
            image.Source = BackgroundImageManager.GetDifficultyImage();
        }

        public void RefreshDifficulty(MusicHolder holder)
        {
            hardR.Visibility = Visibility.Collapsed;
            normalR.Visibility = Visibility.Collapsed;
            easyR.Visibility = Visibility.Collapsed;

            if (holder.GetMusic(Difficulty.Hard) != null)
            {
                difficulty = Difficulty.Hard;
                hardR.IsChecked = true;
                hardR.Visibility = Visibility.Visible;
            }

            if (holder.GetMusic(Difficulty.Easy) != null)
            {
                difficulty = Difficulty.Easy;
                easyR.IsChecked = true;
                easyR.Visibility = Visibility.Visible;
            }

            if (holder.GetMusic(Difficulty.Normal) != null)
            {
                difficulty = Difficulty.Normal;
                normalR.IsChecked = true;
                normalR.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameT.Text))
            {
                MessageBox.Show("名前を入力してください", null, MessageBoxButton.OK);
                return;
            }

            Navigate.Parameters[Constants.NavDifficulty] = difficulty;

            string uname = NameT.Text.TrimStart(' ', '　').TrimEnd(' ', '　');
            Navigate.Parameters[Constants.NavUserNameKey] = uname;
            Ready?.Invoke(this, new DifficultyEventArgs(difficulty));
        }

        private void easyR_Checked(object sender, RoutedEventArgs e)
        {
            difficulty = Difficulty.Easy;
        }

        private void normalR_Checked(object sender, RoutedEventArgs e)
        {
            difficulty = Difficulty.Normal;
        }

        private void hardR_Checked(object sender, RoutedEventArgs e)
        {
            difficulty = Difficulty.Hard;
        }
    }
}
