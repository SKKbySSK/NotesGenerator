using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace NAudio.Wave
{
    class NullOut : IWavePlayer
    {
        IWaveProvider prov;
        bool disposing = false;

        public PlaybackState PlaybackState { get; private set; }

        public float Volume { get; set; } = 1;

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public void Dispose()
        {
            disposing = true;
        }

        public void Init(IWaveProvider waveProvider)
        {
            prov = waveProvider;
        }

        public void Pause()
        {
        }

        public void Play()
        {
            int ms = 100;
            int perRead = prov.WaveFormat.AverageBytesPerSecond / ms;
            byte[] buffer = new byte[prov.WaveFormat.AverageBytesPerSecond];
            Task.Run(() =>
            {
                if (disposing)
                    return;

                while (prov.Read(buffer, 0, perRead) > 0)
                {
                    Task.Delay(ms).Wait();
                }
            });
        }

        public void Stop()
        {
        }
    }
}
