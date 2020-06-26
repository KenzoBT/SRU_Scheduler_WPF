using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Schedule_WPF.Models
{
    public class RGB_Color : INotifyPropertyChanged
    {
        private byte _R;
        private byte _G;
        private byte _B;

        public RGB_Color()
        {
            R = 50;
            G = 50;
            B = 50;
        }
        public RGB_Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte R { get { return _R; } set { _R = value; OnPropertyChanged("R"); } }
        public byte G { get { return _G; } set { _G = value; OnPropertyChanged("G"); } }
        public byte B { get { return _B; } set { _B = value; OnPropertyChanged("B"); } }
        public string colorString { get { return ("" + R + "." + G + "." + B); } }
        public Color colorBrush { get { return Color.FromRgb(R, G, B); } }
        public Brush colorBrush2 { get { return new SolidColorBrush(Color.FromRgb(R, G, B)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
