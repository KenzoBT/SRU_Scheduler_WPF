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

namespace Schedule_WPF.Models
{
    /// <summary>
    /// Interaction logic for EditProfessorDialog.xaml
    /// </summary>
    public partial class EditProfessorDialog : Window
    {
        Professors targetProfessor = null;
        string originalSRUID = "";

        ProfessorList professors = (ProfessorList)Application.Current.FindResource("Professor_List_View");

        public EditProfessorDialog(Professors prof)
        {
            InitializeComponent();
            targetProfessor = prof;
            if (targetProfessor != null)
            {
                originalSRUID = targetProfessor.SRUID;
                FirstName.Text = targetProfessor.FirstName;
                LastName.Text = targetProfessor.LastName;
                ID.Text = targetProfessor.SRUID;
                colorPicker.SelectedColor = targetProfessor.profRGB.colorBrush;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (allRequiredFields() && targetProfessor != null)
            {
                targetProfessor.FirstName = FirstName.Text;
                targetProfessor.LastName = LastName.Text;
                targetProfessor.SRUID = ID.Text;
                targetProfessor.profRGB = new RGB_Color(colorPicker.SelectedColor.ToString());
                Close();
            }
        }

        private bool allRequiredFields()
        {
            bool success = true;
            // First Name
            if (FirstName.Text == "")
            {
                FirstName_Required.Visibility = Visibility.Visible;
                success = false;
            }
            else
            {
                FirstName_Required.Visibility = Visibility.Hidden;
            }
            // Last Name
            if (LastName.Text == "")
            {
                LastName_Required.Visibility = Visibility.Visible;
                success = false;
            }
            else
            {
                LastName_Required.Visibility = Visibility.Hidden;
            }
            // SRU ID
            if (ID.Text == "")
            {
                ID_Required.Visibility = Visibility.Visible;
                ID_Invalid.Visibility = Visibility.Hidden;
                success = false;
            }
            else
            {
                if (ID.Text.Length != 9 || ID.Text.Substring(0, 2) != "A0")
                {
                    ID_Invalid.Visibility = Visibility.Visible;
                    ID_Required.Visibility = Visibility.Hidden;
                    success = false;
                }
                else
                {
                    ID_Invalid.Visibility = Visibility.Hidden;
                    ID_Required.Visibility = Visibility.Hidden;
                }
            }
            // Color
            if (colorPicker.SelectedColor.ToString() == "")
            {
                Color_Required.Visibility = Visibility.Visible;
                Color_Invalid.Visibility = Visibility.Hidden;
                success = false;
            }
            else
            {
                RGB_Color tempColor = new RGB_Color(colorPicker.SelectedColor.ToString());
                if (isColorTaken(tempColor))
                {
                    Color_Invalid.Visibility = Visibility.Visible;
                    Color_Required.Visibility = Visibility.Hidden;
                    success = false;
                }
                else
                {
                    Color_Invalid.Visibility = Visibility.Hidden;
                    Color_Required.Visibility = Visibility.Hidden;
                }
            }

            return success;
        }

        public bool isColorTaken(RGB_Color color)
        {
            for (int i = 0; i < professors.Count; i++)
            {
                if (withinColorRange(color, professors[i].profRGB))
                {
                    return true;
                }
            }
            return false;
        }
        public bool withinColorRange(RGB_Color c1, RGB_Color c2)
        {
            int threshold = 65;
            if (Math.Abs(c1.R - c2.R) <= threshold && Math.Abs(c1.G - c2.G) <= threshold && Math.Abs(c1.B - c2.B) <= threshold)
            {
                return true;
            }
            return false;
        }
    }
}
