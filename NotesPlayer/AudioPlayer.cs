using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NotesPlayer
{
    class LoopStream : WaveStream
    {
        WaveStream sourceStream;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }

        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        /// <summary>
        /// LoopStream simply returns
        /// </summary>
        public override long Length
        {
            get { return sourceStream.Length; }
        }

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }

    class AudioPlayer : IDisposable
    {
        WaveOut wout;
        MediaFoundationReader reader;
        FadeInOutSampleProvider fade;
        WaveChannel32 vol;

        public AudioPlayer(string Path)
        {
            reader = new MediaFoundationReader(Path);
            Init();
        }

        void Init()
        {
            reader.Position = 0;
            wout = new WaveOut();

            vol = new WaveChannel32(reader);
            fade = new FadeInOutSampleProvider(new WaveToSampleProvider(vol));
            
            wout.Init(fade);
        }

        public void FadeIn(double Duration) => fade.BeginFadeIn(Duration);
        public void FadeOut(double Duration) => fade.BeginFadeOut(Duration);

        public float Volume
        {
            get { return vol.Volume; }
            set { vol.Volume = value; }
        }

        public void Play()
        {
            wout.Play();
        }

        public void Play(TimeSpan From, TimeSpan To)
        {
            reader.CurrentTime = From;
            FadeIn(500);
            wout.Play();
            Task.Run(() =>
            {
                while (reader.CurrentTime < To) ;
                FadeOut(500);
            });
        }

        public void Pause()
        {
            wout.Pause();
        }

        public void Dispose()
        {
            wout.Dispose();
            wout = null;
            reader.Dispose();
            reader = null;
        }
    }
}
