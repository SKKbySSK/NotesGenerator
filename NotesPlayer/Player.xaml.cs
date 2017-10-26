﻿using System;
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
using Reactive.Bindings;

namespace NotesPlayer
{
    public class PlayerFinishedEventArgs : EventArgs
    {
        public PlayerFinishedEventArgs(IList<(NoteJudgement, SugaEngine.Note)> Judged, int Score)
        {
            this.Judged = Judged;
            this.Score = Score;
        }

        public IList<(NoteJudgement, SugaEngine.Note)> Judged { get; }
        public int Score { get; }
    }

    /// <summary>
    /// Player.xaml の相互作用ロジック
    /// </summary>
    public partial class Player : UserControl
    {
        MusicPlayer player;

        public Player()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ReactiveProperty<int> Score { get; } = new ReactiveProperty<int>(0);
        private List<(NoteJudgement, SugaEngine.Note)> JudgedList { get; } = new List<(NoteJudgement, SugaEngine.Note)>();
        double actScore { get; set; } = 0;

        public event EventHandler<PlayerFinishedEventArgs> Finished;

        double perfectScore { get; set; } = 0;
        double greatScore { get; set; } = 0;
        double hitScore { get; set; } = 0;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        public void SetMusic(string Directory, SugaEngine.Music Music)
        {
            JudgedList.Clear();
            actScore = 0;
            Score.Value = 0;
            if (player != null)
            {
                player.PlaybackStateChanged -= Player_PlaybackStateChanged;
                player.Dispose();
                player = null;
            }

            double score = (double)Constants.MaximumScore / Music.Notes.Count;
            perfectScore = score * Constants.Perfect;
            greatScore = score * Constants.Great;
            hitScore = score * Constants.Hit;

            try
            {
                player = new MusicPlayer(Directory,
                    Music, new NAudio.Wave.WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, 10));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            player.PlaybackStateChanged += Player_PlaybackStateChanged;
            Dropper.NotesDispenser = player;
            player.Play();
        }

        private void Player_PlaybackStateChanged(object sender, EventArgs e)
        {
            if(e is NAudio.Wave.StoppedEventArgs)
            {
                StopMusic();
                Finished?.Invoke(this, new PlayerFinishedEventArgs(JudgedList, Score.Value));
            }
        }

        public void StopMusic()
        {
            if (player != null)
            {
                player.PlaybackStateChanged -= Player_PlaybackStateChanged;
                player.Dispose();
                player = null;
            }
        }

        private void Dropper_Judged(object sender, JudgementEventArgs e)
        {
            switch (e.Judgement)
            {
                case NoteJudgement.Perfect:
                    actScore += perfectScore;
                    break;
                case NoteJudgement.Great:
                    actScore += greatScore;
                    break;
                case NoteJudgement.Hit:
                    actScore += hitScore;
                    break;
            }
            JudgedList.Add((e.Judgement, e.Note));
            Score.Value = (int)actScore;
        }
    }
}
