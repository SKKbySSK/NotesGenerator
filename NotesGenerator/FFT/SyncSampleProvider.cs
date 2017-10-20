using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;

namespace NotesGenerator.FFT
{
    class SyncSampleProvider : ISampleProvider
    {
        ISampleProvider prov;
        public SyncSampleProvider(ISampleProvider SampleProvider)
        {
            prov = SampleProvider;
        }

        public int NumberOfSamples { get; set; } = 1024;
        public int ReduceCount { get; set; } = 1000;
        public event EventHandler<FourierEventArgs> FftFinished;

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
                        if (FftQueue.Count == 0) break;
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
            get { return prov.WaveFormat; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = prov.Read(buffer, offset, count);
            int fftc = 0;
            foreach (var fft in FftQueue)
                fftc += fft.Length;

            fftc += count;
            FftQueue.Enqueue(buffer.Take(read).ToArray());

            int len = LeftOver == null ? 0 : LeftOver.Length;
            if(fftc + len >= NumberOfSamples)
            {
                ComputeFFT();
            }

            return read;
        }
    }
}
