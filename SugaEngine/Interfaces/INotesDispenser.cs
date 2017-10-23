using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugaEngine
{
    public interface INotesDispenser
    {
        IEnumerable<Note> Dispnse(TimeSpan From, TimeSpan To);
        TimeSpan GetCurrentTime();
    }
}
