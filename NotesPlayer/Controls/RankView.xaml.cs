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

namespace NotesPlayer.Controls
{
    /// <summary>
    /// RankView.xaml の相互作用ロジック
    /// </summary>
    public partial class RankView : UserControl
    {
        public RankView(int Rank)
        {
            InitializeComponent();
            RankT.Content = Rank;

            switch (Rank)
            {
                case 1:
                    LabParent.Background = (Brush)Resources["FirstBrush"];
                    break;
                case 2:
                    LabParent.Background = (Brush)Resources["SecondBrush"];
                    break;
                case 3:
                    LabParent.Background = (Brush)Resources["ThirdBrush"];
                    break;
                default:
                    LabParent.Background = (Brush)Resources["RankBrush"];
                    break;
            }
        }

        Ranking.Result res = null;
        public Ranking.Result Result
        {
            get { return res; }
            set
            {
                res = value;
                NameT.Content = res.UserName;
                ScoreT.Content = res.Score;
                ComboT.Content = res.MaxCombo;
            }
        }
    }
}
