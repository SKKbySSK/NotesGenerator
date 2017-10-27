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

namespace NotesPlayer.Controls
{
    /// <summary>
    /// StartupView.xaml の相互作用ロジック
    /// </summary>
    public partial class StartupView : UserControl
    {
        public event EventHandler Start;

        public StartupView()
        {
            InitializeComponent();
        }
        
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Start?.Invoke(this, new EventArgs());
        }
    }
}
