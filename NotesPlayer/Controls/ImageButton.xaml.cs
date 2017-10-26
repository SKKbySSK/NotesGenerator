using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NotesPlayer.Controls
{
    /// <summary>
    /// ImageButton.xaml の相互作用ロジック
    /// </summary>
    public partial class ImageButton : UserControl, INotifyPropertyChanged
    {
        public event EventHandler Clicked;

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropChanged(string Name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));

        public ImageButton()
        {
            InitializeComponent();
            MouseLeftButtonDown += ImageButton_MouseLeftButtonDown;
            MouseLeftButtonUp += ImageButton_MouseLeftButtonUp;
            MouseLeave += ImageButton_MouseLeave;
        }

        private void ImageButton_MouseLeave(object sender, MouseEventArgs e)
        {
            f = false;
        }

        private void ImageButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (f)
                Clicked?.Invoke(this, new EventArgs());
            f = false;
        }

        bool f = false;
        private void ImageButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            f = true;
        }

        bool hidden = false;
        public bool IsLabelHidden
        {
            get { return hidden; }
            set
            {
                hidden = value;
                if (hidden)
                    LabelRow.Height = new GridLength(0);
                else
                    LabelRow.Height = new GridLength(1, GridUnitType.Star);
            }
        }

        public bool EnableAnimation { get; set; } = false;

        string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                LabelV.Content = text;
                OnPropChanged(nameof(Text));
            }
        }

        ImageSource img;
        public ImageSource Image
        {
            get { return img; }
            set
            {
                img = value;
                ImageV.Source = img;
                OnPropChanged(nameof(Image));
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (EnableAnimation)
            {
                ((System.Windows.Media.Animation.Storyboard)Resources["MouseEnterAnim"]).Begin();
            }
        }

        private void userControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (EnableAnimation)
            {
                ((System.Windows.Media.Animation.Storyboard)Resources["MouseLeaveAnim"]).Begin();
            }
        }
    }
}
