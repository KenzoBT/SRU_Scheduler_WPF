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
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (allRequiredFields() && targetProfessor != null)
            {
                targetProfessor.FirstName = FirstName.Text;
                targetProfessor.LastName = LastName.Text;
                targetProfessor.SRUID = ID.Text;
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

            return success;
        }
    }
}
