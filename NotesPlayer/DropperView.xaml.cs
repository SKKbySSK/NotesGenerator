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
    class NoteRectangle
    {
        public NoteRectangle(SugaEngine.Note Note, Rectangle Element)
        {
            this.Note = Note;
            this.Element = Element;
        }

        public SugaEngine.Note Note { get; }
        public Rectangle Element { get; }
        public double Position { get; set; }
        public NoteJudgement Judgement { get; set; } = NoteJudgement.Failed;
        public bool IsJudged { get; set; } = false;
    }

    public class JudgementEventArgs : EventArgs
    {
        public JudgementEventArgs(NoteJudgement judgement, double difference, SugaEngine.Note note)
        {
            Judgement = judgement;
            Difference = difference;
            Note = note;
        }

        public NoteJudgement Judgement { get; }
        public double Difference { get; }
        public SugaEngine.Note Note { get; }
    }

    public enum NoteJudgement
    {
        Perfect,
        Great,
        Hit,
        Failed
    }

    /// <summary>
    /// DropperView.xaml の相互作用ロジック
    /// </summary>
    public partial class DropperView : UserControl
    {
        const int TimerInterval = 50;

        public event EventHandler<JudgementEventArgs> Judged;

        public double PerfectDiff { get; set; } = 0.08;
        public double GreatDiff { get; set; } = 0.16;
        public double HitDiff { get; set; } = 0.24;

        public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(900);

        #region Notes

        SEPlayer SEPlayer = new SEPlayer();
        List<NoteRectangle> RunnningNotes { get; } = new List<NoteRectangle>();

        Rectangle InitRect(int Index)
        {
            double height = (double)Application.Current.Resources["NoteHeight"];
            Brush fbrush = (Brush)Application.Current.Resources["NoteFillBrush"];
            Brush sbrush = (Brush)Application.Current.Resources["NoteStrokeBrush"];
            double sthickness = (double)Application.Current.Resources["NoteStroke"];
            int insert = DropperParent.Children.IndexOf(Key1l);
            Rectangle r = new Rectangle();
            r.Fill = fbrush;
            r.Stroke = sbrush;
            r.StrokeThickness = sthickness;
            r.Height = height;
            r.VerticalAlignment = VerticalAlignment.Top;
            r.HorizontalAlignment = HorizontalAlignment.Stretch;
            r.Margin = new Thickness(0, -height, 0, 0);
            Grid.SetColumn(r, Index);

            DropperParent.Children.Insert(insert, r);
            return r;
        }

        Thickness? GetMarginForRect(double Value)
        {
            double height = DropperParent.ActualHeight;
            if (height > 0)
            {
                double nh = (double)Application.Current.Resources["NoteHeight"];
                height += nh * 2;

                return new Thickness(0, (height * Value) - nh, 0, 0);
            }
            else
                return null;
        }

        public bool RunNote(SugaEngine.Note Note, TimeSpan Duration)
        {
            foreach(var note in RunnningNotes)
            {
                if (note.Note == Note)
                    return false;
            }

            Rectangle rect = InitRect(Note.Lane);
            NoteRectangle nr = new NoteRectangle(Note, rect);

            double ybarpos = DropperParent.ActualHeight - (HitBarR.ActualHeight / 2) - HitBarR.Margin.Bottom;
            double ratio = ybarpos / DropperParent.ActualHeight;

            Thickness? from = GetMarginForRect(0);
            Thickness? bar = GetMarginForRect(ratio);
            Thickness? to = GetMarginForRect(1);

            double actTime = Duration.TotalMilliseconds * ratio;

            if (from.HasValue && to.HasValue && bar.HasValue)
            {
                ThicknessAnimation ta = new ThicknessAnimation(rect, from.Value, to.Value, Duration.TotalMilliseconds);

                EventHandler<ThicknessEventArgs> changed = null;
                changed = (sender, e) =>
                {
                    nr.Position = (e.Current.Top + (nr.Element.ActualHeight / 2)) / ybarpos;
                    nr.Judgement = Judge(nr.Position);
                };
                ta.Changed += changed;

                EventHandler ev = null;
                ev = (sender, e) =>
                {
                    if (!nr.IsJudged)
                    {
                        nr.Judgement = NoteJudgement.Failed;
                        RaiseJudgement(nr);
                    }
                    ta.Completed -= ev;
                    ta.Changed -= changed;
                };
                ta.Completed += ev;

                RunnningNotes.Add(nr);
                ta.BeginAnimation();
                return true;
            }
            else
                return false;
        }

        NoteJudgement Judge(double Position)
        {
            double diff = Math.Abs(1 - Position);

            if (Instance.FullAutomatic && diff <= 0.001)
                return NoteJudgement.Perfect;

            if (diff <= PerfectDiff)
                return NoteJudgement.Perfect;
            else if (diff <= GreatDiff)
                return NoteJudgement.Great;
            else if (diff <= HitDiff)
                return NoteJudgement.Hit;
            else
                return NoteJudgement.Failed;
        }

        void RaiseJudgement(NoteRectangle NoteRectangle)
        {
            Console.WriteLine(NoteRectangle.Judgement);
            DropperParent.Children.Remove(NoteRectangle.Element);
            RunnningNotes.Remove(NoteRectangle);
            NoteRectangle.IsJudged = true;
            StartJudgeAnim(NoteRectangle.Judgement, NoteRectangle.Note.Lane);

            if(NoteRectangle.Judgement != NoteJudgement.Failed)
            {
                SEPlayer.Play((int)NoteRectangle.Judgement);
            }

            Judged?.Invoke(this,
                new JudgementEventArgs(NoteRectangle.Judgement, Math.Abs(1 - NoteRectangle.Position), NoteRectangle.Note));
        }

        Storyboard[] JudgeAnimations = new Storyboard[5];

        void StartJudgeAnim(NoteJudgement Judgement, int Index)
        {
            var sb = JudgeAnimations[Index];
            sb.Stop(DropperParent);

            string name = "Judge" + (Index + 1);

            JudgementLabels.Judge judge = (JudgementLabels.Judge)FindName(name);
            if(judge != null)
            {
                EventHandler ev = null;
                ev = (sender, e) =>
                {
                    judge.Judgement = null;
                    sb.Completed -= ev;
                };
                sb.Completed += ev;

                sb.Begin(DropperParent);
                judge.Judgement = Judgement;
            }
        }

        private void Sb_Completed(object sender, EventArgs e)
        {
            JudgementLabels.Judge judge = (JudgementLabels.Judge)FindName(Storyboard.GetTargetName((Storyboard)sender));
            judge.Judgement = null;
        }

        #endregion

        DispatcherTimer dispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
        public DropperView()
        {
            InitializeComponent();
            Keys = new KeyData();
            KeyboardHook.HookEvent += KeyboardHook_HookEvent;

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();

            var sb = (Storyboard)Resources["JudgeAnim"];
            for (int i = 0;JudgeAnimations.Length > i; i++)
            {
                var clone = sb.Clone();
                foreach (var child in clone.Children)
                {
                    Storyboard.SetTargetName(child, "Judge" + (i + 1));
                }
                JudgeAnimations[i] = clone;
            }

            //TODO SEの追加
            SEPlayer.SEPaths.Add("SE/Perfect.wav");
            SEPlayer.SEPaths.Add("SE/Perfect.wav");
            SEPlayer.SEPaths.Add("SE/Perfect.wav");
        }

        SugaEngine.INotesDispenser _nd;
        public SugaEngine.INotesDispenser NotesDispenser
        {
            get { return _nd; }
            set
            {
                _nd = value;
                if (value == null)
                {
                    KeyboardHook.Stop();
                    dispatcherTimer.Stop();
                }
                else
                {
                    KeyboardHook.Start();
                    dispatcherTimer.Start();
                }
            }
        }

        Stopwatch sw = new Stopwatch();
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = sw.Elapsed;
            sw.Reset();
            sw.Start();

            if(NotesDispenser != null)
            {
                TimeSpan current = NotesDispenser.GetCurrentTime();
                current += Duration;

                var notes = NotesDispenser.Dispnse(current, current + elapsed);
                foreach (var note in notes)
                {
                    RunNote(note, Duration);
                }
                

                if (Instance.FullAutomatic)
                {
                    var pnotes = RunnningNotes.Where((n) => n.Judgement == NoteJudgement.Perfect).ToArray();
                    foreach(var note in pnotes)
                    {
                        if (note.Judgement == NoteJudgement.Perfect)
                            RaiseJudgement(note);
                    }
                }
            }
        }

        (bool, int, Label, Rectangle) GetViews(Keys Key)
        {
            bool proc = Key == Keys.Key1 || Key == Keys.Key2 || Key == Keys.Key3 || Key == Keys.Key4 || Key == Keys.Key5;
            int index = 0;
            Label lab = null;
            Rectangle rect = null;
            if (Key == Keys.Key1)
            {
                lab = Key1l;
                rect = Key1r;
            }

            if (Key == Keys.Key2)
            {
                index = 1;
                lab = Key2l;
                rect = Key2r;
            }

            if (Key == Keys.Key3)
            {
                index = 2;
                lab = Key3l;
                rect = Key3r;
            }

            if (Key == Keys.Key4)
            {
                index = 3;
                lab = Key4l;
                rect = Key4r;
            }

            if (Key == Keys.Key5)
            {
                index = 4;
                lab = Key5l;
                rect = Key5r;
            }

            return (proc, index, lab, rect);
        }

        private void KeyboardHook_HookEvent(ref KeyboardHook.StateKeyboard state)
        {
            Keys key = state.Key;
            var views = GetViews(key);
            if (views.Item1)
            {
                if (state.Stroke == KeyboardHook.Stroke.KEY_DOWN)
                {
                    views.Item4.Fill = (Brush)Application.Current.Resources["LanePressedFillBrush"];

                    var near = RunnningNotes.Where((note) => note.Note.Lane == views.Item2).OrderBy((note) => note.Note.StartingTime);
                    foreach(var note in near)
                    {
                        RaiseJudgement(note);
                        break;
                    }
                }
                else if (state.Stroke == KeyboardHook.Stroke.KEY_UP)
                    views.Item4.Fill = (Brush)Application.Current.Resources["LaneFillBrush"];
            }
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
