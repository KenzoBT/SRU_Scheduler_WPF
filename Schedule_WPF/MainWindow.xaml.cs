using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Schedule_WPF.Properties;
using System.ComponentModel;
using Schedule_WPF.Models;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Schedule_WPF
{
    /// <summary>
    /// Main Window of the program
    /// </summary>
    public partial class MainWindow : Window
    {
        ////////////// GLOBAL VARIABLES ////////////////
        Timeslot[] times_MWF = { new Timeslot("08:00", "08:50", "AM"), new Timeslot("09:00", "09:50", "AM"), new Timeslot("10:00", "10:50", "AM"), new Timeslot("11:00", "11:50", "AM"), new Timeslot("12:00", "12:50", "PM"), new Timeslot("01:00", "01:50", "PM"), new Timeslot("02:00", "02:50", "PM"), new Timeslot("03:00", "03:50", "PM"), new Timeslot("04:00", "04:50", "PM"), new Timeslot("05:00", "05:50", "PM"), new Timeslot("06:00", "06:50", "PM") };
        Timeslot[] times_TR = { new Timeslot("08:00", "09:15", "AM"), new Timeslot("09:30", "10:45", "AM"), new Timeslot("11:00", "12:15", "AM"), new Timeslot("12:30", "01:45", "PM"), new Timeslot("02:00", "03:15", "PM"), new Timeslot("03:30", "04:45", "PM"), new Timeslot("05:00", "06:15", "PM") };
        ClassRoomList classrooms = (ClassRoomList)Application.Current.FindResource("ClassRoom_List_View");
        ProfessorList professors = (ProfessorList)Application.Current.FindResource("Professor_List_View");
        RGB_Color[] colorPalette = { new RGB_Color(244, 67, 54), new RGB_Color(156, 39, 176), new RGB_Color(63, 81, 181), new RGB_Color(3, 169, 244), new RGB_Color(0, 150, 136), new RGB_Color(139, 195, 74), new RGB_Color(255, 235, 59), new RGB_Color(255, 152, 0), new RGB_Color(233, 30, 99), new RGB_Color(103, 58, 183), new RGB_Color(33, 150, 243), new RGB_Color(0, 188, 212), new RGB_Color(76, 175, 80), new RGB_Color(205, 220, 57), new RGB_Color(255, 193, 7), new RGB_Color(255, 87, 34) };
        Pairs colorPairs;
        ClassList classList = (ClassList)Application.Current.FindResource("Classes_List_View");
        ClassList unassignedClasses = (ClassList)Application.Current.FindResource("Unassigned_Classes_List_View");
        ClassList onlineClasses = (ClassList)Application.Current.FindResource("Online_Classes_List_View");

        ////////////// START OF EXECUTION ////////////////
        public MainWindow()
        {
            InitializeComponent();
            string filePath = Application.Current.Resources["FilePath"].ToString();

            // Read from excel to get data
            ReadExcel(filePath);
            // Assign professor colors 
            AssignProfColors();
            // Draw timetables for MWF / TR
            DrawTimeTables();
            // Fill unassigned / online class lists
            FillUnassigned();
            // Bind data to corresponding gui controls
            BindData();

            Helper.CloseUniqueWindow<FileSelect>();
        }

        private void ReadExcel(string file)
        {
            int sheetIndex = 1;
            Excel.Application App = new Excel.Application();
            App.Visible = false;
            Excel.Workbook Workbook = App.Workbooks.Open(@file);
            Excel.Worksheet Worksheet = Workbook.Sheets[sheetIndex];
            Excel.Range Range = Worksheet.UsedRange;
            int rowCount = Range.Rows.Count;
            int colCount = Range.Columns.Count;

            // Create Professors
            int sruid_indexer = 0;
            for (int i = 2; i <= rowCount; i++)
            {
                string fullName, lastName, firstName, SRUID;
                if (Range.Cells[i, 22] != null && Range.Cells[i, 22].Value2 != null)
                {
                    if (Range.Cells[i, 22].Value2 != "" && Range.Cells[i, 22].Value2.Contains(","))
                    {
                        fullName = Range.Cells[i, 22].Value2.ToString();
                        bool professorFound = false;
                        for (int n = 0; n < professors.Count; n++)
                        {
                            if (professors[n].FullName == fullName)
                            {
                                professorFound = true;
                                break;
                            }
                        }
                        if (!professorFound)
                        {
                            lastName = fullName.Split(',')[0];
                            firstName = fullName.Split(',')[1].Remove(0, 1);
                            if (Range.Cells[i, 23] != null && Range.Cells[i, 23].Value2 != null && Range.Cells[i, 23].Value2 != "" && Range.Cells[i, 23].Value2.ToString().Length == 9)
                            {
                                SRUID = Range.Cells[i, 23].Value2.ToString();
                                //MessageBox.Show("Name: " + fullName + "\nID: " + SRUID);
                            }
                            else
                            {
                                SRUID = "A0" + sruid_indexer;
                                sruid_indexer++;
                                //MessageBox.Show("Name: " + fullName + "\nID: " + SRUID);
                            }
                            professors.Add(new Professors(firstName, lastName, SRUID));
                        }
                    }
                }
            }

            // Create Classrooms
            for (int i = 2; i <= rowCount; i++)
            {
                string bldg;
                int room = -1;
                int capacity = 0;
                if (Range.Cells[i, 20] != null && Range.Cells[i, 20].Value2 != null && Range.Cells[i, 20].Value2 != "")
                {
                    bldg = Range.Cells[i, 20].Value2.ToString().ToUpper();
                    if (bldg != "WEB" && !bldg.Contains("APPT"))
                    {
                        int parseResult = 0;
                        if (Range.Cells[i, 21] != null && Range.Cells[i, 21].Value2 != null && int.TryParse(Range.Cells[i, 21].Value2.ToString(), out parseResult))
                        {
                            room = parseResult;
                            // Implement capacity -- setting default to 50
                            capacity = 50;
                        }
                        bool classroomFound = false;
                        for (int n = 0; n < classrooms.Count; n++)
                        {
                            if (classrooms[n].ClassID == (bldg + room))
                            {
                                classroomFound = true;
                                break;
                            }
                        }
                        if (!classroomFound)
                        {
                            classrooms.Add(new ClassRoom(bldg, room, capacity));
                            //MessageBox.Show("Added: " + bldg + " " + room);
                        }
                    }
                }
            }

            int duplicate_CRN_indexer = -1;
            List<int> CRN_List = new List<int>();

            // Create Classes
            for (int i = 2; i <= rowCount; i++)
            {
                int CRN;
                int ClassNum = -1;
                int Section = -1;
                int Credits = 0;
                int SeatsTaken = 0;
                string Dept = "";
                string ClassName = "";
                string ClassDay = "";
                Professors prof = new Professors();
                ClassRoom classroom = new ClassRoom();
                Timeslot time = new Timeslot();
                bool Online = false;
                bool Appoint = false;

                // CRN 
                // Primary Key, if CRN is empty, do not enter record.
                // If CRN is not a number, assign a unique negative value, tracked by duplicate_CRN_indexer
                if (Range.Cells[i, 6] != null && Range.Cells[i, 6].Value2 != null)
                {
                    int parseResult = -1;
                    if (int.TryParse(Range.Cells[i, 6].Value2.ToString(), out parseResult))
                    {
                        CRN = parseResult;
                        bool duplicate_CRN = false;
                        for (int n = 0; n < CRN_List.Count; n++)
                        {
                            if (CRN_List[n] == CRN)
                            {
                                duplicate_CRN = true;
                                break;
                            }
                        }
                        if (!duplicate_CRN)
                        {
                            CRN_List.Add(CRN);
                        }
                        else
                        {
                            CRN = duplicate_CRN_indexer;
                            duplicate_CRN_indexer--;
                        }
                    }
                    else
                    {
                        CRN = duplicate_CRN_indexer;
                        duplicate_CRN_indexer--;
                    }
                    // DEPT
                    if (Range.Cells[i, 3] != null && Range.Cells[i, 3].Value2 != null && Range.Cells[i, 3].Value2 != "")
                    {
                        Dept = Range.Cells[i, 3].Value2.ToString().ToUpper();
                    }
                    // CLASS NUM
                    if (Range.Cells[i, 4] != null && Range.Cells[i, 4].Value2 != null)
                    {
                        if (int.TryParse(Range.Cells[i, 4].Value2.ToString(), out parseResult))
                        {
                            ClassNum = parseResult;
                        }
                    }
                    // CLASS NAME
                    if (Range.Cells[i, 7] != null && Range.Cells[i, 7].Value2 != null && Range.Cells[i, 7].Value2 != "")
                    {
                        ClassName = Range.Cells[i, 7].Value2.ToString();
                    }
                    // SECTION
                    if (Range.Cells[i, 5] != null && Range.Cells[i, 5].Value2 != null)
                    {
                        if (int.TryParse(Range.Cells[i, 5].Value2.ToString(), out parseResult))
                        {
                            Section = parseResult;
                        }
                    }
                    // CREDITS
                    if (Range.Cells[i, 9] != null && Range.Cells[i, 9].Value2 != null)
                    {
                        Credits = (int)(Range.Cells[i, 9].Value2);
                    }
                    // SEATS TAKEN
                    if (Range.Cells[i, 13] != null && Range.Cells[i, 13].Value2 != null)
                    {
                        SeatsTaken = (int)(Range.Cells[i, 13].Value2);
                    }
                    // DEPT
                    if (Range.Cells[i, 16] != null && Range.Cells[i, 16].Value2 != null && Range.Cells[i, 16].Value2 != "")
                    {
                        ClassDay = Range.Cells[i, 16].Value2.ToString().ToUpper();
                    }
                    // Determine Professor
                    if (Range.Cells[i, 22] != null && Range.Cells[i, 22].Value2 != null && Range.Cells[i, 22].Value2 != "")
                    {
                        string profName = Range.Cells[i, 22].Value2.ToString();
                        for (int n = 0; n < professors.Count; n++)
                        {
                            if (professors[n].FullName == profName)
                            {
                                prof = professors[n];
                                break;
                            }
                        }
                    }
                    // Determine ClassRoom
                    if (Range.Cells[i, 20] != null && Range.Cells[i, 20].Value2 != null && Range.Cells[i, 20].Value2 != "")
                    {
                        string bldg = Range.Cells[i, 20].Value2.ToString().ToUpper();
                        if (bldg != "WEB" && !bldg.Contains("APPT"))
                        {
                            int room = -1;
                            if (Range.Cells[i, 21] != null && Range.Cells[i, 21].Value2 != null)
                            {
                                if (int.TryParse(Range.Cells[i, 21].Value2.ToString(), out parseResult))
                                {
                                    room = parseResult;
                                }
                            }
                            string classID = bldg + room;
                            for (int n = 0; n < classrooms.Count; n++)
                            {
                                if (classrooms[n].ClassID == classID)
                                {
                                    classroom = classrooms[n];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (bldg == "WEB")
                            {
                                Online = true;
                            }
                            else if (bldg.Contains("APPT"))
                            {
                                Appoint = true;
                            }
                        }
                    }
                    // Determine TimeSlot
                    if (Range.Cells[i, 17] != null && Range.Cells[i, 17].Value2 != null && Range.Cells[i, 17].Value2 != "")
                    {
                        string rawTime = Range.Cells[i, 17].Value2.ToString();
                        string timePart = formatTime(rawTime.Split(' ')[0]);
                        time = DetermineTime(timePart, ClassDay);
                    }

                    classList.Add(new Classes(CRN, Dept, ClassNum, Section, ClassName, Credits, ClassDay, time, SeatsTaken, classroom, prof, Online, Appoint));
                }
            }


            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(Range);
            Marshal.ReleaseComObject(Worksheet);

            //close and release
            Workbook.Close();
            Marshal.ReleaseComObject(Workbook);

            //quit and release
            App.Quit();
            Marshal.ReleaseComObject(App);
        }
        public void DrawTimeTables() // Draw the GUI grids for MWF - TR (Called by MainWindow)
        {
            TimeTableSetup(MWF, times_MWF);
            TimeTableSetup(TR, times_TR);
        }
        public void TimeTableSetup(Grid parentGrid, Timeslot[] times) // Creates a GUI grid dynamically based on timeslots + classrooms (Called by drawTimeTables())
        {
            String parentName = parentGrid.Name; // Used to uniquely identify the timeslots
            Grid timeTable = new Grid();
            string timeTableName = parentGrid.Name + "_";
            timeTable.Name = timeTableName;
            timeTable.SetValue(Grid.RowProperty, 1);
            timeTable.SetValue(Grid.ColumnProperty, 1);
            timeTable.MinHeight = 450;
            timeTable.MinWidth = 450;
            timeTable.VerticalAlignment = VerticalAlignment.Stretch;
            timeTable.HorizontalAlignment = HorizontalAlignment.Stretch;
            //timeTable.ShowGridLines = true; // Uncomment for debugging (Shows gridlines)
            // make a row for each timeslot
            for (int i = 0; i <= times.Length; i++)
            {
                if (i == 0)
                {
                    timeTable.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.2, GridUnitType.Star) });
                }
                else
                {
                    timeTable.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
                }
            }
            // make a column for each classroom
            for (int i = 0; i <= classrooms.Count; i++)
            {
                if (i == 0)
                {
                    timeTable.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.2, GridUnitType.Star) });
                }
                else
                {
                    timeTable.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                }
            }
            // fill the grid
            for (int i = 0; i <= times.Length; i++)
            {
                // Add row titles (Time Periods)
                if (i != 0)
                {
                    Label timeLabel = new Label();
                    timeLabel.Content = times[i - 1].Time + " " + times[i - 1].Meridian;
                    timeLabel.SetValue(Grid.RowProperty, i);
                    timeLabel.SetValue(Grid.ColumnProperty, 0);
                    timeLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    timeLabel.VerticalAlignment = VerticalAlignment.Center;
                    timeTable.Children.Add(timeLabel);
                }
                for (int n = 1; n <= classrooms.Count; n++)
                {
                    if (i == 0) // Add column titles (Classroom Bldg-Number)
                    {
                        Label classLabel = new Label();
                        classLabel.Content = classrooms[n - 1].Location + "-" + classrooms[n - 1].RoomNum;
                        classLabel.SetValue(Grid.RowProperty, 0);
                        classLabel.SetValue(Grid.ColumnProperty, n);
                        classLabel.HorizontalAlignment = HorizontalAlignment.Center;
                        classLabel.VerticalAlignment = VerticalAlignment.Center;
                        timeTable.Children.Add(classLabel);
                    }
                    else // Add empty timeslots
                    {
                        Label emptySlot = new Label();
                        string lbl_name = timeTable.Name + times[i - 1].TimeID + "_" + classrooms[n - 1].ClassID; // IMPORTANT: Empty timeslots naming convention = GridName_TimeID_ClassID
                        emptySlot.Name = lbl_name;
                        //MessageBox.Show(emptySlot.Name); // DEBUG
                        emptySlot.Content = "";
                        emptySlot.AllowDrop = true;
                        emptySlot.Drop += new DragEventHandler(HandleDropToCell);
                        emptySlot.Style = Resources["DragLabel"] as Style;
                        emptySlot.SetValue(Grid.RowProperty, i);
                        emptySlot.SetValue(Grid.ColumnProperty, n);
                        emptySlot.HorizontalContentAlignment = HorizontalAlignment.Center;
                        emptySlot.VerticalContentAlignment = VerticalAlignment.Center;
                        emptySlot.BorderThickness = new Thickness(1, 1, 1, 1);
                        emptySlot.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        emptySlot.MinWidth = 75;
                        emptySlot.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        emptySlot.Margin = new Thickness(5);
                        emptySlot.ContextMenu = null;
                        //emptySlot.ContextMenu = Resources["ClassContextMenu"] as ContextMenu;
                        object o = FindName(lbl_name);
                        if (o != null)
                        {
                            UnregisterName(lbl_name);
                        }
                        RegisterName(lbl_name, emptySlot);
                        timeTable.Children.Add(emptySlot);
                    }
                }
            }
            // Add the grid to the MWF_Schedule Grid
            object x = FindName(timeTableName);
            if (x != null)
            {
                UnregisterName(timeTableName);
            }
            RegisterName(timeTableName, timeTable);
            parentGrid.Children.Add(timeTable);

            // Populate the empty timeslots with our available information
            PopulateTimeTable(timeTable, times);
        }
        public void PopulateTimeTable(Grid timeTable, Timeslot[] times) // Populate the GUI grid based on class information (Called by timeTableSetup())
        {
            string days = "";
            if (times.Length == times_MWF.Length)
            {
                days = "MWF";
            }
            else
            {
                days = "TR";
            }
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].ClassDay == days)
                {
                    if (classList[i].StartTime.TimeID != "--" && classList[i].Classroom.Location != "N/A" && !classList[i].Online)
                    {
                        string targetBoxID = days + '_' + classList[i].StartTime.TimeID + '_' + classList[i].Classroom.ClassID;
                        Label lbl = (Label)FindName(targetBoxID);
                        if (lbl.Content.ToString() == "")
                        {
                            if (!DetermineTimeConflict(classList[i], days, classList[i].StartTime.TimeID))
                            {
                                lbl.Content = classList[i].TextBoxName;
                                lbl.Background = classList[i].Prof.Prof_Color;
                                lbl.Tag = classList[i].CRN;
                                lbl.ContextMenu = Resources["ClassContextMenu"] as ContextMenu;
                                lbl.ToolTip = classList[i].ToolTipText;
                                classList[i].isAssigned = true;
                            }
                            else
                            {
                                MessageBoxButton button = MessageBoxButton.OK;
                                MessageBoxImage icon = MessageBoxImage.Exclamation;
                                MessageBox.Show("Conflict: " + classList[i].DeptName + " " + classList[i].ClassNumber + 
                                    "\nAt: " + classList[i].ClassDay + " " + classList[i].StartTime.FullTime +
                                    "\nProfessor is already teaching at that time!\n\nMoving class to unassigned classes list", "Timeslot Conflict", button, icon);
                                classList[i].Classroom = new ClassRoom();
                                classList[i].StartTime = new Timeslot();
                                classList[i].isAssigned = false;
                            }
                        }
                        else
                        {
                            MessageBoxButton button = MessageBoxButton.OK;
                            MessageBoxImage icon = MessageBoxImage.Exclamation;
                            MessageBox.Show("Conflict: " + classList[i].DeptName + " " + classList[i].ClassNumber +
                                   "\nAt: " + classList[i].ClassDay + " " + classList[i].StartTime.FullTime +
                                   "\nTimeslot already taken!\n\nMoving class to unassigned classes list", "Timeslot Conflict", button, icon);
                            classList[i].Classroom = new ClassRoom();
                            classList[i].StartTime = new Timeslot();
                            classList[i].isAssigned = false;
                        }
                    }
                }
            }
        }
        public void EmptyGrid(Grid timetable)
        {
            UIElementCollection items = timetable.Children;
            for (int i = 0; i < items.Count; i++)
            {
                Label slot = items[i] as Label;
                if (slot != null && slot.Tag != null)
                {
                    slot.Content = "";
                    RGB_Color white_bg = new RGB_Color(255, 255, 255);
                    slot.Background = white_bg.colorBrush2;
                    slot.Tag = null;
                    slot.ContextMenu = null;
                }
            }
        }
        public void FillUnassigned() // Fill unassigned classes list (GUI) & online classes list with classes that have not been put in the GUI grid
        {
            // empty online and unassigned class lists
            unassignedClasses.Clear();
            onlineClasses.Clear();
            // add from classList
            for (int i = 0; i < classList.Count; i++)
            {
                if (!classList[i].isAssigned)
                {
                    if (classList[i].Online)
                    {
                        onlineClasses.Add(classList[i]);
                    }
                    else
                    {
                        //MessageBox.Show("fillUnassigned() -> Adding " + classList[i].TextBoxName + " to unassigned list.");
                        unassignedClasses.Add(classList[i]);
                    }
                }
            }
        }
        public void AssignProfColors() // !!! call it during excel reading // Give professors a color key based on the palette defined above + Save assigned colors to XML file
        {
            //MessageBox.Show("ColorIndex is currently: " + Settings.Default.ColorIndex);
            // Read from Colors file to see which professors we have already assigned a color. Store in colorPairings List.
            string tempPath = System.IO.Path.GetTempPath();
            string filename = "ColorConfigurations6.xml";
            string fullPath = System.IO.Path.Combine(tempPath, filename);
            XmlSerializer ser = new XmlSerializer(typeof(Pairs));
            if (!File.Exists(fullPath))
            {
                Settings.Default.Reset();
                colorPairs = new Pairs();
                colorPairs.ColorPairings = new List<ProfColors>();
                
                using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
                {
                    ser.Serialize(fs, colorPairs);
                }
            }
            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                colorPairs = ser.Deserialize(fs) as Pairs;
            }
            // go through the professor array
            // if color not already set, add it based on next item on the palette (palette index is set at 0 the first time of execution on a user PC)
            for (int i = 0; i < professors.Count; i++)
            {
                bool found = false;
                for (int n = 0; n < colorPairs.ColorPairings.Count; n++)
                {
                    if (professors[i].FullName == colorPairs.ColorPairings[n].ProfName)
                    {
                        //MessageBox.Show("Found " + professors[i].FullName + "!");
                        found = true;
                        //MessageBox.Show("Reassigning " + colorPairs.ColorPairings[n].Color + " to " + professors[i].FullName + ".");
                        professors[i].profRGB = StringToRGB(colorPairs.ColorPairings[n].Color);
                        break;
                    }
                }
                if (!found)
                {
                    //MessageBox.Show("Adding " + professors[i].FullName + "!");
                    // Give professor a colour
                    int paletteIndex = Settings.Default.ColorIndex;
                    if (paletteIndex < colorPalette.Length)
                    {
                        professors[i].profRGB = colorPalette[paletteIndex];
                        //MessageBox.Show("Assigned: " + colorPalette[paletteIndex].colorString + "\nProfessor: " + professors[i].FullName);
                        paletteIndex++;
                        Settings.Default.ColorIndex = paletteIndex;
                    }
                    else
                    {
                        //MessageBox.Show("Random Color");
                        Random rand = new Random();
                        professors[i].profRGB = new RGB_Color((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));
                    }
                    // Add it to pairings list
                    colorPairs.ColorPairings.Add(new ProfColors { ProfName = professors[i].FullName, Color = professors[i].profRGB.colorString });
                    //MessageBox.Show("Added " + professors[i].FullName + " + " + professors[i].profRGB.colorString);
                }
            }
            // Save changes to Colors.xml
            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                ser.Serialize(fs, colorPairs);
            }
            Settings.Default.Save();
            // Reassign colors to professors in classlist
            for (int i = 0; i < classList.Count; i++)
            {
                for (int n = 0; n < colorPairs.ColorPairings.Count; n++)
                {
                    if (classList[i].Prof.FullName == colorPairs.ColorPairings[n].ProfName)
                    {
                        classList[i].Prof.profRGB = StringToRGB(colorPairs.ColorPairings[n].Color);
                        break;
                    }
                }
            }
        }
        public void BindData()
        {
            BindUnassignedList();
            BindProfessorKey();
            BindClassList();
            BindProfList();
        }
        public void BindUnassignedList()
        {
            Online_Classes_Grid.ItemsSource = onlineClasses;
            Unassigned_Classes_Grid.ItemsSource = unassignedClasses;
        }
        public void BindProfessorKey()
        {
            Professor_Key_List.ItemsSource = professors;
        } // Fill professor color key list in the GUI
        public void BindClassList()
        {
            Full_Classes_Grid.ItemsSource = classList;
        }
        public void BindProfList()
        {
            Full_Professors_Grid.ItemsSource = professors;
        }
        public void RefreshGUI()
        {
            Grid timetable_MWF = (Grid)FindName("MWF_");
            Grid timetable_TR = (Grid)FindName("TR_");
            EmptyGrid(timetable_MWF);
            EmptyGrid(timetable_TR);
            PopulateTimeTable(timetable_MWF, times_MWF);
            PopulateTimeTable(timetable_TR, times_TR);
            FillUnassigned();
        }
        public void SaveChanges()
        {

        } // Writes to excel file

        // ADD / REMOVE / EDIT functionality (Professors, Classrooms, Classes)
        // Professors
        public void AddProfessor(Professors prof)
        {
            professors.Add(prof);
            AssignProfColors();
        }
        private void Btn_AddProfessor_Click(object sender, RoutedEventArgs e)
        {
            Unfocus_Overlay.Visibility = Visibility.Visible;
            AddProfessorDialog addProfDialog = new AddProfessorDialog();
            addProfDialog.Owner = this;
            addProfDialog.ShowDialog();
            Unfocus_Overlay.Visibility = Visibility.Hidden;
            if (Application.Current.MainWindow.Resources["Set_Prof_Success"] != null && (bool)Application.Current.MainWindow.Resources["Set_Prof_Success"] == true)
            {
                string fName = Application.Current.MainWindow.Resources["Set_Prof_FN"].ToString();
                string lName = Application.Current.MainWindow.Resources["Set_Prof_LN"].ToString();
                string id = Application.Current.MainWindow.Resources["Set_Prof_ID"].ToString();
                AddProfessor(new Professors(fName, lName, id));
                Application.Current.MainWindow.Resources["Set_Prof_Success"] = false;
            }
        }
        public void RemoveProfessor(string sruID)
        {
            for (int i = 0; i < professors.Count; i++)
            {
                if (professors[i].SRUID == sruID)
                {
                    professors.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].Prof.SRUID == sruID)
                {
                    classList[i].Prof = new Professors();
                }
            }
            for (int i = 0; i < unassignedClasses.Count; i++)
            {
                if (unassignedClasses[i].Prof.SRUID == sruID)
                {
                    unassignedClasses[i].Prof = new Professors();
                }
            }
            for (int i = 0; i < onlineClasses.Count; i++)
            {
                if (onlineClasses[i].Prof.SRUID == sruID)
                {
                    onlineClasses[i].Prof = new Professors();
                }
            }
            // update the GUI grid
        }
        private void Btn_RemoveProfessor_Click(object sender, RoutedEventArgs e)
        {
            // find the professor
            string sruID = "";
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    ListViewItem source = cm.PlacementTarget as ListViewItem;
                    if (source != null) // Being called from a Professor Color Key
                    {
                        sruID = source.Tag.ToString();
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock prof_ID = parentGrid.Columns[2].GetCellContent(sourceRow) as TextBlock;
                        sruID = prof_ID.Text;
                    }
                    RemoveProfessor(sruID);
                    RefreshGUI();
                }
            }
        }
        public void EditProfessor(string sruID)
        {
            Unfocus_Overlay.Visibility = Visibility.Visible;
            Professors prof = DetermineProfessor(sruID);
            EditProfessorDialog editProfessorDialog = new EditProfessorDialog(prof);
            editProfessorDialog.ShowDialog();
            Unfocus_Overlay.Visibility = Visibility.Hidden;

            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].Prof.SRUID == sruID)
                {
                    classList[i].Prof = prof;
                }
            }
            for (int i = 0; i < unassignedClasses.Count; i++)
            {
                if (unassignedClasses[i].Prof.SRUID == sruID)
                {
                    unassignedClasses[i].Prof = prof;
                }
            }
            for (int i = 0; i < onlineClasses.Count; i++)
            {
                if (onlineClasses[i].Prof.SRUID == sruID)
                {
                    onlineClasses[i].Prof = prof;
                }
            }
        }
        private void Btn_EditProfessor_Click(object sender, RoutedEventArgs e)
        {
            // find the professor
            string sruID = "";
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    ListViewItem source = cm.PlacementTarget as ListViewItem;
                    if (source != null) // Being called from a Professor Color Key
                    {
                        sruID = source.Tag.ToString();
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock prof_ID = parentGrid.Columns[2].GetCellContent(sourceRow) as TextBlock;
                        sruID = prof_ID.Text;
                    }
                    EditProfessor(sruID);
                    RefreshGUI();
                }
            }
        }
        // Classrooms
        public void AddClassroom(ClassRoom room)
        {
            // Add Classroom to classroom list
            classrooms.Add(room);
            // Remove old Grids
            Grid child = FindName("MWF_") as Grid;
            MWF.Children.Remove(child);
            Grid child2 = FindName("TR_") as Grid;
            TR.Children.Remove(child2);
            // Redraw Grids
            DrawTimeTables();
        }
        private void Btn_AddClassRoom_Click(object sender, RoutedEventArgs e)
        {
            Unfocus_Overlay.Visibility = Visibility.Visible;
            AddClassRoomDialog addClassRoomDialog = new AddClassRoomDialog();
            addClassRoomDialog.Owner = this;
            addClassRoomDialog.ShowDialog();
            Unfocus_Overlay.Visibility = Visibility.Hidden;
            if (Application.Current.MainWindow.Resources["Set_ClassRoom_Success"] != null && (bool)Application.Current.MainWindow.Resources["Set_ClassRoom_Success"] == true)
            {
                string bldg = Application.Current.MainWindow.Resources["Set_ClassRoom_Bldg"].ToString();
                int roomNum = Int32.Parse(Application.Current.MainWindow.Resources["Set_ClassRoom_Num"].ToString());
                int capacity = Int32.Parse(Application.Current.MainWindow.Resources["Set_ClassRoom_Seats"].ToString());
                AddClassroom(new ClassRoom(bldg, roomNum, capacity));
                Application.Current.MainWindow.Resources["Set_ClassRoom_Success"] = false;
            }
        }
        public void RemoveClassroom(string classID)
        {
            for (int i = 0; i < classrooms.Count; i++)
            {
                if (classrooms[i].ClassID == classID)
                {
                    classrooms.RemoveAt(i);
                }
            }
            // update the GUI grid
        }
        private void Btn_RemoveClassroom_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Yet to be implemented");
        }
        /*
        public void EditClassroom(string classID)
        {

        }
        private void Btn_EditClassroom_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Yet to be implemented");
        }
        */
        // Classes
        public void AddClass(Classes newClass)
        {
            classList.Add(newClass);
            if (newClass.Online)
            {
                onlineClasses.Add(newClass);
            }
            else
            {
                unassignedClasses.Add(newClass);
            }
        }
        private void Btn_AddClass_Click(object sender, RoutedEventArgs e)
        {
            Unfocus_Overlay.Visibility = Visibility.Visible;
            AddClassDialog addClassDialog = new AddClassDialog();
            addClassDialog.Owner = this;
            addClassDialog.ShowDialog();
            Unfocus_Overlay.Visibility = Visibility.Hidden;
            //MessageBox.Show("class_success: " + Application.Current.MainWindow.Resources["Set_Class_Success"].ToString());

            if (Application.Current.MainWindow.Resources["Set_Class_Success"] != null && (bool)Application.Current.MainWindow.Resources["Set_Class_Success"] == true)
            {
                int crn = Int32.Parse(Application.Current.MainWindow.Resources["Set_Class_CRN"].ToString());
                string dpt = Application.Current.MainWindow.Resources["Set_Class_Dept"].ToString();
                int number = Int32.Parse(Application.Current.MainWindow.Resources["Set_Class_Number"].ToString());
                int sect = Int32.Parse(Application.Current.MainWindow.Resources["Set_Class_Section"].ToString());
                string name = Application.Current.MainWindow.Resources["Set_Class_Name"].ToString();
                int credits = Int32.Parse(Application.Current.MainWindow.Resources["Set_Class_Credits"].ToString());
                string prof = Application.Current.MainWindow.Resources["Set_Class_Professor"].ToString();
                bool online = Boolean.Parse(Application.Current.MainWindow.Resources["Set_Class_Online"].ToString());
                AddClass(new Classes(crn, dpt, number, sect, name, credits, "", new Timeslot(), 0, new ClassRoom(), DetermineProfessor(prof), online, false));
                Application.Current.MainWindow.Resources["Set_Class_Success"] = false;
            }
        }
        public void RemoveClass(int crn)
        {
            Classes removalTarget;
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].CRN == crn)
                {
                    removalTarget = classList[i];
                    if (removalTarget.Online)
                    {
                        for (int n = 0; n < onlineClasses.Count; n++)
                        {
                            if (onlineClasses[n].CRN == crn)
                            {
                                onlineClasses.RemoveAt(n);
                                break;
                            }
                        }
                    }
                    else if (!removalTarget.isAssigned)
                    {
                        for (int n = 0; n < unassignedClasses.Count; n++)
                        {
                            if (unassignedClasses[n].CRN == crn)
                            {
                                unassignedClasses.RemoveAt(n);
                                break;
                            }
                        }
                    }
                    classList.RemoveAt(i);
                    break;
                }
            }
        }
        private void Btn_RemoveClass_Click(object sender, RoutedEventArgs e)
        {
            // find the class
            int crn = -1;
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    Label source = cm.PlacementTarget as Label;
                    if (source != null) // Being called from a Label
                    {
                        crn = Int32.Parse(source.Tag.ToString());
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock crn_number = parentGrid.Columns[0].GetCellContent(sourceRow) as TextBlock;
                        crn = Int32.Parse(crn_number.Text);
                    }
                    RemoveClass(crn);
                    RefreshGUI();
                }
            }
        }
        public void EditClass(int crn)
        {
            Unfocus_Overlay.Visibility = Visibility.Visible;
            Classes toEdit = DetermineClass(crn);
            EditClassDialog editClassDialog = new EditClassDialog(toEdit);
            editClassDialog.ShowDialog();
            Unfocus_Overlay.Visibility = Visibility.Hidden;
        }
        private void Btn_EditClass_Click(object sender, RoutedEventArgs e)
        {
            // find the class
            int crn = -1;
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    Label source = cm.PlacementTarget as Label;
                    if (source != null) // Being called from a Label
                    {
                        crn = Int32.Parse(source.Tag.ToString());
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock crn_number = parentGrid.Columns[0].GetCellContent(sourceRow) as TextBlock;
                        crn = Int32.Parse(crn_number.Text);
                    }
                    EditClass(crn);
                    RefreshGUI();
                }
            }
        }

        // DRAG/DROP functionality
        private void MouseMoveOnGridRow(object sender, MouseEventArgs e) // Handles DRAG operation on unassigned classes list item
        {
            TextBlock cellUnderMouse = sender as TextBlock;
            if (cellUnderMouse != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DataGridRow row = DataGridRow.GetRowContainingElement(cellUnderMouse);
                DragDrop.DoDragDrop(Unassigned_Classes_Grid, row, DragDropEffects.Copy);
            }
        }
        private void HandleDropToCell(Object sender, DragEventArgs e) // !!! Needs validation checks // Handles DROP operation to assigned classes box
        {
            Label sourceLabel = (Label)e.Data.GetData(typeof(object));
            Label receiver = sender as Label;
            if (receiver.Content.ToString() == "")
            {
                if (sourceLabel != null)
                {
                    int classIndex = (int)e.Data.GetData(typeof(int));
                    // parse target slot info
                    string days = receiver.Name.Split('_')[0];
                    string start = receiver.Name.Split('_')[1];
                    string roomInfo = receiver.Name.Split('_')[2];
                    string bldg = roomInfo.Substring(0, 3);
                    int room = Int32.Parse(roomInfo.Substring(3, (roomInfo.Length - 3)));

                    bool isConflict = DetermineTimeConflict(classList[classIndex], days, start);
                    if (!isConflict)
                    {
                        classList[classIndex].ClassDay = days;
                        classList[classIndex].StartTime = DetermineTime(start, days);
                        classList[classIndex].Classroom = DetermineClassroom(bldg, room);

                        // Give the newLabel the class information
                        receiver.Content = sourceLabel.Content;
                        receiver.Background = sourceLabel.Background;
                        receiver.Tag = sourceLabel.Tag;
                        receiver.ToolTip = sourceLabel.ToolTip;
                        receiver.ContextMenu = Resources["ClassContextMenu"] as ContextMenu;

                        // clear the sourceLabel
                        sourceLabel.Content = "";
                        RGB_Color white_bg = new RGB_Color(255, 255, 255);
                        sourceLabel.Background = white_bg.colorBrush2;
                        sourceLabel.Tag = null;
                        sourceLabel.ToolTip = null;
                        sourceLabel.ContextMenu = null;
                    }
                    else
                    {
                        MessageBoxButton button = MessageBoxButton.OK;
                        MessageBoxImage icon = MessageBoxImage.Exclamation;
                        MessageBox.Show("Professor is already teaching at that time!", "Invalid action", button, icon);
                    }
                }
                else
                {
                    int classCRN = 0;
                    DataGridRow droppedRow = (DataGridRow)e.Data.GetData(typeof(DataGridRow));
                    if (droppedRow == null)
                    {
                        MessageBox.Show("dropped row was null");
                    }
                    else
                    {
                        TextBlock crn_number = Unassigned_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                        //MessageBox.Show(crn_number.Text);
                        classCRN = Int32.Parse(crn_number.Text);
                    }

                    /// VALIDATION CHECKS GO HERE ///
                    // check if its online class
                    bool validOperation = true;
                    int classIndex = -1;
                    for (int i = 0; i < classList.Count; i++)
                    {
                        if (classList[i].CRN == classCRN)
                        {
                            if (classList[i].Online)
                            {
                                string messageBoxText = "Are you sure you want to change this class from Online to In-Class?\n\n(You can later drag it back to the online class list to revert changes)";
                                string caption = "Online class warning";
                                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                                MessageBoxImage icon = MessageBoxImage.Question;
                                // Display + process message box results
                                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                                switch (result)
                                {
                                    case MessageBoxResult.Yes:
                                        break;
                                    case MessageBoxResult.No:
                                        validOperation = false;
                                        break;
                                    case MessageBoxResult.Cancel:
                                        validOperation = false;
                                        break;
                                }
                            }
                            classIndex = i;
                            break;
                        }
                    }
                    if (validOperation)
                    {
                        string days = receiver.Name.Split('_')[0];
                        string start = receiver.Name.Split('_')[1];
                        string roomInfo = receiver.Name.Split('_')[2];
                        string bldg = roomInfo.Substring(0, 3);
                        int room = Int32.Parse(roomInfo.Substring(3, (roomInfo.Length - 3)));
                        if (!DetermineTimeConflict(classList[classIndex], days, start))
                        {
                            if (!classList[classIndex].Online)
                            {
                                classList[classIndex].isAssigned = true;
                                // remove record from unassigned classes list
                                for (int i = 0; i < unassignedClasses.Count; i++)
                                {
                                    if (unassignedClasses[i].CRN == classCRN)
                                    {
                                        unassignedClasses.RemoveAt(i);
                                        classList[classIndex].isAssigned = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                classList[classIndex].Online = false;
                                // remove record from online classes list
                                for (int i = 0; i < onlineClasses.Count; i++)
                                {
                                    if (onlineClasses[i].CRN == classCRN)
                                    {
                                        onlineClasses.RemoveAt(i);
                                        classList[classIndex].Online = false;
                                        break;
                                    }
                                }
                            }
                            // Update class in masterlist = give it a start time + classroom
                            classList[classIndex].ClassDay = days;
                            classList[classIndex].StartTime = DetermineTime(start, days);
                            classList[classIndex].Classroom = DetermineClassroom(bldg, room);
                            // Give the Label the class information
                            receiver.Content = classList[classIndex].TextBoxName;
                            receiver.Background = classList[classIndex].Prof.Prof_Color;
                            receiver.Tag = classCRN;
                            receiver.ToolTip = classList[classIndex].ToolTipText;
                            receiver.ContextMenu = Resources["ClassContextMenu"] as ContextMenu;
                        }
                        else
                        {
                            MessageBoxButton button = MessageBoxButton.OK;
                            MessageBoxImage icon = MessageBoxImage.Exclamation;
                            MessageBox.Show("Professor is already teaching at that time!", "Invalid action", button, icon);
                        }
                    }
                }
            }
            else
            {
                if (sourceLabel != null)
                {
                    if (sourceLabel.Content.ToString() != receiver.Content.ToString())
                    {
                        MessageBoxButton button = MessageBoxButton.OK;
                        MessageBoxImage icon = MessageBoxImage.Exclamation;
                        MessageBox.Show("Timeslot is already taken!", "Invalid action", button, icon);
                    }
                }
                else
                {
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Exclamation;
                    MessageBox.Show("Timeslot is already taken!", "Invalid action", button, icon);
                }
            }
        }
        private void MouseMoveOnAssignedClass(object sender, MouseEventArgs e) // Handles DRAG operation on assigned classes box
        {
            Label labelUnderMouse = sender as Label;
            int classIndex = -1;
            if ((labelUnderMouse != null) && (e.LeftButton == MouseButtonState.Pressed) && (labelUnderMouse.Tag != null) && (labelUnderMouse.Tag.ToString() != ""))
            {
                // find index of class being represented by the label
                for (int i = 0; i < classList.Count; i++)
                {
                    if (classList[i].CRN == Int32.Parse(labelUnderMouse.Tag.ToString()))
                    {
                        classIndex = i;
                        break;
                    }
                }
                // Package the data
                DataObject data = new DataObject();
                data.SetData(typeof(int), classIndex);
                data.SetData(typeof(object), labelUnderMouse);
                // send dataObject
                DragDrop.DoDragDrop(labelUnderMouse, data, DragDropEffects.Copy);
            }
        }
        private void HandleDropToList(Object sender, DragEventArgs e) // Handles DROP operation to unassigned classes list 
        {
            Label sourceLabel = (Label)e.Data.GetData(typeof(object));
            if (sourceLabel != null)
            {
                int classIndex = (int)e.Data.GetData(typeof(int));
                // clear the Label
                sourceLabel.Content = "";
                RGB_Color white_bg = new RGB_Color(255, 255, 255);
                sourceLabel.Background = white_bg.colorBrush2;
                sourceLabel.ContextMenu = null;
                // add the class to unassigned class list
                classList[classIndex].Classroom = new ClassRoom();
                classList[classIndex].ClassDay = "";
                classList[classIndex].StartTime = new Timeslot();
                classList[classIndex].isAssigned = false;
                unassignedClasses.Add(classList[classIndex]);
            }
            else
            {
                int classCRN = 0;
                DataGridRow droppedRow = (DataGridRow)e.Data.GetData(typeof(DataGridRow));
                if (droppedRow != null)
                {
                    TextBlock crn_number = Unassigned_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                    classCRN = Int32.Parse(crn_number.Text);
                    Classes theClass = DetermineClass(classCRN);
                    if (theClass.Online)
                    {
                        string messageBoxText = "Are you sure you want to change this class\nfrom Online to In-Class?\n\n(You can later drag it back to the online class list to revert changes)";
                        string caption = "Online class alteration";
                        MessageBoxButton button = MessageBoxButton.YesNoCancel;
                        MessageBoxImage icon = MessageBoxImage.Question;
                        // Display + Process message box results
                        MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                        switch (result)
                        {
                            case MessageBoxResult.Yes:
                                // Find the class
                                for (int i = 0; i < classList.Count; i++)
                                {
                                    if (classList[i].CRN == classCRN)
                                    {
                                        classList[i].Online = false;
                                        // Add it to Unassigned classes list
                                        unassignedClasses.Add(classList[i]);
                                    }
                                }
                                // remove record from online classes list
                                for (int i = 0; i < onlineClasses.Count; i++)
                                {
                                    if (onlineClasses[i].CRN == classCRN)
                                    {
                                        onlineClasses.RemoveAt(i);
                                        break;
                                    }
                                }
                                break;
                            case MessageBoxResult.No:
                                break;
                            case MessageBoxResult.Cancel:
                                break;
                        }
                    }
                }
            }
        }
        private void HandleDropToOnlineList(Object sender, DragEventArgs e) // Handles DROP operation to online classes list 
        {
            Label sourceLabel = (Label)e.Data.GetData(typeof(object));
            if (sourceLabel != null)
            {
                string messageBoxText = "Are you sure you want to change this\nIn-Class class to Online format?\n\n(You can later drag it back to the timetable to revert changes)";
                string caption = "Online class alteration";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Question;
                // Display + Process message box results
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // User pressed Yes button
                        int classIndex = (int)e.Data.GetData(typeof(int));
                        // clear the Label
                        sourceLabel.Content = "";
                        RGB_Color white_bg = new RGB_Color(255, 255, 255);
                        sourceLabel.Background = white_bg.colorBrush2;
                        sourceLabel.ContextMenu = Resources["ClassContextMenu"] as ContextMenu;
                        // add the class to online class list
                        classList[classIndex].Classroom = new ClassRoom();
                        classList[classIndex].ClassDay = "";
                        classList[classIndex].StartTime = new Timeslot();
                        classList[classIndex].isAssigned = false;
                        classList[classIndex].Online = true;
                        onlineClasses.Add(classList[classIndex]);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else
            {
                DataGridRow droppedRow = (DataGridRow)e.Data.GetData(typeof(DataGridRow));
                if (droppedRow != null)
                {
                    TextBlock crn_number = Online_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                    int classCRN = Int32.Parse(crn_number.Text);
                    Classes theClass = DetermineClass(classCRN);
                    if (!theClass.Online)
                    {
                        string messageBoxText = "Are you sure you want to change this\nIn-Class class to Online format?\n\n(You can later drag it back to the unassigned class list to revert changes)";
                        string caption = "Online class alteration";
                        MessageBoxButton button = MessageBoxButton.YesNoCancel;
                        MessageBoxImage icon = MessageBoxImage.Question;
                        // Display + Process message box results
                        MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                        switch (result)
                        {
                            case MessageBoxResult.Yes:
                                // User pressed Yes button
                                // Find the class
                                for (int i = 0; i < classList.Count; i++)
                                {
                                    if (classList[i].CRN == classCRN)
                                    {
                                        classList[i].Online = true;
                                        // Add it to Online classes list
                                        onlineClasses.Add(classList[i]);
                                    }
                                }
                                // remove record from unassigned classes list
                                for (int i = 0; i < unassignedClasses.Count; i++)
                                {
                                    if (unassignedClasses[i].CRN == classCRN)
                                    {
                                        unassignedClasses.RemoveAt(i);
                                        break;
                                    }
                                }
                                break;
                            case MessageBoxResult.No:
                                break;
                            case MessageBoxResult.Cancel:
                                break;
                        }
                    }
                }
            }
        }

        // Utility functions
        public RGB_Color StringToRGB(string s)
        {
            RGB_Color color;
            String[] parts = s.Split('.');
            color = new RGB_Color(Byte.Parse(parts[0]), Byte.Parse(parts[1]), Byte.Parse(parts[2]));
            return color;
        }
        public Timeslot DetermineTime(string startTime, string classDay)
        {
            string id = startTime.Substring(0, 2);
            if (classDay == "MWF")
            {
                for (int i = 0; i < times_MWF.Length; i++)
                {
                    if (times_MWF[i].TimeID == id)
                    {
                        return times_MWF[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < times_TR.Length; i++)
                {
                    if (times_TR[i].TimeID == id)
                    {
                        return times_TR[i];
                    }
                }
            }
            MessageBox.Show("DEBUG: Couldnt find the referenced time!");
            return new Timeslot();
        }
        public ClassRoom DetermineClassroom(string building, int roomNum)
        {
            string id = building + roomNum;
            for (int i = 0; i < classrooms.Count; i++)
            {
                if (classrooms[i].ClassID == id)
                {
                    return classrooms[i];
                }
            }
            MessageBox.Show("DEBUG: Couldnt find the referenced classroom!");
            return new ClassRoom();
        }
        public Professors DetermineProfessor(string sruID)
        {
            for (int i = 0; i < professors.Count; i++)
            {
                if (professors[i].SRUID == sruID)
                {
                    return professors[i];
                }
            }
            MessageBox.Show("DEBUG: Couldnt find the referenced professor!");
            return new Professors();
        }
        public Classes DetermineClass(int crn)
        {
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].CRN == crn)
                {
                    return classList[i];
                }
            }
            MessageBox.Show("DEBUG: Couldnt find the referenced professor!");
            return new Classes();
        }
        private T GetParent<T>(DependencyObject d) where T : class
        {
            while (d != null && !(d is T))
            {
                d = VisualTreeHelper.GetParent(d);
            }
            return d as T;
        }
        public bool DetermineTimeConflict(Classes _class, string days, string timeID)
        {
            if (_class.Prof.FirstName == "None")
            {
                return false;
            }
            else
            {
                bool isConflict = false;
                string profName = _class.Prof.FullName;
                string rowID = days + "_" + timeID;
                //MessageBox.Show("Checking against " + rowID + "\nProf: " + profName);
                string labelID = "";
                Label lbl = null;
                int classCRN = -1;
                for (int i = 0; i < classrooms.Count; i++)
                {
                    labelID = rowID + "_" + classrooms[i].ClassID;
                    lbl = (Label)FindName(labelID);
                    if (lbl != null)
                    {
                        if (lbl.Tag != null)
                        {
                            classCRN = Int32.Parse(lbl.Tag.ToString());
                            for (int n = 0; n < classList.Count; n++)
                            {
                                if (classList[n].CRN == classCRN)
                                {
                                    if (classList[n].Prof.FullName == profName && _class.CRN != classCRN)
                                    {
                                        isConflict = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Label " + labelID + " wasnt found!");
                    }
                    if (isConflict)
                    {
                        break;
                    }
                }
                return isConflict;
            }
        }
        public string formatTime(string time)
        {
            string formattedTime = "";
            if (time.Contains(":"))
            {
                string left = time.Split(':')[0];
                string right = time.Split(':')[1];
                if (left.Length == 1)
                {
                    left = "0" + left;
                }
                formattedTime = left + ":" + right;
            }
            else
            {
                if (time.Length == 3)
                {
                    time = "0" + time;
                }
                string left = time.Substring(0, 2);
                string right = time.Substring(2, 2);
                formattedTime = left + ":" + right;
            }
            return formattedTime;
        }

        // Professor + Color pairings (Used for persistent memory storage in xml file)
        public class Pairs
        {
            [XmlArray("ColorPairings"), XmlArrayItem(typeof(ProfColors), ElementName = "ProfColors")]
            public List<ProfColors> ColorPairings { get; set; }
        } 
        [XmlRoot("Pairs")]
        public class ProfColors
        {
            public string ProfName { get; set; }
            public string Color { get; set; }
        }
    }
}
