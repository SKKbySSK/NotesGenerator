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

namespace NotesPlayer.JudgementLabels
{
    /// <summary>
    /// Judge.xaml の相互作用ロジック
    /// </summary>
    public partial class Judge : UserControl
    {
        public Judge()
        {
            InitializeComponent();
            Judgement = null;
        }

        NoteJudgement? noteJudgement;
        public NoteJudgement? Judgement
        {
            get { return noteJudgement; }
            set
            {
                noteJudgement = value;

                PerfectL.Visibility = Visibility.Hidden;
                GreatL.Visibility = Visibility.Hidden;
                HitL.Visibility = Visibility.Hidden;
                FailedL.Visibility = Visibility.Hidden;

                if (noteJudgement.HasValue)
                {
                    switch (noteJudgement)
                    {
                        case NoteJudgement.Perfect:
                            PerfectL.Visibility = Visibility.Visible;
                            break;
                        case NoteJudgement.Great:
                            GreatL.Visibility = Visibility.Visible;
                            break;
                        case NoteJudgement.Hit:
                            HitL.Visibility = Visibility.Visible;
                            break;
                        case NoteJudgement.Failed:
                            FailedL.Visibility = Visibility.Visible;
                            break;
                    }
                }
            }
        }
    }
}
