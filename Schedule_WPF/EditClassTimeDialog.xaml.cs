using Schedule_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Schedule_WPF
{
    /// <summary>
    /// Interaction logic for EditClassTimeDialog.xaml
    /// </summary>
    public partial class EditClassTimeDialog : Window
    {
        Timeslot[] times_MWF = { new Timeslot("08:00", "08:50", "AM"), new Timeslot("09:00", "09:50", "AM"), new Timeslot("10:00", "10:50", "AM"), new Timeslot("11:00", "11:50", "AM"), new Timeslot("12:00", "12:50", "PM"), new Timeslot("01:00", "01:50", "PM"), new Timeslot("02:00", "02:50", "PM"), new Timeslot("03:00", "03:50", "PM"), new Timeslot("04:00", "04:50", "PM"), new Timeslot("05:00", "05:50", "PM"), new Timeslot("06:00", "06:50", "PM") };
        Timeslot[] times_TR = { new Timeslot("08:00", "09:15", "AM"), new Timeslot("09:30", "10:45", "AM"), new Timeslot("11:00", "12:15", "AM"), new Timeslot("12:30", "01:45", "PM"), new Timeslot("02:00", "03:15", "PM"), new Timeslot("03:30", "04:45", "PM"), new Timeslot("05:00", "06:15", "PM") };
        ClassList classList = (ClassList)Application.Current.FindResource("Classes_List_View");
        Classes targetClass;
        Timeslot selectedTime;
        string days;

        public EditClassTimeDialog(Classes target)
        {
            InitializeComponent();
            targetClass = target;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TimeComboBox.SelectedIndex == 0) // MWF
            {
                TimeListComboBox.ItemsSource = times_MWF;
            }
            else // TR
            {
                TimeListComboBox.ItemsSource = times_TR;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TimeComboBox.Text == "MWF")
            {
                selectedTime = times_MWF[TimeListComboBox.SelectedIndex];
                days = "MWF";
            }
            else
            {
                selectedTime = times_TR[TimeListComboBox.SelectedIndex];
                days = "TR";
            }

            if (validateTime())
            {
                targetClass.StartTime = selectedTime;
                targetClass.ClassDay = days;
                this.Close();
            }
        }

        private bool validateTime()
        {
            bool valid = true;
            bool timeConflict = false;
            // if any combobox is empty = invalid
            if (TimeComboBox.Text == "" || TimeListComboBox.Text == "")
            {
                valid = false;
                Time_Required.Visibility = Visibility.Visible;
                Time_Invalid.Visibility = Visibility.Hidden;
            }
            else
            {
                // go through classlist and see if the professor is assigned at the same time
                for (int i = 0; i < classList.Count; i++)
                {
                    if (classList[i].Prof.FullName == targetClass.Prof.FullName)
                    {
                        //MessageBox.Show("Prof Hit: " + targetClass.Prof.LastName);
                        if (classList[i].StartTime.FullTime == selectedTime.FullTime)
                        {
                            timeConflict = true;
                            break;
                        }
                        //MessageBox.Show("No Match\nTarget: " + selectedTime.FullTime + "\nClassList: " + classList[i].StartTime.FullTime);
                    }
                }
                if (timeConflict)
                {
                    Time_Required.Visibility = Visibility.Hidden;
                    Time_Invalid.Visibility = Visibility.Visible;
                    valid = false;
                }
                else
                {
                    Time_Required.Visibility = Visibility.Hidden;
                    Time_Invalid.Visibility = Visibility.Hidden;
                }
            }
            return valid;
        }
    }
}
