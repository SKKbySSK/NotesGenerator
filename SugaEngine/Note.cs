using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugaEngine
{
    public enum NoteMode
    {
        Continuous,
        Tap
    }

    public class Note
    {
        public Note() { }
        private Note(NoteMode Mode, TimeSpan Start, TimeSpan End, int Lane)
        {
            this.Mode = Mode;
            StartingTime = Start;
            EndingTime = End;
            this.Lane = Lane;
        }

        public Note(TimeSpan Start, int Lane) : this(NoteMode.Tap, Start, new TimeSpan(0), Lane) { }

        public Note(TimeSpan Start, TimeSpan End, int Lane) : this(NoteMode.Continuous, Start, End, Lane) { }

        public NoteMode Mode { get; set; }
        public TimeSpan StartingTime { get; set; }
        public TimeSpan EndingTime { get; set; }
        public int Lane { get; set; }
        
        public TimeSpan Duration
        {
            get
            {
                if (Mode == NoteMode.Tap)
                    return new TimeSpan(0);
                else
                    return EndingTime - StartingTime;
            }
        }
    }
}