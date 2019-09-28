using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NotesPlayer
{
    static class Instance
    {
        public static bool FullAutomatic { get; set; } = false;
        public static bool Skip { get; set; } = false;
        public static bool OverrideRank { get; set; } = false;
        public static string Rank { get; set; } = null;

        public static WindowStyle WindowStyle { get; set; } = WindowStyle.None;
    }
}
