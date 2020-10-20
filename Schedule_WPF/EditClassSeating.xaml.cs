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
    /// Interaction logic for EditClassSeating.xaml
    /// </summary>
    public partial class EditClassSeating : Window
    {
        Classes targetClass;

        public EditClassSeating(Classes target)
        {
            InitializeComponent();
            targetClass = target;
            MaxSeats.Text = target.MaxSeats;
            ProjSeats.Text = target.ProjSeats;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (validFields())
            {
                targetClass.MaxSeats = MaxSeats.Text;
                targetClass.ProjSeats = ProjSeats.Text;
                this.Close();
            }
        }

        private bool validFields()
        {
            bool valid = true;
            int maxNum, projNum;
            if (MaxSeats.Text == "")
            {
                Max_Invalid.Visibility = Visibility.Visible;
                valid = false;
            }
            else
            {
                if (!Int32.TryParse(MaxSeats.Text, out maxNum))
                {
                    Max_Invalid.Visibility = Visibility.Visible;
                    valid = false;
                }
                else
                {
                    Max_Invalid.Visibility = Visibility.Hidden;
                }
            }
            if (ProjSeats.Text == "")
            {
                Proj_Invalid.Visibility = Visibility.Visible;
                valid = false;
            }
            else
            {
                if (!Int32.TryParse(ProjSeats.Text, out projNum))
                {
                    Proj_Invalid.Visibility = Visibility.Visible;
                    valid = false;
                }
                else
                {
                    Proj_Invalid.Visibility = Visibility.Hidden;
                }
            }
            return valid;
        }
    }
}
