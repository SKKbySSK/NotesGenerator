using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Dsp;
using SugaEngine;

namespace NotesPlayer
{
    public class MusicPlayer : IDisposable, INotesDispenser
    {
        public event EventHandler PlaybackStateChanged;

        public const int Latency = 10;
        
        private MediaFoundationReader reader;
        private IWavePlayer output;
        private Music music;
        
        public MusicPlayer(string music, Music notes, IWavePlayer Player)
        {
            this.music = notes;
            output = Player;
            reader = new MediaFoundationReader(music);
            output.Init(reader);
            output.PlaybackStopped += Wasapi_PlaybackStopped;
        }
        
        private void Wasapi_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlaybackStateChanged?.Invoke(this, e);
        }

        public void Play()
        {
            if (Instance.Skip)
            {
                reader.CurrentTime = reader.TotalTime - TimeSpan.FromMilliseconds(1000);
            }
            output.Play();
            PlaybackStateChanged?.Invoke(this, new EventArgs());
        }

        public void Pause()
        {
            output.Pause();
            PlaybackStateChanged?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            reader.Dispose();
            output.Dispose();
        }

        public IEnumerable<Note> Dispnse(TimeSpan From, TimeSpan To)
        {
            List<Note> notes = new List<Note>();

            foreach(Note n in music.Notes)
            {
                if(n.StartingTime >= From && n.StartingTime <= To)
                {
                    notes.Add(n);
                }
            }

            return notes;
        }

        public TimeSpan GetCurrentTime()
        {
            return Position;
        }

        public TimeSpan Position
        {
            get { return reader.CurrentTime; }
            set { reader.CurrentTime = value; }
        }

        public TimeSpan Duration
        {
            get { return reader.TotalTime; }
        }

        public WaveFormat WaveFormat
        {
            get { return reader.WaveFormat; }
        }

        public PlaybackState State
        {
            get { return output.PlaybackState; }
        }
    }
}
