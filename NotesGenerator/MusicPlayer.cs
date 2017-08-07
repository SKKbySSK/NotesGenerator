using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NotesGenerator
{
    class MusicPlayer : IDisposable
    {
        public event EventHandler PlaybackStateChanged;

        private const int Latency = 50;
        private MediaFoundationReader reader;
        private WasapiOut wasapi = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, Latency);
        private SoundTouch.VarispeedSampleProvider speed;

        public MusicPlayer(string Path)
        {
            reader = new MediaFoundationReader(Path);

            speed = new SoundTouch.VarispeedSampleProvider(new SampleChannel(reader), Latency, new SoundTouch.SoundTouchProfile(false, true));
            speed.PlaybackRate = 0.4f;
            wasapi.Init(speed);
            wasapi.PlaybackStopped += Wasapi_PlaybackStopped;
        }

        private void Wasapi_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlaybackStateChanged?.Invoke(this, e);
        }

        public void Play()
        {
            wasapi.Play();
            PlaybackStateChanged?.Invoke(this, new EventArgs());
        }

        public void Pause()
        {
            wasapi.Pause();
            PlaybackStateChanged?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            reader.Dispose();
            wasapi.Dispose();
            speed.Dispose();
        }

        public float Rate
        {
            get { return speed.PlaybackRate; }
            set { speed.PlaybackRate = value; }
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

        public PlaybackState State
        {
            get { return wasapi.PlaybackState; }
        }
    }
}
