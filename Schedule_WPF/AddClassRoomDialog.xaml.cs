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
            string building = Building_Text.Text;
            int roomNum = Int32.Parse(Number_Text.Text);
            Application.Current.MainWindow.Resources["Set_ClassRoom_Bldg"] = building;
            Application.Current.MainWindow.Resources["Set_ClassRoom_Num"] = roomNum;
            this.Close();
        }

    }
}
