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
using System.Windows.Shapes;
using SugaEngine;

namespace NotesGenerator
{
    /// <summary>
    /// UnifyDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class UnifyDialog : Window
    {
        private IList<Note> Notes;

        public UnifyDialog()
        {
            InitializeComponent();
        }

        public UnifyDialog(IList<Note> Notes)
        {
            InitializeComponent();
            this.Notes = Notes;
        }

        private void Time_BeginB_Click(object sender, RoutedEventArgs e)
        {
            if (Notes.Count == 0) return;

            TimeSpan span = TimeSpan.FromMilliseconds(Time_DurS.Value);
            var ordered = Notes.ToList().OrderBy((n) => n.StartingTime).ToList();
            Notes.Clear();

            TimeSpan last = new TimeSpan();
            int c = 0;
            for(int i = 0;ordered.Count > i; i++)
            {
                Note now = ordered[i];
                TimeSpan diff = now.StartingTime - last;
                if (diff <= span && diff.TotalMilliseconds > 0)
                {
                    now.StartingTime = last;
                    c++;
                }
                last = now.StartingTime;
            }
            foreach (var note in ordered)
                Notes.Add(note);

            Time_StateL.Content = $"{c}件処理されました";
        }
    }
}
