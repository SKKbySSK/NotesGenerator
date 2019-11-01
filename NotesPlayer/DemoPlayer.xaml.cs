using SugaEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotesPlayer
{
    /// <summary>
    /// DemoPlayer.xaml の相互作用ロジック
    /// </summary>
    public partial class DemoPlayer : UserControl, INotesDispenser
    {
        public event EventHandler Completed;
        private TimeSpan startedTime;
        private TimeSpan elapsed = TimeSpan.Zero;

        public DemoPlayer()
        {
            InitializeComponent();
            image.Source = BackgroundImageManager.CreateImageSource(BackgroundImageManager.FirstHalfImagePath);
            Dropper.NotesDispenser = this;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            Navigate.Parameters[Constants.NavDurationKey] = TimeSpan.FromMilliseconds((speedS.Value + 5) * 100);
            Completed?.Invoke(this, EventArgs.Empty);
        }

        public void Start()
        {
            startedTime = TimeSpan.FromMilliseconds(Environment.TickCount);
        }

        public IEnumerable<Note> Dispnse(TimeSpan From, TimeSpan To)
        {
            elapsed += To - From;

            if (elapsed.TotalSeconds >= 1)
            {
                elapsed = TimeSpan.Zero;
                return new Note[]
                {
                    new Note(From, 2),
                };
            }
            else
            {
                return new Note[0];
            }
        }

        public TimeSpan GetCurrentTime()
        {
            return TimeSpan.FromMilliseconds(Environment.TickCount) - startedTime;
        }

        private void SpeedS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dropper.Duration = TimeSpan.FromMilliseconds((speedS.Value + 5) * 100);
        }
    }
}
