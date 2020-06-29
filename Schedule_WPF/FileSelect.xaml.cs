using Microsoft.Win32;
using System.Windows;


namespace Schedule_WPF
{
    /// <summary>
    /// Interaction logic for FileSelect.xaml
    /// </summary>
    public partial class FileSelect : Window
    {
        public FileSelect()
        {
            InitializeComponent();
        }

        private void Btn_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                Application.Current.Resources["FilePath"] = openFileDialog.FileName;
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }
        }
    }
}
