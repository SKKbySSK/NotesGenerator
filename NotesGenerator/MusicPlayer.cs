﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Dsp;

namespace NotesGenerator
{
    public class MusicPlayer : IDisposable
    {
        public event EventHandler PlaybackStateChanged;

        public const int Latency = 50;

        public event EventHandler<FFT.FourierEventArgs> FftFinished;

        private MediaFoundationReader reader;
        private IWavePlayer output;
        private SoundTouch.VarispeedSampleProvider speed;
        private FFT.SampleProvider fft;

        public MusicPlayer(string Path) : this(Path, new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, Latency))
        {
        }

        public MusicPlayer(string Path, IWavePlayer Player)
        {
            output = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, Latency);
            reader = new MediaFoundationReader(Path);

            BiQuadFilter lowpass = BiQuadFilter.LowPassFilter(reader.WaveFormat.SampleRate, 900, 1);
            Equalizer eq = new Equalizer(new SampleChannel(reader)) { Filter = lowpass };

            fft = new FFT.SampleProvider(eq);
            fft.FftFinished += Fft_FftFinished;

            speed = new SoundTouch.VarispeedSampleProvider(fft, Latency, new SoundTouch.SoundTouchProfile(false, true));
            speed.PlaybackRate = 0.4f;

            output.Init(speed);
            output.PlaybackStopped += Wasapi_PlaybackStopped;
        }

        private void Fft_FftFinished(object sender, FFT.FourierEventArgs e)
        {
            FftFinished?.Invoke(this, e);
        }

        private void Wasapi_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlaybackStateChanged?.Invoke(this, e);
        }

        public void Play()
        {
            output.Play();
            PlaybackStateChanged?.Invoke(this, new EventArgs());
        }

        public void Pause()
        {
            output.Pause();
            PlaybackStateChanged?.Invoke(this, new EventArgs());
        }

        public int NumberOfFftSamples
        {
            get { return fft.NumberOfSamples; }
            set { fft.NumberOfSamples = value; }
        }

        public void Dispose()
        {
            reader.Dispose();
            output.Dispose();
            speed.Dispose();
            fft.Dispose();
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
