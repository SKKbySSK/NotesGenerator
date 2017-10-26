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
        public ResultView()
        {
            InitializeComponent();
        }

        public ReactiveProperty<int> PerfectCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> GreatCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> HitCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> FailedCount { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> Score { get; } = new ReactiveProperty<int>(0);
    }
}
