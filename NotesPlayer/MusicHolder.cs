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
            ParentDirectory = System.IO.Path.GetDirectoryName(easy ?? normal ?? hard);
            Easy = easy != null ? SugaEngine.Export.Notes.Deserialize(easy) : null;
            Normal = normal != null ? SugaEngine.Export.Notes.Deserialize(normal) : null;
            Hard = hard != null ? SugaEngine.Export.Notes.Deserialize(hard) : null;

            EasyPath = easy;
            NormalPath = normal;
            HardPath = hard;

            var music = Easy ?? Normal ?? Hard;
            Title = music.Title;
            MusicFile = System.IO.Path.Combine(ParentDirectory, music.Song.TrimStart('/', '\\'));

            var firstHalf = System.IO.Path.Combine(ParentDirectory, "first.png");
            var secondHalf = System.IO.Path.Combine(ParentDirectory, "second.png");

            if (System.IO.File.Exists(firstHalf))
            {
                FirstHalfImage = firstHalf;
            }
            else
            {
                FirstHalfImage = BackgroundImageManager.FirstHalfImagePath;
            }

            if (System.IO.File.Exists(secondHalf))
            {
                SecondHalfImage = secondHalf;
            }
            else
            {
                SecondHalfImage = BackgroundImageManager.SecondHalfImagePath;
            }
        }

        public string ParentDirectory { get; }

        public string EasyPath { get; }

        public string NormalPath { get; }

        public string HardPath { get; }

        public string MusicFile { get; }

        public string Title { get; }

        public string FirstHalfImage { get; }

        public string SecondHalfImage { get; }

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
