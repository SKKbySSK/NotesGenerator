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
            MouseLeftButtonDown += ResultView_MouseLeftButtonDown;
        }

        private void ResultView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Dismissed?.Invoke(this, new EventArgs());
        }

        private void Score_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Score.Value >= Constants.RankS)
                RankL.Content = "S";
            else if (Score.Value >= Constants.RankA)
                RankL.Content = "A";
            else if (Score.Value >= Constants.RankB)
                RankL.Content = "B";
            else if (Score.Value >= Constants.RankC)
                RankL.Content = "C";
            else
                RankL.Content = "D";
        }

        public ReactiveProperty<int> PerfectCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> GreatCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> HitCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> FailedCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> Score { get; } = new ReactiveProperty<int>(-1);
    }
}
