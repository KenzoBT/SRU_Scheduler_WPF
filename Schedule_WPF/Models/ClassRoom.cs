using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    public class ClassRoom : INotifyPropertyChanged
    {
        private string _Location;
        private int _RoomNum;
        private int _AvailableSeats;

        public ClassRoom()
        {
            RoomNum = 0;
            AvailableSeats = 0;
            Location = "N/A";
        }

        public ClassRoom(string offCampus_ID, int seatCapacity)
        {
            RoomNum = 0;
            AvailableSeats = seatCapacity;
            Location = offCampus_ID;
        }

        public ClassRoom(string bldg, int num, int seatCapacity)
        {
            RoomNum = num;
            AvailableSeats = seatCapacity;
            Location = bldg;
        }

        public string Location { get { return _Location; } set { _Location = value; OnPropertyChanged("Location"); } }
        public int RoomNum { get { return _RoomNum; } set { _RoomNum = value; OnPropertyChanged("RoomNum"); } }
        public int AvailableSeats { get { return _AvailableSeats; } set { _AvailableSeats = value; OnPropertyChanged("AvailableSeats"); } }
        public string ClassID { get { return Location + RoomNum; } }

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
