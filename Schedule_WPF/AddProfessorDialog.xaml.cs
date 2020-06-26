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
    /// Interaction logic for AddProfessorDialog.xaml
    /// </summary>
    public partial class AddProfessorDialog : Window
    {
        public AddProfessorDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (allRequiredFields())
            {
                string first = FirstName.Text;
                string last = LastName.Text;
                string id = ID.Text;
                Application.Current.MainWindow.Resources["Set_Prof_FN"] = first;
                Application.Current.MainWindow.Resources["Set_Prof_LN"] = last;
                Application.Current.MainWindow.Resources["Set_Prof_ID"] = id;
                Application.Current.MainWindow.Resources["Set_Prof_Success"] = true;
                this.Close();
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
                ID_Invalid.Visibility = Visibility.Visible;
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

            return success;
        }
    }
}
