using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Schedule_WPF.Models
{
    public class Preference : INotifyPropertyChanged
    {
        private string _dept;
        private int _num;
        private string _code;
        private int _sentiment;
        private string _message;

        public Preference(string dpt, int num, string code)
        {
            Dept = dpt;
            ClassNum = num;
            Code = code;
            determineSentiment();
        }

        public string Dept { get { return _dept; } set { _dept = value; OnPropertyChanged("Dept"); } }
        public int ClassNum { get { return _num; } set { _num = value; OnPropertyChanged("ClassNum"); } }
        public string Code { get { return _code; } set { _code = value; OnPropertyChanged("Code"); } }
        public int Sentiment { get { return _sentiment; } set { _sentiment = value; OnPropertyChanged("Sentiment"); } }
        public string Message { get { return _message; } set { _message = value; OnPropertyChanged("Message"); } }

        public void determineSentiment()
        {
            switch (Code)
            {
                case "T":   // Ok with teaching
                    Sentiment = 2;
                    Message = "Has taught class or would like to teach";
                    break;
                case "TPOL": // Ok but prefers online
                    Sentiment = 1;
                    Message = "Taught before but prefer to teach on-line";
                    break;
                case "N":   // Ok but would be new prep
                    Sentiment = 1;
                    Message = "New prep / Could Learn";
                    break;
                case "P":   // Prefer not to teach
                    Sentiment = -1;
                    Message = "Prefer not to teach this class";
                    break;
                case "PF":  // Prefer not to teach in Fall
                    Sentiment = -1;
                    Message = "Prefer not to teach this class in the Fall";
                    break;
                case "PS":  // Prefer not to teach in Spring
                    Sentiment = -1;
                    Message = "Prefer not to teach this class in the Spring";
                    break;
                case "TP":  // Prefer not to teach (Taught before)
                    Sentiment = -1;
                    Message = "Taught class before, prefer not to teach";
                    break;
                case "NP":  // Prefer not to teach (New Prep)
                    Sentiment = -2;
                    Message = "New prep / Prefer not to teach";
                    break;
                case "X":   // Outside of their comfort zone
                    Sentiment = -2;
                    Message = "Class is outside of professor comfort zone";
                    break;
                default:
                    Sentiment = 0;
                    Message = "";
                    break;
            }
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
