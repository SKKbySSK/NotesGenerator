using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace NotesPlayer
{
    class SEPlayer
    {
        public SEPlayer()
        {
            paths.CollectionChanged += Paths_CollectionChanged;
        }

        private void Paths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    players.Add(new ReusablePlayer((string)e.NewItems[0]));
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    var player = players.Where((p) => p.FilePath == (string)e.OldItems[0]).First();
                    players.Remove(player);
                    break;
            }
        }

        public void Play(int Index)
        {
            if(Index > -1 && players.Count > Index)
            {
                var p = players[Index];
                p.Play();
            }
        }

        ObservableCollection<string> paths = new ObservableCollection<string>();
        List<ReusablePlayer> players = new List<ReusablePlayer>();
        public IList<string> SEPaths
        {
            get { return paths; }
        }
    }

    class ReusablePlayer
    {
        WaveFormat format;
        byte[] memory;
        IWavePlayer player;
        public ReusablePlayer(string Path)
        {
            FilePath = Path;

            if (System.IO.File.Exists(Path))
            {
                switch (System.IO.Path.GetExtension(Path.ToLower()))
                {
                    default:
                        MediaFoundationReader mfr = new MediaFoundationReader(Path);
                        mfr.Position = 0;
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        ms.Position = 0;
                        mfr.CopyTo(ms);
                        mfr.Dispose();
                        memory = ms.ToArray();
                        format = mfr.WaveFormat;
                        ms.Dispose();
                        break;
                }
            }
        }

        public string FilePath { get; }

        public void Play()
        {
            if(player != null)
            {
                player.Dispose();
            }

            System.IO.MemoryStream ms = new System.IO.MemoryStream(memory);
            RawSourceWaveStream raws = new RawSourceWaveStream(ms, format);
            player = new DirectSoundOut(10);
            player.Init(raws);
            raws.CurrentTime = new TimeSpan();
            player.Play();
        }

        public void Stop()
        {
            if (player == null) return;
            player.Stop();
        }
    }
}
