using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace SugaEngine
{
    public class Music
    {
        public Music()
        {
            Notes = new List<Note>();
        }

        public Music(IList<Note> Notes)
        {
            this.Notes = Notes;
        }

        public string Title { get; set; }
        public int BPM { get; set; }
        public int Delay { get; set; } = 0;
        public IList<Note> Notes { get; set; }
        public string Song { get; set; }
    }
}
