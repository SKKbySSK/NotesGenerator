using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesPlayer
{
    static class Instance
    {
        public static bool FullAutomatic { get; set; } = false;
        public static bool Skip { get; set; } = false;
        public static bool OverrideRank { get; set; } = false;
        public static string Rank { get; set; } = null;
    }
}
