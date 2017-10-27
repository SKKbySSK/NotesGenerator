using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Diagnostics;

namespace NotesPlayer.Controls
{
    /// <summary>
    /// AnimatedLabel.xaml の相互作用ロジック
    /// </summary>
    public partial class AnimatedLabel : UserControl, INotifyPropertyChanged
    {
        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;

        int sc = 0;
        public int Score
        {
            get { return sc; }
            set
            {
                if(value != sc)
                {
                    if (proc)
                        StopAnimate();
                    diff = value - sc;
                    lastScore = sc;
                    sc = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Score)));
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                }
            }
        }

        public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(1000);
        
        private Stopwatch sw { get; } = new Stopwatch();
        private long framecounter = -1;
        private bool proc = false;
        private int diff;
        private int lastScore;
        #endregion

        public event EventHandler Animated;

        public AnimatedLabel()
        {
            InitializeComponent();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            TimeSpan elapsed = sw.Elapsed;
            if (framecounter++ == 0)
            {
                proc = true;
                sw.Start();
            }

            double ratio = elapsed.TotalMilliseconds / Duration.TotalMilliseconds;

            if (elapsed >= Duration || ratio >= 1)
            {
                StopAnimate();
            }
            else
                UpdateContent((int)(lastScore + (diff * ratio)));
        }

        private void StopAnimate()
        {
            proc = false;
            sw.Stop();
            sw.Reset();
            framecounter = -1;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            UpdateContent(Score);
            Animated?.Invoke(this, new EventArgs());
        }

        void UpdateContent(int Score)
        {
            if (string.IsNullOrEmpty(ContentStringFormat))
                label.Content = Score;
            else
                label.Content = string.Format(ContentStringFormat, Score);
        }
    }
}
