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

namespace NotesPlayer
{
    /// <summary>
    /// NotesDropper.xaml の相互作用ロジック
    /// </summary>
    public partial class NotesDropper : UserControl
    {
        public NotesDropper()
        {
            InitializeComponent();
            Dropper.Judged += (sender, e) => Judged?.Invoke(this, e);
        }

        public event EventHandler<JudgementEventArgs> Judged;

        public SugaEngine.INotesDispenser NotesDispenser
        {
            get { return Dropper.NotesDispenser; }
            set { Dropper.NotesDispenser = value; }
        }

        public double PerfectDiff
        {
            get { return Dropper.PerfectDiff; }
            set { Dropper.PerfectDiff = value; }
        }

        public double GreatDiff
        {
            get { return Dropper.GreatDiff; }
            set { Dropper.GreatDiff = value; }
        }

        public double HitDiff
        {
            get { return Dropper.HitDiff; }
            set { Dropper.HitDiff = value; }
        }

        public TimeSpan Duration
        {
            get { return Dropper.Duration; }
            set { Dropper.Duration = value; }
        }
    }
}
