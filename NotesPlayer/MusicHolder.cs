using SugaEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesPlayer
{
    public class MusicHolder
    {
        public MusicHolder(string easy, string normal, string hard)
        {
            ParentDirectory = System.IO.Path.GetDirectoryName(easy);
            Easy = SugaEngine.Export.Notes.Deserialize(easy);
            Normal = SugaEngine.Export.Notes.Deserialize(normal);
            Hard = SugaEngine.Export.Notes.Deserialize(hard);

            EasyPath = easy;
            NormalPath = normal;
            HardPath = hard;

            MusicFile = System.IO.Path.Combine(ParentDirectory, Easy.Song.TrimStart('/', '\\'));
        }

        public string ParentDirectory { get; }

        public string EasyPath { get; }

        public string NormalPath { get; }

        public string HardPath { get; }

        public string MusicFile { get; }

        public Music Easy { get; }

        public Music Normal { get; }

        public Music Hard { get; }

        public Music GetMusic(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    return Easy;
                case Difficulty.Normal:
                    return Normal;
                case Difficulty.Hard:
                    return Hard;
            }

            return null;
        }

        public string GetMusicPath(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    return EasyPath;
                case Difficulty.Normal:
                    return NormalPath;
                case Difficulty.Hard:
                    return HardPath;
            }

            return null;
        }
    }
}
