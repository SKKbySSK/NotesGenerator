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
using System.Windows.Shapes;
using ofd = System.Windows.Forms.OpenFileDialog;
using NotesPlayer.Extensions;

namespace NotesPlayer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        class ControlsSet
        {
            public Lazy<Controls.StartupView> StartupView { get; private set; } = new Lazy<Controls.StartupView>();
            public Lazy<Controls.MusicSelectionView> SelectionView { get; private set; } = new Lazy<Controls.MusicSelectionView>();
            public Lazy<Controls.DifficultyView> DifficultyView { get; private set; } = new Lazy<Controls.DifficultyView>(() =>
            {
                var diff = new Controls.DifficultyView();
                diff.FontSize = 40;
                return diff;
            });
            public Lazy<Player> PlayerView { get; } = new Lazy<Player>();
            public Lazy<Controls.ResultView> ResultView { get; } = new Lazy<Controls.ResultView>();
            public Lazy<Controls.RankingView> RankingView { get; } = new Lazy<Controls.RankingView>();

            public void ReadyToPlay()
            {
                StartupView = null;
                SelectionView = null;
                DifficultyView = null;
                GC.Collect(3, GCCollectionMode.Forced, true, true);
            }
        }
        
        AudioPlayer AudioPlayer { get; set; }
        ControlsSet CurrentSet { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = Instance.WindowStyle;
            Loaded += (sender, e) => BeginGaming();
        }

        void BeginGaming()
        {
            if(AudioPlayer != null)
            {
                AudioPlayer.Dispose();
                AudioPlayer = null;
            }

            Navigate.Parameters.Clear();
            CurrentSet = new ControlsSet();
            AudioPlayer = new AudioPlayer("BGM.wav");
            AudioPlayer.Volume = 0.1f;
            AudioPlayer.Play();

            CurrentSet.StartupView.Value.Start += Value_Start;
            SetView(CurrentSet.StartupView.Value);
            FadeIn();
        }

        private void Value_Start(object sender, EventArgs e)
        {
            CurrentSet.StartupView.Value.Start -= Value_Start;
            FadeOut(() =>
            {
                CurrentSet.SelectionView.Value.MusicSelected += Value_MusicSelected;
                SetView(CurrentSet.SelectionView.Value);
                FadeIn();
            });
        }

        private void Value_MusicSelected(object sender, Controls.SelectedEventArgs e)
        {
            Navigate.Parameters[Constants.NavMusicKey] = e.Music;

            CurrentSet.SelectionView.Value.MusicSelected -= Value_MusicSelected;
            FadeOut(() =>
            {
                CurrentSet.DifficultyView.Value.Ready += Value_Ready;
                SetView(CurrentSet.DifficultyView.Value);
                FadeIn();
            });
        }

        private void Value_Ready(object sender, Controls.DifficultyEventArgs e)
        {
            CurrentSet.DifficultyView.Value.Ready -= Value_Ready;

            AudioPlayer.FadeOut(500);
            FadeOut(() =>
            {
                AudioPlayer.Pause();
                CurrentSet.ReadyToPlay();
                SetView(CurrentSet.PlayerView.Value);
                var holder = (MusicHolder)Navigate.Parameters[Constants.NavMusicKey];
                CurrentSet.PlayerView.Value.SetMusic(holder, e.Difficulty);
                CurrentSet.PlayerView.Value.Finished += Value_Finished;
                FadeIn();
            });
        }

        private void Value_Finished(object sender, PlayerFinishedEventArgs e)
        {
            CurrentSet.PlayerView.Value.Finished -= Value_Finished;
            FadeOut(() =>
            {
                int combo = (int)Navigate.Parameters[Constants.NavComboKey];
                int pc = e.Judged.Where((j) => j.Item1 == NoteJudgement.Perfect).Count();
                int gc = e.Judged.Where((j) => j.Item1 == NoteJudgement.Great).Count();
                int hc = e.Judged.Where((j) => j.Item1 == NoteJudgement.Hit).Count();
                int fc = e.Judged.Where((j) => j.Item1 == NoteJudgement.Failed).Count();
                CurrentSet.ResultView.Value.SetScore(e.Score, combo, pc, gc, hc, fc);
                SetView(CurrentSet.ResultView.Value);

                var diff = (Difficulty)Navigate.Parameters[Constants.NavDifficulty];
                var holder = (MusicHolder)Navigate.Parameters[Constants.NavMusicKey];
                var res = new Ranking.Result()
                {
                    Difficulty = diff,
                    SgSongFile = holder.GetMusicPath(diff),
                    Score = e.Score,
                    UserName = (string)Navigate.Parameters[Constants.NavUserNameKey],
                    Registered = DateTime.Now,
                    MaxCombo = combo
                };

                Navigate.Parameters[Constants.NavResult] = res;
                Ranking.Saved.Results.Add(res);
                Ranking.Saved.Save();

                CurrentSet.ResultView.Value.Dismissed += Value_Dismissed;

                FadeIn();
                AudioPlayer.Play();
                AudioPlayer.FadeIn(1500);
            });
        }

        private void Value_Dismissed(object sender, EventArgs e)
        {
            CurrentSet.ResultView.Value.Dismissed -= Value_Dismissed;
            FadeOut(() =>
            {
                CurrentSet.RankingView.Value.Dismissed += Value_Dismissed1;
                SetView(CurrentSet.RankingView.Value);
                FadeIn();
            });
        }

        private void Value_Dismissed1(object sender, EventArgs e)
        {
            CurrentSet.RankingView.Value.Dismissed -= Value_Dismissed1;
            FadeOut(() =>
            {
                BeginGaming();
            });
        }

        void SetView(UIElement Element)
        {
            var grid = (Grid)Content;
            grid.Children.Clear();
            grid.Children.Add(Element);
            grid.Children.Add(FadeR);
        }

        void FadeOut(Action Completed = null)
        {
            if (FadeR.Opacity == 1 && FadeR.Visibility == Visibility.Visible)
                return;

            FadeR.Opacity = 0;
            FadeR.Visibility = Visibility.Visible;
            FadeR.AnimateOpacity(1, 500, (_) => Completed?.Invoke());
        }

        void FadeIn(Action Completed = null)
        {
            if (FadeR.Visibility == Visibility.Hidden)
                return;

            FadeR.Opacity = 1;
            FadeR.Visibility = Visibility.Visible;
            FadeR.AnimateOpacity(0, 500, (element) =>
            {
                element.Visibility = Visibility.Hidden;
                Completed?.Invoke();
            });
        }
    }
}
