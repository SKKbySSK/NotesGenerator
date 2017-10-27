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
using System.Windows.Threading;
using NotesPlayer.Extensions;

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
        }
        
        public void SetScore(int Score, int Combo, int Perfect, int Great, int Hit, int Failed)
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
                if (Score >= Constants.RankS)
                {
                    RankL.Content = "S";
                    brush = (Brush)Resources["RankSBrush"];
                }
                else if (Score >= Constants.RankA)
                {
                    RankL.Content = "A";
                    brush = (Brush)Resources["RankABrush"];
                }
                else if (Score >= Constants.RankB)
                {
                    RankL.Content = "B";
                    brush = (Brush)Resources["RankBBrush"];
                }
                else if (Score >= Constants.RankC)
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

            this.Score = Score;
            this.Combo = Combo;
            Pc = Perfect;
            Gc = Great;
            Hc = Hit;
            Fc = Failed;

            ScoreL.Animated += ScoreL_Animated;
            ScoreL.Score = Score;
        }

        private void ScoreL_Animated(object sender, EventArgs e)
        {
            ComboL.Duration = CalcSpan(Combo, TimeSpan.FromMilliseconds(1000));
            ComboL.Animated += ComboL_Animated;
            ComboL.Score = Combo;
        }

        private void ComboL_Animated(object sender, EventArgs e)
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(400);

            int i = 0;
            dt.Tick += (_, _2) =>
            {
                switch (i)
                {
                    case 0:
                        GreatL.Duration = CalcSpan(Gc, TimeSpan.FromMilliseconds(1000));
                        GreatL.Score = Gc;
                        break;
                    case 1:
                        HitL.Duration = CalcSpan(Hc, TimeSpan.FromMilliseconds(1000));
                        HitL.Score = Hc;
                        break;
                    case 2:
                        FailedL.Duration = CalcSpan(Fc, TimeSpan.FromMilliseconds(1000));
                        FailedL.Animated += FailedL_Animated;
                        FailedL.Score = Fc;
                        dt.Stop();
                        break;
                }
                i++;
            };
            PerfectL.Duration = CalcSpan(Pc, TimeSpan.FromMilliseconds(1000));
            PerfectL.Score = Pc;
            dt.Start();
        }

        private void FailedL_Animated(object sender, EventArgs e)
        {
            RankL.AnimateOpacity(1, 3000);
        }

        TimeSpan CalcSpan(int ToScore, TimeSpan PerHandred)
        {
            double p = ToScore / 100.0;
            return TimeSpan.FromMilliseconds(PerHandred.TotalMilliseconds * p);
        }
        
        private int Score, Combo, Pc, Gc, Hc, Fc;

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            Dismissed?.Invoke(this, new EventArgs());
        }
    }
}
