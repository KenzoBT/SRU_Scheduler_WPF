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
    /// Interaction logic for AddClassRoomDialog.xaml
    /// </summary>
    public partial class AddClassRoomDialog : Window
    {
        public AddClassRoomDialog()
        {
            InitializeComponent();
        }

        private void SubmitData(object sender, RoutedEventArgs e)
        {
            if (allRequiredFields())
            {
                string building = Building_Text.Text;
                int roomNum = Int32.Parse(Number_Text.Text);
                int seats = Int32.Parse(Seats_Text.Text);
                Application.Current.MainWindow.Resources["Set_ClassRoom_Bldg"] = building;
                Application.Current.MainWindow.Resources["Set_ClassRoom_Num"] = roomNum;
                Application.Current.MainWindow.Resources["Set_ClassRoom_Seats"] = seats;
                Application.Current.MainWindow.Resources["Set_ClassRoom_Success"] = true;
                this.Close();
            }

        }

        private bool allRequiredFields()
        {
            bool success = true;
            int tmp;
            // Building name
            if (Building_Text.Text == "")
            {
                Building_Required.Visibility = Visibility.Visible;
                Building_Invalid.Visibility = Visibility.Hidden;
                success = false;
            }
            else
            {
                if (Building_Text.Text.Contains(" "))
                {
                    Building_Required.Visibility = Visibility.Hidden;
                    Building_Invalid.Visibility = Visibility.Visible;
                    success = false;
                }
                else
                {
                    Building_Invalid.Visibility = Visibility.Hidden;
                    Building_Required.Visibility = Visibility.Hidden;
                    Building_Text.Text = Building_Text.Text.ToUpper();
                }
            }
            // Room Number
            if (Number_Text.Text == "")
            {
                Number_Required.Visibility = Visibility.Visible;
                Number_Invalid.Visibility = Visibility.Hidden;
                success = false;
            }
            else if (!Int32.TryParse(Number_Text.Text, out tmp))
            {
                Number_Invalid.Visibility = Visibility.Visible;
                Number_Required.Visibility = Visibility.Hidden;
                success = false;
            }
            else
            {
                Number_Invalid.Visibility = Visibility.Hidden;
                Number_Required.Visibility = Visibility.Hidden;
            }
            // Seats
            if (Seats_Text.Text != "")
            {
                if (!Int32.TryParse(Seats_Text.Text, out tmp))
                {
                    Seats_Invalid.Visibility = Visibility.Visible;
                    success = false;
                }
                else
                {
                    Seats_Invalid.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                Seats_Text.Text = "0";
            }

            return success;
        }
    }
}
