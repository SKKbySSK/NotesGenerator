using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;

namespace NotesGenerator.FFT
{
    public class FourierEventArgs : EventArgs
    {
        public FourierEventArgs(System.Numerics.Complex[] Samples, FourierOptions AppliedOptions)
        {
            this.Samples = Samples;
            this.AppliedOptions = AppliedOptions;
        }

        public System.Numerics.Complex[] Samples { get; }
        public FourierOptions AppliedOptions { get; }
    }

    public class SampleProvider : ISampleProvider, IDisposable
    {
        public event EventHandler<FourierEventArgs> FftFinished;

        bool disposing = false;
        Task FftTask;
        ISampleProvider sampleProvider;

        public SampleProvider(ISampleProvider sampleProvider)
        {
            this.sampleProvider = sampleProvider;
            RunTask();
        }

        public int NumberOfSamples { get; set; } = 1024;
        public int ReduceCount { get; set; } = 1000;

        void RunTask()
        {
            FftTask = Task.Run(() =>
            {
                int red = 0;
                while (!disposing)
                {
                    if(red < ReduceCount)
                    {
                        red++;
                    }
                    else
                    {
                        red = 0;
                        lock (FftQueue)
                        {
                            ComputeFFT();
                        }
                    }
                }
            });
        }

        void ComputeFFT()
        {
            if (FftQueue.Count > 0)
            {
                System.Numerics.Complex[] samples = new System.Numerics.Complex[NumberOfSamples];

                int index = 0;
                if (LeftOver != null)
                {
                    int count = Math.Min(LeftOver.Length, NumberOfSamples);
                    for (; count > index; index++)
                        samples[index] = new System.Numerics.Complex(LeftOver[index], 0);

                    if (index + 1 == LeftOver.Length)
                        LeftOver = null;
                    else
                        LeftOver = LeftOver.Skip(index + 1).ToArray();
                }

                if (index + 1 < NumberOfSamples)
                {
                    while (index + 1 < NumberOfSamples)
                    {
                        float[] array = FftQueue.Dequeue();
                        double[] hamming = Window.Hamming(array.Length);

                        array = array.Select((f, i) => (float)(hamming[i] * f)).ToArray();

                        int count = Math.Min(array.Length, NumberOfSamples - index);
                        for (int i = 0; count > i; i++)
                        {
                            samples[index] = new System.Numerics.Complex(array[i], 0);
                            index++;
                        }

                        if (array.Length > count)
                        {
                            int diff = array.Length - count;
                            LeftOver = array.Skip(count).ToArray();
                        }
                    }
                }

                FourierOptions options = FourierOptions.Matlab;
                Fourier.Forward(samples, options);
                FftFinished?.Invoke(this, new FourierEventArgs(samples, options));
            }
        }

        float[] LeftOver { get; set; } = null;
        Queue<float[]> FftQueue { get; } = new Queue<float[]>();
        Queue<System.Numerics.Complex[]> Samples { get; } = new Queue<System.Numerics.Complex[]>();

        public WaveFormat WaveFormat
        {
            get { return sampleProvider.WaveFormat; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            lock (FftQueue)
            {
                FftQueue.Enqueue(buffer);
            }

            return sampleProvider.Read(buffer, offset, count);
        }

        public void Dispose()
        {
            disposing = true;
            FftTask.Wait();
        }
    }
}
