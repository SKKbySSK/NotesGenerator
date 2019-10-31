using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesPlayer
{
    public static class Constants
    {
        public const double Perfect = 1;
        public const double Great = 0.8;
        public const double Hit = 0.5;
        public const double Failed = 0;
        public const double PerfectDiff = 0.08;
        public const double GreatDiff = 0.16;
        public const double HitDiff = 0.24;
        public const int MaximumScore = 150000;
        public const int RankS = 120000;
        public const int RankA = 80000;
        public const int RankB = 60000;
        public const int RankC = 30000;

        public const string NavMusicKey = "MUSIC";
        public const string NavUserNameKey = "USER_NAME";
        public const string NavDifficulty = "DIFFICULT";
        public const string NavResult = "RESULT";
        public const string NavComboKey = "COMBO";

        public const string NormalSgSongName = "_Normal.sgsong";
        public const string EasySgSongName = "_Easy.sgsong";

        public const string RankindFileName = "Rank.sgr";
    }
}