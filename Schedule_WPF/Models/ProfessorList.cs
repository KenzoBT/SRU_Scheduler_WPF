using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    public class ProfessorList : ObservableCollection<Professors>
    {
        public ProfessorList() : base()
        {
            Add(new Professors("Sam", "Thangiah", "A09999"));
            Add(new Professors("Abdullah", "Wahbeh", "A01223"));
            Add(new Professors("Raed", "Seetan", "A01717"));
            Add(new Professors("Nitin", "Sukhija", "A07819"));
            Add(new Professors("Yili", "Tseng", "A09192"));
            Add(new Professors("Deborah", "Whitfield", "A06486"));
        }
    }
}
