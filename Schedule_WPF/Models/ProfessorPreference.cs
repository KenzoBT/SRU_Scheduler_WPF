using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    public class ProfessorPreference : INotifyPropertyChanged
    {
        private string _professorID;
        private List<Preference> _preferenceList;

        public ProfessorPreference(string profID)
        {
            _professorID = profID;
            _preferenceList = new List<Preference>();
        }

        public string ProfessorID { get { return _professorID; } set { _professorID = value; OnPropertyChanged("ProfessorID"); } }
        public List<Preference> PreferenceList { get { return _preferenceList; } }

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
