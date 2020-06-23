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
            string first = FirstName.Text;
            string last = LastName.Text;
            string id = ID.Text;
            Application.Current.MainWindow.Resources["Set_Prof_FN"] = first;
            Application.Current.MainWindow.Resources["Set_Prof_LN"] = last;
            Application.Current.MainWindow.Resources["Set_Prof_ID"] = id;
            this.Close();
        }
    }
}
