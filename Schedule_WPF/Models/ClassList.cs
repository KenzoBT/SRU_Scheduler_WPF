using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    public class ClassList : ObservableCollection<Classes>
    {
        public ClassList() : base()
        {
            Add(new Classes(90210, "CPSC", 246, 01, "Advanced Programming Principles", 3, "MWF", new Timeslot("10:00", "10:50", "AM"), 0, new ClassRoom("ATS", 215, 40), new Professors("Sam", "Thangiah", "A09999"), false));
            Add(new Classes(1078, "CPSC", 217, 02, "Structured & Dynamic Web Programming", 3, "MWF", new Timeslot("11:00", "11:50", "AM"), 0, new ClassRoom("ATS", 347, 40), new Professors("Abdullah", "Wahbeh", "A01223"), false));
            Add(new Classes(2099, "CPSC", 311, 01, "Discrete Computational Structures", 3, "TR", new Timeslot("02:00", "03:15", "PM"), 0, new ClassRoom("ATS", 117, 40), new Professors("Raed", "Seetan", "A01717"), false));
            Add(new Classes(1097, "CPSC", 400, 01, "Computer Networks", 3, "TR", new Timeslot("12:30", "01:45", "PM"), 0, new ClassRoom("ATS", 999, 40), new Professors("Nitin", "Sukhija", "A07819"), false));
            Add(new Classes(10945, "CPSC", 374, 02, "Administration & Security", 3, "", new Timeslot(), 0, new ClassRoom(), new Professors("Yili", "Tseng", "A09192"), false));
            Add(new Classes(16002, "CPSC", 278, 02, "Programming Language & Theory", 3, "", new Timeslot(), 0, new ClassRoom(), new Professors("Deborah", "Whitfield", "A06486"), false));
            Add(new Classes(8501, "CPSC", 300, 01, "Challenges of Computing", 3, "", new Timeslot(), 0, new ClassRoom(), new Professors("Raed", "Seetan", "A01717"), true));
        }

    }
}
