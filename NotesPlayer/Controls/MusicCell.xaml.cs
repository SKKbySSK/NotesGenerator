﻿using System;
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
    /// MusicCell.xaml の相互作用ロジック
    /// </summary>
    public partial class MusicCell : UserControl
    {
        private MusicHolder music;

        public MusicCell()
        {
            InitializeComponent();
        }

        public MusicHolder Music 
        {
            get => music;
            set 
            {
                music = value;
                label.Content = value?.Title;
            }
        }
    }
}
