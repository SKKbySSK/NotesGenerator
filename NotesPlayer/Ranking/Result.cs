using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesPlayer.Ranking
{
    public class Result
    {
        public string SgSongFile { get; set; }
        public string UserName { get; set; }
        public int Score { get; set; }
        public Difficulty Difficulty { get; set; }
        public DateTime Registered { get; set; }
    }

    public class ResultCollection : List<Result> { }
}
