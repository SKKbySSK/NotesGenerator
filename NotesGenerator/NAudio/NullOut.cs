using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace NAudio.Wave
{
    class BufferUpdatedEventArgs : EventArgs
    {
        public BufferUpdatedEventArgs(byte[] Buffer, int Offset, int Count)
        {
            this.Buffer = Buffer;
            this.Offset = Offset;
            this.Count = Count;
        }

        public byte[] Buffer { get; }
        public int Count { get; }
        public int Offset { get; }
    }

    class NullOut : IWavePlayer
    {
        IWaveProvider prov;
        
        public int ReadRate { get; set; } = 1024;

        public PlaybackState PlaybackState { get; private set; }

        public float Volume { get; set; } = 1;

        public event EventHandler<BufferUpdatedEventArgs> BufferUpdated;

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public void Dispose()
        {
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
            PlaybackState = PlaybackState.Playing;
            int read = 0;
            byte[] buffer = new byte[ReadRate];
            while ((read = prov.Read(buffer, 0, ReadRate)) > 0)
            {
                BufferUpdated?.Invoke(this, new BufferUpdatedEventArgs(buffer, 0, read));
            }

            PlaybackStopped?.Invoke(this, new StoppedEventArgs());
        }

        public void Stop()
        {
        }
    }
}
