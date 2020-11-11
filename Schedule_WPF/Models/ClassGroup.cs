using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    class ClassGroup : INotifyPropertyChanged
    {

        public ClassGroup()
        {
            ClassNum = new List<string>();
            ClassDept = new List<string>();
        }

        public List<string> ClassNum { get; set; }
        public List<string> ClassDept { get; set; }

        public void AddEntry(string dept, string num)
        {
            ClassDept.Add(dept);
            ClassNum.Add(num);
        }
        public void DeleteEntry(int index)
        {
            ClassNum.RemoveAt(index);
            ClassDept.RemoveAt(index);
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
