using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Schedule_WPF.Models;
using System.ComponentModel;
using System.IO;
using ClosedXML.Excel;

namespace Schedule_WPF
{
    /// <summary>
    /// Interaction logic for FileSelect.xaml
    /// </summary>
    public partial class FileSelect : Window
    {
        private readonly BackgroundWorker worker_MainLoaded = new BackgroundWorker();
        private readonly BackgroundWorker worker_MainClosed = new BackgroundWorker();

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
                loadingIcon.Visibility = Visibility.Visible;
                btn_OpenFile.Visibility = Visibility.Hidden;
                Application.Current.Resources["FilePath"] = openFileDialog.FileName;
                try
                {
                    using (var excelWorkbook = new XLWorkbook(openFileDialog.FileName))
                    {
                    }
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.ShowDialog();
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Excel file is currently open!\n\nPlease close it before proceeding...");
                    loadingIcon.Visibility = Visibility.Hidden;
                    btn_OpenFile.Visibility = Visibility.Visible;
                }

                /*
                Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();
                
                worker_MainLoaded.DoWork += worker_checkMainLoaded;
                worker_MainLoaded.RunWorkerCompleted += worker_MainFinishedLoading;
                worker_MainLoaded.RunWorkerAsync();

                worker_MainClosed.DoWork += worker_checkMainClosed;
                worker_MainClosed.RunWorkerCompleted += worker_MainFinishedClosing;
                worker_MainClosed.RunWorkerAsync();
                */
            }
        }

        private void ThreadStartingPoint()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
            System.Windows.Threading.Dispatcher.Run();
        }
        private void getConfirmationLoaded()
        {
            bool isReady = MainLoaded.isLoaded;
            while (!isReady)
            {
                //Task.Delay(1);
                isReady = MainLoaded.isLoaded;
            }
        }
        private void getConfirmationClosed()
        {
            bool isClosed = MainLoaded.isClosed;
            while (!isClosed)
            {
                Thread.Sleep(1000);
                isClosed = MainLoaded.isClosed;
            }
        }
        private void worker_checkMainLoaded(object sender, DoWorkEventArgs e)
        {
            getConfirmationLoaded();
        }
        private void worker_checkMainClosed(object sender, DoWorkEventArgs e)
        {
            getConfirmationClosed();
        }
        private void worker_MainFinishedLoading(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
        private void worker_MainFinishedClosing(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }
    }
}
