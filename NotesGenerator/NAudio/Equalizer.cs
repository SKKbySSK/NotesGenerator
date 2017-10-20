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

        public BiQuadFilter Filter { get; set; }

        public bool Enabled { get; set; } = true;

        public int Read(float[] buffer, int offset, int count)
        {
            int read = prov.Read(buffer, offset, count);

            if(Filter != null && Enabled)
            {
                for (int i = offset; count > i; i++)
                {
                    buffer[i] = Filter.Transform(buffer[i]);
                }
            }

            return read;
        }
    }
}
