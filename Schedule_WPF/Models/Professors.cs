using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Schedule_WPF.Models
{
    public class Professors : INotifyPropertyChanged
    {
        private string _FirstName;
        private string _LastName;
        private string _SRUID;
        private RGB_Color _profRGB;

        public Professors()
        {
            FirstName = "None";
            LastName = "None";
            SRUID = "---";
            profRGB = new RGB_Color(255, 255, 255);
        }

        // CONSTRUCTOR FOR ADDING PROFESSORS
        public Professors(string profFN, string profLN, string profID)
        {
            FirstName = profFN;
            LastName = profLN;
            SRUID = profID;
            profRGB = new RGB_Color(255, 255, 255);
        }

        public string FirstName { get { return _FirstName; } set { _FirstName = value; OnPropertyChanged("FirstName"); } }
        public string LastName { get { return _LastName; } set { _LastName = value; OnPropertyChanged("LastName"); } }
        public string SRUID { get { return _SRUID; } set { _SRUID = value; OnPropertyChanged("SRUID"); } }
        public RGB_Color profRGB { get { return _profRGB; } set { _profRGB = value; OnPropertyChanged("profRGB"); } }
        public string FullName { get { return LastName + ", " + FirstName; } }
        public Brush Prof_Color { get { return profRGB.colorBrush2; } }
        public string colorString { get { return profRGB.colorString; } }

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
