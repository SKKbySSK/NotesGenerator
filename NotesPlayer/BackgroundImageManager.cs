using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NotesPlayer
{
    static class BackgroundImageManager
    {
        public static string SystemDirectory { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System");

        public static string FirstHalfImagePath { get; } = Path.Combine(SystemDirectory, "FirstHalf.png");

        public static string SecondHalfImagePath { get; } = Path.Combine(SystemDirectory, "SecondHalf.png");

        public static ImageSource GetStartupImage()
        {
            return new BitmapImage(new Uri(Path.Combine(SystemDirectory, "Startup.png")));
        }

        public static ImageSource GetMusicSelectionImage()
        {
            return new BitmapImage(new Uri(Path.Combine(SystemDirectory, "Selection.png")));
        }

        public static ImageSource GetDifficultyImage()
        {
            return new BitmapImage(new Uri(Path.Combine(SystemDirectory, "Difficulty.png")));
        }

        public static ImageSource GetScoreImage()
        {
            return new BitmapImage(new Uri(Path.Combine(SystemDirectory, "Score.png")));
        }

        public static ImageSource CreateImageSource(string path)
        {
            return new BitmapImage(new Uri(path));
        }
    }
}
