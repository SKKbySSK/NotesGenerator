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
    /// RankingView.xaml の相互作用ロジック
    /// </summary>
    public partial class RankingView : UserControl
    {
        public event EventHandler Dismissed;

        public RankingView()
        {
            InitializeComponent();
            UpdateRank();
            MouseLeftButtonDown += (sender, e) => Dismissed?.Invoke(this, new EventArgs());
        }

        void UpdateRank()
        {
            RankStack.Children.Clear();

            Ranking.Result cur = (Ranking.Result)Navigate.Parameters[Constants.NavResult];
            var ordered = Ranking.Saved.Results.Where((r) => cur.SgSongFile == r.SgSongFile).OrderByDescending((r) => r.Score);
            int i = 0, last = -1;
            foreach(var res in ordered)
            {
                if (res.Score != last)
                    i++;

                RankView rankView = new RankView(i);
                rankView.Result = res;
                RankStack.Children.Add(rankView);
                last = res.Score;


            }
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}
