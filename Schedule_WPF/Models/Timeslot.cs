using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    public class Timeslot : INotifyPropertyChanged
    {
        private string _Time;
        private string _EndTime;
        private string _Meridian;

        public Timeslot()
        {
            Time = "--";
            EndTime = "--";
            Meridian = "";
        }

        public Timeslot(string time, string endTime, string meridian)
        {
            Time = time;
            EndTime = endTime;
            Meridian = meridian;
        }

        public string Time { get { return _Time; } set { _Time = value; OnPropertyChanged("Time"); } }
        public string EndTime { get { return _EndTime; } set { _EndTime = value; OnPropertyChanged("EndTime"); } }
        public string Meridian { get { return _Meridian; } set { _Meridian = value; OnPropertyChanged("Meridian"); } }
        public string TimeID { get { return Time.Substring(0, 2); } }
        public string FullTime { get { return Time + " - " + EndTime; } }

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
