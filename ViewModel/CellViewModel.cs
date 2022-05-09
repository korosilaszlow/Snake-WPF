using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;

namespace SnakeWPF.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        private SolidColorBrush color;

        private int i;
        private int j;

        public int I { get { return i; } set { i = value; OnPropertyChanged(); OnPropertyChanged("Pos"); } }
        public int J { get { return j; } set { j = value; OnPropertyChanged(); OnPropertyChanged("Pos"); } }

        public string Pos { get { return i + " " + j; } }

        public SolidColorBrush Color { get { return color; } set { color = value; OnPropertyChanged(); } }
    }
}
