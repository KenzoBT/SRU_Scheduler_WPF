using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        private bool _isAppointment;
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
            isAppointment = false;
        }

        public Classes(int crn, string deptName, int classNum, int secNum, string className, int credits,
            string classDay, Timeslot startTime, int seatsTaken, ClassRoom classroom, Professors professor, bool online, bool appointment)
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
            isAppointment = appointment;
        }

        public Classes DeepCopy()
        {
            Classes deepcopy = new Classes(CRN, DeptName, ClassNumber, SectionNumber, ClassName, Credits, ClassDay, StartTime, SeatsTaken, Classroom, Prof, Online, isAppointment);
            return deepcopy;
        }

        public byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(DeptName + ClassNumber + SectionNumber + ClassName + ClassDay + StartTime.FullTime + SeatsTaken + Credits + Online + isAssigned + isAppointment + Prof.FullName + Classroom.ClassID + Prof.Prof_Color.ToString());
                }
                return m.ToArray();
            }
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
        public bool isAppointment { get { return _isAppointment; } set { _isAppointment = value; OnPropertyChanged("isAppointment"); } }
        public Professors Prof { get { return _Prof; } set { _Prof = value; OnPropertyChanged("Prof"); } }
        public ClassRoom Classroom { get { return _Classroom; } set { _Classroom = value; OnPropertyChanged("Classroom"); } }
        public string TextBoxName { get { return DeptName + " " + ClassNumber + " [" + SectionNumber + "]"; } }
        public int SeatsLeft { get { return Classroom.AvailableSeats - SeatsTaken; } }
        public string ToolTipText { get { return "Name: " + ClassName + "\nProfessor: " + Prof.FullName; } }

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
