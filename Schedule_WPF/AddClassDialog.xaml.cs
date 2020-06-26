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
    /// Interaction logic for AddClassDialog.xaml
    /// </summary>
    public partial class AddClassDialog : Window
    {
        public AddClassDialog()
        {
            InitializeComponent();
            Prof_Text.ItemsSource = (IEnumerable<Professors>)Application.Current.FindResource("Professor_List_View");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // If successfull data entry, communicate data back to MainWindow
            if (allRequiredFields())
            {
                // Get information from input fields
                int crn = Int32.Parse(CRN_Text.Text);
                string dpt = Dept_Text.Text;
                int classNum = Int32.Parse(ClassNum_Text.Text);
                int sectNum = Int32.Parse(Section_Text.Text);
                string name = Name_Text.Text;
                int credits = Int32.Parse(Credits_Text.Text);
                Professors professor = (Professors)Prof_Text.SelectedItem;
                string profname;
                if (professor != null)
                {
                    profname = professor.FullName;
                }
                else
                {
                    profname = "";
                }
                bool online = (bool)Online_Box.IsChecked;

                // Store the information in the appropriate variables inside MainWindow
                Application.Current.MainWindow.Resources["Set_Class_Success"] = true;
                Application.Current.MainWindow.Resources["Set_Class_CRN"] = crn;
                Application.Current.MainWindow.Resources["Set_Class_Dept"] = dpt;
                Application.Current.MainWindow.Resources["Set_Class_Number"] = classNum;
                Application.Current.MainWindow.Resources["Set_Class_Section"] = sectNum;
                Application.Current.MainWindow.Resources["Set_Class_Name"] = name;
                Application.Current.MainWindow.Resources["Set_Class_Credits"] = credits;
                Application.Current.MainWindow.Resources["Set_Class_Professor"] = profname;
                Application.Current.MainWindow.Resources["Set_Class_Online"] = online;

                // Close the window
                this.Close();
            }
        }

        private bool allRequiredFields()
        {
            bool success = true;
            int tmp;
            // Class CRN
            if (CRN_Text.Text == "")
            {
                CRN_Required.Visibility = Visibility.Visible;
                CRN_Invalid.Visibility = Visibility.Hidden;
                success = false;
            }
            else if (!Int32.TryParse(CRN_Text.Text, out tmp))
            {
                CRN_Invalid.Visibility = Visibility.Visible;
                CRN_Required.Visibility = Visibility.Hidden;
                success = false;
            }
            else
            {
                CRN_Invalid.Visibility = Visibility.Hidden;
                CRN_Required.Visibility = Visibility.Hidden;
            }
            // Department Name
            if (Dept_Text.Text == "")
            {
                Dept_Required.Visibility = Visibility.Visible;
                Dept_Invalid.Visibility = Visibility.Hidden;
                success = false;
            }
            else
            {
                if (Dept_Text.Text.Length != 4)
                {
                    Dept_Required.Visibility = Visibility.Hidden;
                    Dept_Invalid.Visibility = Visibility.Visible;
                    success = false;
                }
                else
                {
                    Dept_Required.Visibility = Visibility.Hidden;
                    Dept_Invalid.Visibility = Visibility.Hidden;
                    Dept_Text.Text = Dept_Text.Text.ToUpper();
                }
            }
            // Class Number
            if (ClassNum_Text.Text == "")
            {
                Number_Required.Visibility = Visibility.Visible;
                Number_Invalid.Visibility = Visibility.Hidden;

                success = false;
            }
            else if (!Int32.TryParse(ClassNum_Text.Text, out tmp))
            {
                Number_Invalid.Visibility = Visibility.Visible;
                Number_Required.Visibility = Visibility.Hidden;
                success = false;
            }
            else
            {
                if (tmp > 99 && tmp < 1000)
                {
                    Number_Invalid.Visibility = Visibility.Hidden;
                    Number_Required.Visibility = Visibility.Hidden;
                }
                else
                {
                    Number_Invalid.Visibility = Visibility.Visible;
                    Number_Required.Visibility = Visibility.Hidden;
                    success = false;
                }
            }

            ////// Class Section
            if (Section_Text.Text == "")
            {
                Section_Required.Visibility = Visibility.Visible;
                Section_Invalid.Visibility = Visibility.Hidden;
                success = false;
            }
            else if (!Int32.TryParse(Section_Text.Text, out tmp))
            {
                Section_Invalid.Visibility = Visibility.Visible;
                Section_Required.Visibility = Visibility.Hidden;
                success = false;
            }
            else
            {
                Section_Invalid.Visibility = Visibility.Hidden;
                Section_Required.Visibility = Visibility.Hidden;
            }
            // Class Name
            if (Name_Text.Text == "")
            {
                Name_Required.Visibility = Visibility.Visible;
                success = false;
            }
            else
            {
                Name_Required.Visibility = Visibility.Hidden;
            }
            // Class Credits
            if (Credits_Text.Text == "")
            {
                Credits_Required.Visibility = Visibility.Visible;
                Credits_Invalid.Visibility = Visibility.Hidden;
                success = false;
            }
            else if (!Int32.TryParse(Credits_Text.Text, out tmp))
            {
                Credits_Invalid.Visibility = Visibility.Visible;
                Credits_Required.Visibility = Visibility.Hidden;
                success = false;
            }
            else
            {
                if (tmp > 0 && tmp < 10)
                {
                    Credits_Invalid.Visibility = Visibility.Hidden;
                    Credits_Required.Visibility = Visibility.Hidden;
                }
                else
                {
                    Credits_Invalid.Visibility = Visibility.Visible;
                    Credits_Required.Visibility = Visibility.Hidden;
                    success = false;
                }
            }

            return success;
        }

    }
}
