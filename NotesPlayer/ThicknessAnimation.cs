using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace NotesPlayer
{
    public class ThicknessEventArgs
    {
        public ThicknessEventArgs(Thickness Current)
        {
            this.Current = Current;
        }

        public Thickness Current { get; }
    }
    public class ThicknessAnimation
    {
        public ThicknessAnimation(FrameworkElement Element, Thickness From, Thickness To, double Duration)
        {
            this.Element = Element;
            this.From = From;
            this.To = To;
            this.Duration = Duration;

            diff = new Thickness(To.Left - From.Left, To.Top - From.Top, To.Right - From.Right, To.Bottom - From.Bottom);
        }

        public event EventHandler<ThicknessEventArgs> Changed;
        public event EventHandler Completed;

        public FrameworkElement Element { get; }
        public Thickness From { get; }
        public Thickness To { get; }
        public double Duration { get; }
        public int ErrorRange { get; set; } = 15;

        private Thickness diff { get; }
        private Stopwatch sw { get; } = new Stopwatch();
        private long framecounter = -1;

        public void BeginAnimation()
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            TimeSpan elapsed = sw.Elapsed;
            if(framecounter++ == 0)
            {
                sw.Start();
            }
            
            double ratio = elapsed.TotalMilliseconds / Duration;
            Thickness current = From;
            current.Left += diff.Left * ratio;
            current.Top += diff.Top * ratio;
            current.Right += diff.Right * ratio;
            current.Bottom += diff.Bottom * ratio;
            Element.Margin = current;
            Changed?.Invoke(this, new ThicknessEventArgs(current));

            if (elapsed.TotalMilliseconds + ErrorRange >= Duration)
            {
                sw.Stop();
                sw.Reset();
                framecounter = -1;
                Completed?.Invoke(this, new EventArgs());
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }
    }
}
