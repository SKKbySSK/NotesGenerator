using System;
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
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SugaEngine;
using SugaEngine.Export;
using NAudio.Dsp;

namespace NotesGenerator
{
    /// <summary>
    /// Fft.xaml の相互作用ロジック
    /// </summary>
    public partial class Fft : Window
    {
        class CalcParams
        {

            public double LowMagnitude { get; set; } = 2;
            public double LowTake { get; set; } = 30;
            public double LowThreshold { get; set; } = 10;
            public bool LowFFT { get; set; } = true;

            public double HighMagnitude { get; set; } = 2;
            public double HighTake { get; set; } = 50;
            public double HighThreshold { get; set; } = 10;
            public bool HighFFT { get; set; } = true;
        }

        bool Began = false;
        NullOut Output = null;
        string Path;
        IList<Note> Notes;
        FFT.SyncSampleProvider fftProv;
        AudioFileReader afr;
        CalcParams cparam = new CalcParams();

        Queue<System.Numerics.Complex[]> CurrentComplexes = new Queue<System.Numerics.Complex[]>();

        public int CalcRate { get; set; } = 4;

        public Fft(string Path, IList<Note> Notes)
        {
            InitializeComponent();

            this.Path = Path;
            this.Notes = Notes;
        }

        private void BeginB_Click(object sender, RoutedEventArgs e)
        {
            Began = true;
            Output = new NullOut();
            CurrentComplexes.Clear();

            afr = new AudioFileReader(Path);
            ProgressB.Value = 0;
            ProgressB.Maximum = afr.TotalTime.TotalMilliseconds;
            
            BiQuadFilter lowpass = BiQuadFilter.LowPassFilter(afr.WaveFormat.SampleRate, 900, 1);
            Equalizer eq = new Equalizer(new SampleChannel(afr)) { Filter = lowpass, Enabled = true };

            fftProv = new FFT.SyncSampleProvider(eq);
            Output.Init(fftProv);

            fftProv.FftFinished += FftProv_FftFinished;
            Output.BufferUpdated += Output_BufferUpdated;
            Output.PlaybackStopped += Output_PlaybackStopped;

            cparam.LowTake = TakeS.Value;
            Task.Run(() =>
            {
                Output.Play();
            });
        }
        
        private void FftProv_FftFinished(object sender, FFT.FourierEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ProgressB.Value = afr.CurrentTime.TotalMilliseconds));
            CurrentComplexes.Enqueue(e.Samples);

            if(CurrentComplexes.Count > CalcRate)
            {
                System.Numerics.Complex[][] Samples = new System.Numerics.Complex[CalcRate][];
                for (int i = 0; CalcRate > i; i++) Samples[i] = CurrentComplexes.Dequeue();

                if (cparam.LowFFT)
                    ComputeLowFFT(Samples);
                if(cparam.HighFFT)


                CurrentComplexes.Clear();
            }
        }

        void ComputeLowFFT(System.Numerics.Complex[][] Samples)
        {
            List<List<double>> mags = new List<List<double>>();
            foreach(var cons in Samples)
            {
                var taken = cons.Take((int)(cons.Length * cparam.LowTake));
                var list = new List<double>();
                mags.Add(list);
                foreach (var sample in taken)
                {
                    double mag = sample.Magnitude;
                    if (mag >= cparam.LowMagnitude)
                        list.Add(mag);
                }
            }

            List<Note> temp = new List<Note>();

            List<double> avs = new List<double>(temp.Count);
            foreach(var freqm in mags)
            {
                avs.Add(freqm.Sum() / freqm.Count);
            }

            double min = avs.Min();
            double max = avs.Max();

            if (max - min >= cparam.LowThreshold)
            {
                temp.Add(new Note(afr.CurrentTime, new Random((int)((max - min) * 100)).Next(0, 5)));
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (Note n in temp)
                    Notes.Add(n);
            }));
        }

        void ComputeHighFFT(System.Numerics.Complex[][] Samples)
        {
            List<List<double>> mags = new List<List<double>>();
            foreach (var cons in Samples)
            {
                var taken = cons.Take((int)(cons.Length * cparam.HighTake));
                var list = new List<double>();
                mags.Add(list);
                foreach (var sample in taken)
                {
                    double mag = sample.Magnitude;
                    if (mag >= cparam.HighMagnitude)
                        list.Add(mag);
                }
            }

            List<Note> temp = new List<Note>();

            List<double> avs = new List<double>(temp.Count);
            foreach (var freqm in mags)
            {
                avs.Add(freqm.Sum() / freqm.Count);
            }

            double min = avs.Min();
            double max = avs.Max();

            if (max - min >= cparam.HighThreshold)
            {
                temp.Add(new Note(afr.CurrentTime, new Random((int)((max - min) * 100)).Next(0, 5)));
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (Note n in temp)
                    Notes.Add(n);
            }));
        }

        private void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Console.WriteLine("Finished");
            afr.Dispose();
            afr = null;

            fftProv.FftFinished -= FftProv_FftFinished;
            Output.BufferUpdated -= Output_BufferUpdated;
            Output.PlaybackStopped -= Output_PlaybackStopped;
        }

        private void Output_BufferUpdated(object sender, BufferUpdatedEventArgs e)
        {
            if (Began)
            {
                Began = false;
            }
        }
    }
}
