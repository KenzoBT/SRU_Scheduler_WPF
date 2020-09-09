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
    /// Interaction logic for EditNotesDialog.xaml
    /// </summary>
    public partial class EditNotesDialog : Window
    {
        Classes c1 = null;

        public EditNotesDialog(Classes _class)
        {
            InitializeComponent();
            c1 = _class;
            // Initialize fields with available data from class
            ClassNotes.Text = c1.Notes;
            SectionNotes.Text = c1.SectionNotes;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            c1.Notes = ClassNotes.Text;
            c1.SectionNotes = SectionNotes.Text;
            // Close the window
            this.Close();
        }
    }
}
