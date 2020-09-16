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
        private int _maxClasses;
        private int _numClasses;
        private int _maxPrep;
        private int _numPrep;

        public Professors()
        {
            FirstName = "None";
            LastName = "None";
            SRUID = "---";
            profRGB = new RGB_Color(255, 255, 255);
            MaxClasses = 12;
            NumClasses = 0;
            MaxPrep = 4;
            NumPrep = 0;
        }

        // CONSTRUCTOR FOR ADDING PROFESSORS
        public Professors(string profFN, string profLN, string profID)
        {
            FirstName = profFN;
            LastName = profLN;
            SRUID = profID;
            profRGB = new RGB_Color(255, 255, 255);
            MaxClasses = 12;
            NumClasses = 0;
            MaxPrep = 4;
            NumPrep = 0;
        }

        public string FirstName { get { return _FirstName; } set { _FirstName = value; OnPropertyChanged("FirstName"); OnPropertyChanged("FullName"); } }
        public string LastName { get { return _LastName; } set { _LastName = value; OnPropertyChanged("LastName"); OnPropertyChanged("FullName"); } }
        public string SRUID { get { return _SRUID; } set { _SRUID = value; OnPropertyChanged("SRUID"); } }
        public RGB_Color profRGB { get { return _profRGB; } set { _profRGB = value; OnPropertyChanged("profRGB"); OnPropertyChanged("Prof_Color"); } }
        public string FullName { get { return LastName + ", " + FirstName; } }
        public Brush Prof_Color { get { return profRGB.colorBrush2; } }
        public string colorString { get { return profRGB.colorString; } }
        public int MaxClasses { get { return _maxClasses; } set { _maxClasses = value; OnPropertyChanged("MaxClasses"); OnPropertyChanged("classRatio"); } }
        public int MaxPrep { get { return _maxPrep; } set { _maxPrep = value; OnPropertyChanged("MaxPrep"); OnPropertyChanged("prepRatio"); } }
        public int NumClasses { get { return _numClasses; } set { _numClasses = value; OnPropertyChanged("NumClasses"); OnPropertyChanged("classRatio"); } }
        public int NumPrep { get { return _numPrep; } set { _numPrep = value; OnPropertyChanged("NumPrep"); OnPropertyChanged("prepRatio"); } }
        public string classRatio { get { return ("" + NumClasses + " / " + MaxClasses); } }
        public string prepRatio { get { return ("" + NumPrep + " / " + MaxPrep); } }

        public bool isClassOverload()
        {
            if (NumClasses > MaxClasses)
            {
                return true;
            }
            return false;
        }
        public bool isPrepOverload()
        {
            if (NumPrep > MaxPrep)
            {
                return true;
            }
            return false;
        }

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
