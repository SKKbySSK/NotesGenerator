using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public class Note : System.ComponentModel.INotifyPropertyChanged
    {
        public Note() { }

        public Note(Note Base)
        {
            Mode = Base.Mode;
            StartingTime = Base.StartingTime;
            EndingTime = Base.EndingTime;
            Lane = Base.Lane;
        }

        private Note(NoteMode Mode, TimeSpan Start, TimeSpan End, int Lane)
        {
            this.Mode = Mode;
            StartingTime = Start;
            EndingTime = End;
            this.Lane = Lane;
        }

        public Note(TimeSpan Start, int Lane) : this(NoteMode.Tap, Start, new TimeSpan(0), Lane) { }

        public Note(TimeSpan Start, TimeSpan End, int Lane) : this(NoteMode.Continuous, Start, End, Lane) { }

        NoteMode noteMode = NoteMode.Tap;
        public NoteMode Mode
        {
            get { return noteMode; }
            set
            {
                noteMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mode)));
            }
        }

        TimeSpan start, end;
        public TimeSpan StartingTime
        {
            get { return start; }
            set
            {
                start = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartingTime)));
            }
        }
        public TimeSpan EndingTime
        {
            get { return end; }
            set
            {
                end = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndingTime)));
            }
        }

        int lane;
        public int Lane
        {
            get { return lane; }
            set
            {
                lane = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Lane)));
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}