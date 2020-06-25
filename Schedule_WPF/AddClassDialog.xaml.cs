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
            int crn = Int32.Parse(CRN_Text.Text);
            string dpt = Dept_Text.Text;
            int classNum = Int32.Parse(ClassNum_Text.Text);
            int sectNum = Int32.Parse(Section_Text.Text);
            string name = Name_Text.Text;
            int credits = Int32.Parse(Credits_Text.Text);
            string prof = Prof_Text.Text;
            bool online = (bool)Online_Box.IsChecked;
            // Check if something isnt there
            //
            Application.Current.MainWindow.Resources["Set_Class_CRN"] = crn;
            Application.Current.MainWindow.Resources["Set_Class_Dept"] = dpt;
            Application.Current.MainWindow.Resources["Set_Class_Number"] = classNum;
            Application.Current.MainWindow.Resources["Set_Class_Section"] = sectNum;
            Application.Current.MainWindow.Resources["Set_Class_Name"] = name;
            Application.Current.MainWindow.Resources["Set_Class_Credits"] = credits;
            Application.Current.MainWindow.Resources["Set_Class_Professor"] = prof;
            Application.Current.MainWindow.Resources["Set_Class_Online"] = online;
            this.Close();
        }
    }
}
