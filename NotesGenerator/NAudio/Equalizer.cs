using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Dsp;

namespace NAudio.Wave
{
    class Equalizer : ISampleProvider
    {
        ISampleProvider prov;
        public Equalizer(ISampleProvider Provider)
        {
            prov = Provider;
        }

        public WaveFormat WaveFormat
        {
            get { return prov.WaveFormat; }
        }

        public List<BiQuadFilter> Filters { get; } = new List<BiQuadFilter>();

        public void AddHighPassFilter(float Cutoff, float Q)
        {
            Filters.Add(BiQuadFilter.HighPassFilter(WaveFormat.SampleRate, Cutoff, Q));
        }

        public void AddLowPassFilter(float Cutoff, float Q)
        {
            Filters.Add(BiQuadFilter.LowPassFilter(WaveFormat.SampleRate, Cutoff, Q));
        }

        public bool Enabled { get; set; } = true;

        public bool UseMax { get; set; } = false;

        public int Read(float[] buffer, int offset, int count)
        {
            int read = prov.Read(buffer, offset, count);

            if(Filters.Count > 0 && Enabled)
            {
                lock (Filters)
                {
                    for (int i = offset; count > i; i++)
                    {
                        float act = buffer[i];

                        var e = Filters.Select((filter) => filter.Transform(act));

                        float apply = 0;
                        foreach(float val in e)
                        {
                            if (Math.Abs(val) > Math.Abs(apply))
                                apply = val;
                        }

                        buffer[i] = apply;
                    }
                }
            }

            return read;
        }
    }
}
