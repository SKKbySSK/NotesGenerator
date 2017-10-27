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
using Reactive.Bindings;

namespace NotesPlayer.Controls
{
    /// <summary>
    /// ResultView.xaml の相互作用ロジック
    /// </summary>
    public partial class ResultView : UserControl
    {
        public event EventHandler Dismissed;

        public ResultView()
        {
            InitializeComponent();
            Score.PropertyChanged += Score_PropertyChanged;
        }
        
        private void Score_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Brush brush = null;

            if (Instance.OverrideRank)
            {
                RankL.Content = Instance.Rank.ToUpper();
                switch (Instance.Rank[0])
                {
                    case 's':
                        brush = (Brush)Resources["RankSBrush"];
                        break;
                    case 'a':
                        brush = (Brush)Resources["RankABrush"];
                        break;
                    case 'b':
                        brush = (Brush)Resources["RankBBrush"];
                        break;
                    case 'c':
                        brush = (Brush)Resources["RankCBrush"];
                        break;
                    case 'd':
                        brush = (Brush)Resources["RankDBrush"];
                        break;
                }
            }
            else
            {
                if (Score.Value >= Constants.RankS)
                {
                    RankL.Content = "S";
                    brush = (Brush)Resources["RankSBrush"];
                }
                else if (Score.Value >= Constants.RankA)
                {
                    RankL.Content = "A";
                    brush = (Brush)Resources["RankABrush"];
                }
                else if (Score.Value >= Constants.RankB)
                {
                    RankL.Content = "B";
                    brush = (Brush)Resources["RankBBrush"];
                }
                else if (Score.Value >= Constants.RankC)
                {
                    RankL.Content = "C";
                    brush = (Brush)Resources["RankCBrush"];
                }
                else
                {
                    RankL.Content = "D";
                    brush = (Brush)Resources["RankDBrush"];
                }
            }

            RankL.Foreground = brush ?? new SolidColorBrush(Colors.Black);
            ScoreL.Foreground = brush ?? new SolidColorBrush(Colors.Black);
        }

        public ReactiveProperty<int> PerfectCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> GreatCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> HitCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> FailedCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> Score { get; } = new ReactiveProperty<int>(-1);
        public ReactiveProperty<int> Combo { get; } = new ReactiveProperty<int>(0);

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            Dismissed?.Invoke(this, new EventArgs());
        }
    }
}
