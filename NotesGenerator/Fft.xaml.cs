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
            public double Begining { get; set; } = 2;
            public double Magnitude { get; set; } = 0;
            public double Take { get; set; } = 30;
            public double Threshold { get; set; } = 5;
            public int Limit { get; set; } = 3;
            public bool UsingLowPass { get; set; } = false;
        }

        NullOut Output = null;
        string Path;
        IList<Note> Notes;
        FFT.SyncSampleProvider fftProv;
        MediaFoundationReader mfr;
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
            BeginB.IsEnabled = false;
            Output = new NullOut();
            CurrentComplexes.Clear();

            cparam.Take = TakeS.Value;
            cparam.Begining = FftBeginS.Value;
            cparam.Threshold = ThreshS.Value;
            cparam.UsingLowPass = LowPassC.IsChecked ?? false;

            mfr = new MediaFoundationReader(Path);
            ProgressB.Value = 0;
            ProgressB.Maximum = mfr.TotalTime.TotalMilliseconds;
            
            Equalizer eq = new Equalizer(new SampleChannel(mfr));
            eq.AddLowPassFilter(500, 0.1f);
            eq.Enabled = cparam.UsingLowPass;

            fftProv = new FFT.SyncSampleProvider(eq);
            Output.Init(fftProv);

            fftProv.FftFinished += FftProv_FftFinished;
            Output.PlaybackStopped += Output_PlaybackStopped;

            Task.Run(() =>
            {
                Output.Play();
            });
        }
        
        private void FftProv_FftFinished(object sender, FFT.FourierEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ProgressB.Value = mfr.CurrentTime.TotalMilliseconds));

            if(mfr.CurrentTime.TotalSeconds >= cparam.Begining)
            {
                CurrentComplexes.Enqueue(e.Samples);

                if (CurrentComplexes.Count > CalcRate)
                {
                    System.Numerics.Complex[][] Samples = new System.Numerics.Complex[CalcRate][];
                    for (int i = 0; CalcRate > i; i++) Samples[i] = CurrentComplexes.Dequeue();

                    if (cparam.UsingLowPass)
                        ComputeFFTInLowPassMode(Samples);
                    else
                        ComputeFFTInDifferencesMode(Samples);

                    CurrentComplexes.Clear();
                }
            }
        }

        void ComputeFFTInDifferencesMode(System.Numerics.Complex[][] Samples)
        {
            foreach(var cons in Samples)
            {
                var taken = cons.Take((int)(cons.Length * cparam.Take));

                double tmax = taken.Max((comp) => comp.Magnitude);

                int clim = 0;
                bool add = true;
                foreach (var sample in taken)
                {
                    double mag = sample.Magnitude;
                    if (mag >= cparam.Magnitude)
                    {
                        if(tmax - mag < cparam.Threshold)
                        {
                            if (clim < cparam.Limit)
                                clim++;
                            else
                            {
                                add = false;
                                break;
                            }
                        }
                    }
                }

                if (add)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Notes.Add(new Note(mfr.CurrentTime, new Random((int)((tmax) * 100)).Next(0, 5)));
                    }));
                    return;
                }
            }
        }

        TimeSpan CalculateTime(long Position, int Difference)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds((Position + Difference) / mfr.WaveFormat.SampleRate);
            return timeSpan;
        }

        void ComputeFFTInLowPassMode(System.Numerics.Complex[][] Samples)
        {
            double max = 0, sec = -1;
            int maxind = 0;

            int i = 0;
            foreach (var cons in Samples)
            {
                var taken = cons.Take((int)(cons.Length * cparam.Take));
                var ordered = taken.OrderByDescending((comp) => comp.Magnitude);

                double cmax = ordered.First().Magnitude;
                if (cmax > max)
                {
                    sec = max;
                    max = cmax;
                    maxind = i;
                }
            }

            if(max >= cparam.Threshold && max - sec >= cparam.Threshold)
            {
                int samplesc = Samples.Skip(maxind + 1).Sum((sample) => sample.Length);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Notes.Add(new Note(mfr.CurrentTime, new Random((int)((max) * 100)).Next(0, 5)));
                }));
            }
        }

        private void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Console.WriteLine("Finished");
                mfr.Dispose();
                mfr = null;
                BeginB.IsEnabled = true;

                fftProv.FftFinished -= FftProv_FftFinished;
                Output.PlaybackStopped -= Output_PlaybackStopped;
            }));
        }
    }
}
