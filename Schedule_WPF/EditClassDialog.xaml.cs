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
    /// Interaction logic for EditClassDialog.xaml
    /// </summary>
    public partial class EditClassDialog : Window
    {
        Classes targetClass = null;
        int originalCRN = -1;
        bool originalOnline;
        bool originalAssigned;

        public EditClassDialog(Classes _class)
        {
            InitializeComponent();
            targetClass = _class;
            originalCRN = _class.CRN;
            originalOnline = _class.Online;
            originalAssigned = _class.isAssigned;
            ProfessorList profs = (ProfessorList)Application.Current.FindResource("Professor_List_View");
            Prof_Text.ItemsSource = profs;

            // Initialize fields with available data from class
            Classes c1 = _class;
            CRN_Text.Text = _class.CRN.ToString();
            Dept_Text.Text = _class.DeptName;
            ClassNum_Text.Text = _class.ClassNumber.ToString();
            Section_Text.Text = _class.SectionNumber.ToString();
            Name_Text.Text = _class.ClassName;
            Credits_Text.Text = _class.Credits.ToString();
            int profIndex;
            for (profIndex = 0; profIndex < profs.Count; profIndex++)
            {
                if (profs[profIndex].FullName == _class.Prof.FullName)
                {
                    break;
                }
            }
            Prof_Text.SelectedIndex = profIndex;
            Online_Box.IsChecked = _class.Online;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (allRequiredFields() && targetClass != null)
            {
                targetClass.CRN = Int32.Parse(CRN_Text.Text.ToString());
                targetClass.DeptName = Dept_Text.Text;
                targetClass.ClassNumber = Int32.Parse(ClassNum_Text.Text.ToString());
                targetClass.SectionNumber = Int32.Parse(Section_Text.Text.ToString());
                targetClass.ClassName = Name_Text.Text;
                targetClass.Credits = Int32.Parse(Credits_Text.Text.ToString());
                targetClass.Prof = (Professors)Prof_Text.SelectedItem;
                targetClass.Online = (bool)Online_Box.IsChecked;
                if (targetClass.Online == true && originalOnline == false)
                {
                    targetClass.StartTime = new Timeslot();
                    targetClass.Classroom = new ClassRoom();
                    targetClass.ClassDay = "";
                    targetClass.isAssigned = false;
                    EmptyClassList onlineList = (EmptyClassList)Application.Current.FindResource("Online_Classes_List_View");
                    onlineList.Add(targetClass);
                    if (!originalAssigned)
                    {
                        int removeIndex = -1;
                        EmptyClassList unassignedList = (EmptyClassList)Application.Current.FindResource("Unassigned_Classes_List_View");
                        for (int i = 0; i < unassignedList.Count; i++)
                        {
                            if (unassignedList[i].CRN == originalCRN)
                            {
                                removeIndex = i;
                                break;
                            }
                        }
                        unassignedList.RemoveAt(removeIndex);
                    }
                }
                else if(targetClass.Online == false && originalOnline == true)
                {
                    EmptyClassList unassignedList = (EmptyClassList)Application.Current.FindResource("Unassigned_Classes_List_View");
                    unassignedList.Add(targetClass);
                    targetClass.isAssigned = false;
                    int removeIndex = -1;
                    EmptyClassList onlineList = (EmptyClassList)Application.Current.FindResource("Online_Classes_List_View");
                    for (int i = 0; i < onlineList.Count; i++)
                    {
                        if (onlineList[i].CRN == originalCRN)
                        {
                            removeIndex = i;
                            break;
                        }
                    }
                   onlineList.RemoveAt(removeIndex);
                }

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
