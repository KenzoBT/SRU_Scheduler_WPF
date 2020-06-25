using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Schedule_WPF.Models
{
    public class RGB_Color
    {
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

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public string colorString { get { return ("" + R + "." + G + "." + B); } }
        public Color colorBrush { get { return Color.FromRgb(R, G, B); } }
        public Brush colorBrush2 { get { return new SolidColorBrush(Color.FromRgb(R, G, B)); } }
    }
}
