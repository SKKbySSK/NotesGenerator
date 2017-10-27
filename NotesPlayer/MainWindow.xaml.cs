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

        ControlsSet CurrentSet { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, e) => BeginGaming();
        }

        void BeginGaming()
        {
            Navigate.Parameters.Clear();
            CurrentSet = new ControlsSet();
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
            Navigate.Parameters[Constants.NavMusicFileKey] = e.SgSongPath;

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

            FadeOut(() =>
            {
                CurrentSet.ReadyToPlay();
                SetView(CurrentSet.PlayerView.Value);
                string path = (string)Navigate.Parameters[Constants.NavMusicFileKey];
                SugaEngine.Music music = (SugaEngine.Music)Navigate.Parameters[Constants.NavMusicKey];
                CurrentSet.PlayerView.Value.SetMusic(System.IO.Path.GetDirectoryName(path), music, e.Difficulty);
                FadeIn(() =>
                {
                    CurrentSet.PlayerView.Value.Finished += Value_Finished;
                });
            });
        }

        private void Value_Finished(object sender, PlayerFinishedEventArgs e)
        {
            CurrentSet.PlayerView.Value.Finished -= Value_Finished;
            FadeOut(() =>
            {
                int combo = (int)Navigate.Parameters[Constants.NavComboKey];
                CurrentSet.ResultView.Value.PerfectCount.Value = e.Judged.Where((j) => j.Item1 == NoteJudgement.Perfect).Count();
                CurrentSet.ResultView.Value.GreatCount.Value = e.Judged.Where((j) => j.Item1 == NoteJudgement.Great).Count();
                CurrentSet.ResultView.Value.HitCount.Value = e.Judged.Where((j) => j.Item1 == NoteJudgement.Hit).Count();
                CurrentSet.ResultView.Value.FailedCount.Value = e.Judged.Where((j) => j.Item1 == NoteJudgement.Failed).Count();
                CurrentSet.ResultView.Value.Score.Value = e.Score;
                CurrentSet.ResultView.Value.Combo.Value = combo;
                SetView(CurrentSet.ResultView.Value);

                var res = new Ranking.Result()
                {
                    Difficulty = (Difficulty)Navigate.Parameters[Constants.NavDifficulty],
                    SgSongFile = System.IO.Path.GetFileName((string)Navigate.Parameters[Constants.NavMusicFileKey]),
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
