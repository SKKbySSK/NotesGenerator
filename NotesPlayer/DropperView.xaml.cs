using System;
using System.Collections;
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
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;
using Keys = System.Windows.Forms.Keys;

namespace NotesPlayer
{
    /// <summary>
    /// DropperView.xaml の相互作用ロジック
    /// </summary>
    public partial class DropperView : UserControl
    {
        const int TimerInterval = 50;
        
#region TimerAction
        IEnumerator ContinuousActions(params Action[] Actions)
        {
            foreach(Action act in Actions)
            {
                act?.Invoke();
                yield return null;
            }
        }
#endregion

        List<IEnumerator> TickActions { get; } = new List<IEnumerator>();

        DispatcherTimer dispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
        public DropperView()
        {
            InitializeComponent();
            Keys = new KeyData();
            KeyboardHook.HookEvent += KeyboardHook_HookEvent;

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        Stopwatch sw = new Stopwatch();
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            sw.Stop();

            if (sw.ElapsedMilliseconds > 0)
                Console.WriteLine($"{sw.ElapsedMilliseconds} ms, diff {sw.ElapsedMilliseconds - dispatcherTimer.Interval.TotalMilliseconds} ms");
            sw.Reset();
            sw.Start();
            
            for(int i = 0; TickActions.Count > i; i++)
            {
                if (!TickActions[i].MoveNext())
                {
                    TickActions.RemoveAt(i);
                    i--;
                }
            }
        }

        (bool, Label, Rectangle) GetViews(Keys Key)
        {
            bool proc = Key == Keys.Key1 || Key == Keys.Key2 || Key == Keys.Key3 || Key == Keys.Key4 || Key == Keys.Key5;
            Label lab = null;
            Rectangle rect = null;
            if (Key == Keys.Key1)
            {
                lab = Key1l;
                rect = Key1r;
            }

            if (Key == Keys.Key2)
            {
                lab = Key2l;
                rect = Key2r;
            }

            if (Key == Keys.Key3)
            {
                lab = Key3l;
                rect = Key3r;
            }

            if (Key == Keys.Key4)
            {
                lab = Key4l;
                rect = Key4r;
            }

            if (Key == Keys.Key5)
            {
                lab = Key5l;
                rect = Key5r;
            }

            return (proc, lab, rect);
        }

        private void KeyboardHook_HookEvent(ref KeyboardHook.StateKeyboard state)
        {
            Keys key = state.Key;
            var views = GetViews(key);
            if (views.Item1)
            {
                if (state.Stroke == KeyboardHook.Stroke.KEY_DOWN)
                    views.Item3.Fill = (Brush)Application.Current.Resources["LanePressedFillBrush"];
                else if (state.Stroke == KeyboardHook.Stroke.KEY_UP)
                    views.Item3.Fill = (Brush)Application.Current.Resources["LaneFillBrush"];
            }
        }

        public void BeginKeyHook()
        {
            KeyboardHook.Start();
        }

        KeyData kd;
        public KeyData Keys
        {
            get { return kd; }
            set
            {
                kd = value;
                Key1l.Content = kd.Key1.ToString();
                Key2l.Content = kd.Key2.ToString();
                Key3l.Content = kd.Key3.ToString();
                Key4l.Content = kd.Key4.ToString();
                Key5l.Content = kd.Key5.ToString();
            }
        }
    }

    public class KeyData
    {
        public Keys Key1 { get; set; } = Keys.F;
        public Keys Key2 { get; set; } = Keys.G;
        public Keys Key3 { get; set; } = Keys.H;
        public Keys Key4 { get; set; } = Keys.J;
        public Keys Key5 { get; set; } = Keys.K;
    }
}
