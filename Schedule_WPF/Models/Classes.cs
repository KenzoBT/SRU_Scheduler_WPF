using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    public class Classes : INotifyPropertyChanged
    {
        private string _DeptName;
        private int _ClassNumber;
        private int _SectionNumber;
        private string _ClassName;
        private string _ClassDay;
        private Timeslot _StartTime;
        private int _SeatsTaken;
        private int _Credits;
        private int _CRN;
        private bool _Online;
        private bool _isAssigned;
        private Professors _Prof;
        private ClassRoom _Classroom;

        public Classes()
        {
            CRN = 0;
            DeptName = "";
            ClassNumber = 0;
            SectionNumber = 0;
            ClassName = "";
            Credits = 0;
            ClassDay = "";
            StartTime = new Timeslot();
            SeatsTaken = 0;
            Classroom = new ClassRoom();
            Prof = new Professors();
            isAssigned = false;
            Online = false;
        }

        public Classes(int crn, string deptName, int classNum, int secNum, string className, int credits,
            string classDay, Timeslot startTime, int seatsTaken, ClassRoom classroom, Professors professor, bool online)
        {
            CRN = crn;
            DeptName = deptName;
            ClassNumber = classNum;
            SectionNumber = secNum;
            ClassName = className;
            Credits = credits;
            ClassDay = classDay;
            StartTime = startTime;
            SeatsTaken = seatsTaken;
            Classroom = classroom;
            Prof = professor;
            isAssigned = false;
            Online = online;
        }

        public string DeptName { get { return _DeptName; } set { _DeptName = value; OnPropertyChanged("DeptName"); } }
        public int ClassNumber { get { return _ClassNumber; } set { _ClassNumber = value; OnPropertyChanged("ClassNumber"); } }
        public int SectionNumber { get { return _SectionNumber; } set { _SectionNumber = value; OnPropertyChanged("SectionNumber"); } }
        public string ClassName { get { return _ClassName; } set { _ClassName = value; OnPropertyChanged("ClassName"); } }
        public string ClassDay { get { return _ClassDay; } set { _ClassDay = value; OnPropertyChanged("ClassDay"); } }
        public Timeslot StartTime { get { return _StartTime; } set { _StartTime = value; OnPropertyChanged("StartTime"); } }
        public int SeatsTaken { get { return _SeatsTaken; } set { _SeatsTaken = value; OnPropertyChanged("SeatsTaken"); } }
        public int Credits { get { return _Credits; } set { _Credits = value; OnPropertyChanged("Credits"); } }
        public int CRN { get { return _CRN; } set { _CRN = value; OnPropertyChanged("CRN"); } }
        public bool Online { get { return _Online; } set { _Online = value; OnPropertyChanged("Online"); } }
        public bool isAssigned { get { return _isAssigned; } set { _isAssigned = value; OnPropertyChanged("isAssigned"); } }
        public Professors Prof { get { return _Prof; } set { _Prof = value; OnPropertyChanged("Prof"); } }
        public ClassRoom Classroom { get { return _Classroom; } set { _Classroom = value; OnPropertyChanged("Classroom"); } }
        public string TextBoxName { get { return DeptName + " " + ClassNumber + " [" + SectionNumber + "]"; } }
        public int SeatsLeft { get { return Classroom.AvailableSeats - SeatsTaken; } }

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
