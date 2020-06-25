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

namespace Schedule_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ////////////// GLOBAL VARIABLES ////////////////
        Timeslot[] times_MWF = { new Timeslot("08:00", "08:50", "AM"), new Timeslot("09:00", "09:50", "AM"), new Timeslot("10:00", "10:50", "AM"), new Timeslot("11:00", "11:50", "AM"), new Timeslot("12:00", "12:50", "PM"), new Timeslot("01:00", "01:50", "PM"), new Timeslot("02:00", "02:50", "PM"), new Timeslot("03:00", "03:50", "PM"), new Timeslot("04:00", "04:50", "PM"), new Timeslot("05:00", "05:50", "PM"), new Timeslot("06:00", "06:50", "PM") };
        Timeslot[] times_TR = { new Timeslot("08:00", "09:15", "AM"), new Timeslot("09:30", "10:45", "AM"), new Timeslot("11:00", "12:15", "AM"), new Timeslot("12:30", "01:45", "PM"), new Timeslot("02:00", "03:15", "PM"), new Timeslot("03:30", "04:45", "PM"), new Timeslot("05:00", "06:15", "PM") };
        ObservableCollection<ClassRoom> classrooms = new ObservableCollection<ClassRoom>(new ClassRoom[] { new ClassRoom("ATS", 215, 40), new ClassRoom("ATS", 347, 40), new ClassRoom("ATS", 117, 40), new ClassRoom("ATS", 999, 40) });
        ProfessorList professors = (ProfessorList)Application.Current.FindResource("Professor_List_View");
        RGB_Color[] colorPalette = { new RGB_Color(244,67,54), new RGB_Color(156,39,176), new RGB_Color(63,81,181), new RGB_Color(3,169,244), new RGB_Color(0,150,136), new RGB_Color(139,195,74), new RGB_Color(255,235,59), new RGB_Color(255,152,0), new RGB_Color(233,30,99), new RGB_Color(103,58,183), new RGB_Color(33,150,243), new RGB_Color(0,188,212), new RGB_Color(76,175,80), new RGB_Color(205,220,57), new RGB_Color(255,193,7), new RGB_Color(255,87,34) };
        pairs colorPairs;
        ClassList classList = (ClassList)Application.Current.FindResource("Classes_List_View");
        EmptyClassList unassignedClasses = (EmptyClassList)Application.Current.FindResource("Unassigned_Classes_List_View");
        EmptyClassList onlineClasses = (EmptyClassList)Application.Current.FindResource("Online_Classes_List_View");


        ////////////// START OF EXECUTION ////////////////
        public MainWindow()
        {
            InitializeComponent();

            // Read from excel to get data
            readExcel();

            // Assign professor colors 
            assignProfColors();

            // Draw timetables for MWF / TR
            drawTimeTables();

            // Fill Unassigned Classes List
            fillUnassigned();

            // Bind classes list to the "Classes" tab dataGrid of the GUI
            bindClassList();
            // Bind professors list to the "Professors" tab dataGrid of the GUI
            bindProfList();
        }

        public void readExcel() // Read from excel to fill up classList + classrooms + professors (Called by MainWindow)
        {

        }

        public void drawTimeTables() // Draw the GUI grids for MWF - TR (Called by MainWindow)
        {
            timeTableSetup(MWF, times_MWF);
            timeTableSetup(TR, times_TR);
        }

        public void timeTableSetup(Grid parentGrid, Timeslot[] times) // Creates a GUI grid dynamically based on timeslots + classrooms (Called by drawTimeTables())
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
            populateTimeTable(timeTable, times);
        }

        public void populateTimeTable(Grid timeTable, Timeslot[] times) // Populate the GUI grid based on class information (Called by timeTableSetup())
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
                    if (classList[i].StartTime.TimeID != "--" && classList[i].Classroom.Location != "N/A")
                    {
                        string targetBoxID = days + '_' + classList[i].StartTime.TimeID + '_' + classList[i].Classroom.ClassID;
                        //Label targetBox = Resources[targetBoxID] as Label;
                        Label lbl = (Label)FindName(targetBoxID);
                        // !!!!! ------ VALIDATION (Already contains class?) -------- !!!!!!! //
                        lbl.Content = classList[i].TextBoxName;
                        lbl.Background = classList[i].Prof.Prof_Color;
                        lbl.Tag = classList[i].CRN;
                        classList[i].isAssigned = true;
                    }
                }
            }
        } 

        public void fillUnassigned() // Fill unassigned classes list (GUI) & online classes list with classes that have not been put in the GUI grid
        {
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
            Online_Classes_Grid.ItemsSource = onlineClasses;
            Unassigned_Classes_Grid.ItemsSource = unassignedClasses;
        }  

        public void assignProfColors() // !!! call it during excel reading // Give professors a color key based on the palette defined above + Save assigned colors to XML file
        {
            // Read from Colors file to see which professors we have already assigned a color. Store in colorPairings List.
            string tempPath = System.IO.Path.GetTempPath();
            string filename = "ColorsConfig.xml";
            string fullPath = System.IO.Path.Combine(tempPath, filename);
            XmlSerializer ser = new XmlSerializer(typeof(pairs));
            if (!File.Exists(fullPath))
            {
                colorPairs = new pairs();
                colorPairs.colorPairings = new List<ProfColors>();
                colorPairs.colorPairings.Add(new ProfColors { ProfName = "John Doe", Color = "0.0.0" });
                
                using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
                {
                    ser.Serialize(fs, colorPairs);
                }
            }
            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                colorPairs = ser.Deserialize(fs) as pairs;
            }
            // go through the professor array
            // if color not already set, add it based on next item on the palette (palette index is set at 0 the first time of execution on a user PC)
            for (int i = 0; i < professors.Count; i++)
            {
                bool found = false;
                for (int n = 0; n < colorPairs.colorPairings.Count; n++)
                {
                    if (professors[i].FullName == colorPairs.colorPairings[n].ProfName)
                    {
                        //MessageBox.Show("Found " + professors[i].FullName + "!");
                        found = true;
                        //MessageBox.Show("Reassigning " + colorPairs.colorPairings[n].Color + " to " + professors[i].FullName + ".");
                        professors[i].profRGB = stringToRGB(colorPairs.colorPairings[n].Color);
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
                        paletteIndex++;
                        Settings.Default.ColorIndex = paletteIndex;
                    }
                    else
                    {
                        Random rand = new Random();
                        professors[i].profRGB = new RGB_Color((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));
                    }
                    // Add it to pairings list
                    colorPairs.colorPairings.Add(new ProfColors { ProfName = professors[i].FullName, Color = professors[i].profRGB.colorString });
                    //MessageBox.Show("Added " + professors[i].FullName + " + " + professors[i].profRGB.colorString);
                }
            }
            // Save changes to Colors.xml
            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                ser.Serialize(fs, colorPairs);
            }
            Properties.Settings.Default.Save();
            // Reassign colors to professors in classlist
            for (int i = 0; i < classList.Count; i++)
            {
                for (int n = 0; n < colorPairs.colorPairings.Count; n++)
                {
                    if (classList[i].Prof.FullName == colorPairs.colorPairings[n].ProfName)
                    {
                        classList[i].Prof.profRGB = stringToRGB(colorPairs.colorPairings[n].Color);
                        break;
                    }
                }
            }
            fillProfessorKey();
        }

        public void fillProfessorKey()
        {
            Professor_Key_List.ItemsSource = professors;
        } // Fill professor color key list in the GUI

        public void bindClassList()
        {
            Full_Classes_Grid.ItemsSource = classList;
        }

        public void bindProfList()
        {
            Full_Professors_Grid.ItemsSource = professors;
        }

        public void saveChanges()
        {

        } // Writes to excel file

        // ADD / REMOVE functionality (Professors, Classrooms, Classes)
        // Professors
        public void addProfessor(Professors prof)
        {
            professors.Add(prof);
            // Assign color
            assignProfColors();
        }
        private void Btn_AddProfessor_Click(object sender, RoutedEventArgs e)
        {
            AddProfessorDialog addProfDialog = new AddProfessorDialog();
            addProfDialog.ShowDialog();
            // If operation successful ------- IMPLEMENT CHECK 
            string fName = Resources["Set_Prof_FN"].ToString();
            string lName = Resources["Set_Prof_LN"].ToString();
            string id = Resources["Set_Prof_ID"].ToString();
            addProfessor(new Professors(fName, lName, id));
        }
        public void removeProfessor(string name)
        {

        }
        // Classrooms
        public void addClassroom(ClassRoom room)
        {
            // Add Classroom to classroom list
            classrooms.Add(room);
            // Remove old Grids
            Grid child = FindName("MWF_") as Grid;
            MWF.Children.Remove(child);
            Grid child2 = FindName("TR_") as Grid;
            TR.Children.Remove(child2);
            // Redraw Grids
            drawTimeTables();
        }
        private void Btn_AddClassRoom_Click(object sender, RoutedEventArgs e)
        {
            AddClassRoomDialog addClassDialog = new AddClassRoomDialog();
            addClassDialog.ShowDialog();
            // If operation successful ------- IMPLEMENT CHECK 
            string bldg = Resources["Set_ClassRoom_Bldg"].ToString();
            int roomNum = Int32.Parse(Resources["Set_ClassRoom_Num"].ToString());
            int capacity = Int32.Parse(Resources["Set_ClassRoom_Seats"].ToString());
            addClassroom(new ClassRoom(bldg, roomNum, capacity));
        }
        public void removeClassroom(string classID)
        {

        }
        // Classes
        public void addClass(Classes newClass)
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
            AddClassDialog addClassDialog = new AddClassDialog();
            addClassDialog.ShowDialog();
            // If operation successful ------- IMPLEMENT CHECK 
            int crn = Int32.Parse(Resources["Set_Class_CRN"].ToString());
            string dpt = Resources["Set_Class_Dept"].ToString();
            int number = Int32.Parse(Resources["Set_Class_Number"].ToString());
            int sect = Int32.Parse(Resources["Set_Class_Section"].ToString());
            string name = Resources["Set_Class_Name"].ToString();
            int credits = Int32.Parse(Resources["Set_Class_Credits"].ToString());
            string prof = Resources["Set_Class_Professor"].ToString();
            bool online = Boolean.Parse(Resources["Set_Class_Online"].ToString());
            /*
             Classes(int crn, string deptName, int classNum, int secNum, string className, int credits,
                string classDay, Timeslot startTime, int seatsTaken, ClassRoom classroom, Professors professor, bool online)
             */
            addClass(new Classes(crn, dpt, number, sect, name, credits, "", new Timeslot(), 0, new ClassRoom(), determineProfessor(prof), online));

        }
        public void removeClass(int crn)
        {

        } // !!! make sure to remove from Classlist + GUI + Unassigned

        // Utility functions
        public RGB_Color stringToRGB(string s)
        {
            RGB_Color color;
            String[] parts = s.Split('.');
            color = new RGB_Color(Byte.Parse(parts[0]), Byte.Parse(parts[1]), Byte.Parse(parts[2]));
            return color;
        }
        public Timeslot determineTime(string startTime, string classDay)
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
        public ClassRoom determineClassroom(string building, int roomNum)
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
        public Professors determineProfessor(string fullName)
        {
            for (int i = 0; i < professors.Count; i++)
            {
                if (professors[i].FullName == fullName)
                {
                    return professors[i];
                }
            }
            MessageBox.Show("DEBUG: Couldnt find the referenced professor!");
            return new Professors();
        }
        public Classes determineClass(int crn)
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

        // DRAG/DROP functionality
        void MouseMoveOnGridRow(object sender, MouseEventArgs e) // Handles DRAG operation on unassigned classes list item
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
            if (sourceLabel != null)
            {
                int classIndex = (int)e.Data.GetData(typeof(int));
                // add the info to the target Label
                Label receiver = sender as Label;
                string days = receiver.Name.Split('_')[0];
                string start = receiver.Name.Split('_')[1];
                string roomInfo = receiver.Name.Split('_')[2];
                string bldg = roomInfo.Substring(0, 3);
                int room = Int32.Parse(roomInfo.Substring(3, (roomInfo.Length - 3)));
                classList[classIndex].ClassDay = days;
                classList[classIndex].StartTime = determineTime(start, days);
                classList[classIndex].Classroom = determineClassroom(bldg, room);
                // Give the newLabel the class information
                receiver.Content = sourceLabel.Content;
                receiver.Background = sourceLabel.Background;
                receiver.Tag = sourceLabel.Tag;

                // clear the sourceLabel
                sourceLabel.Content = "";
                RGB_Color white_bg = new RGB_Color(255, 255, 255);
                sourceLabel.Background = white_bg.colorBrush2;
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
                            // Display message box
                            MessageBox.Show(messageBoxText, caption, button, icon);
                            // Process message box results
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
                    Label receiver = sender as Label;
                    if (!classList[classIndex].Online)
                    {
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
                    string days = receiver.Name.Split('_')[0];
                    string start = receiver.Name.Split('_')[1];
                    string roomInfo = receiver.Name.Split('_')[2];
                    string bldg = roomInfo.Substring(0, 3);
                    int room = Int32.Parse(roomInfo.Substring(3, (roomInfo.Length - 3)));
                    classList[classIndex].ClassDay = days;
                    classList[classIndex].StartTime = determineTime(start, days);
                    classList[classIndex].Classroom = determineClassroom(bldg, room);
                    // Give the Label the class information
                    receiver.Content = classList[classIndex].TextBoxName;
                    receiver.Background = classList[classIndex].Prof.Prof_Color;
                    receiver.Tag = classCRN;
                }
            }
        }
        void MouseMoveOnAssignedClass(object sender, MouseEventArgs e) // Handles DRAG operation on assigned classes box
        {
            Label labelUnderMouse = sender as Label;
            int classIndex = -1;
            if (labelUnderMouse != null && e.LeftButton == MouseButtonState.Pressed)
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
                    Classes theClass = determineClass(classCRN);
                    if (theClass.Online)
                    {
                        string messageBoxText = "Are you sure you want to change this class\nfrom Online to In-Class?\n\n(You can later drag it back to the online class list to revert changes)";
                        string caption = "Online class alteration";
                        MessageBoxButton button = MessageBoxButton.YesNoCancel;
                        MessageBoxImage icon = MessageBoxImage.Question;
                        // Display message box
                        MessageBox.Show(messageBoxText, caption, button, icon);
                        // Process message box results
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
                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
                // Process message box results
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
                        // add the class to unassigned class list
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
                    Classes theClass = determineClass(classCRN);
                    if (!theClass.Online)
                    {
                        string messageBoxText = "Are you sure you want to change this\nIn-Class class to Online format?\n\n(You can later drag it back to the unassigned class list to revert changes)";
                        string caption = "Online class alteration";
                        MessageBoxButton button = MessageBoxButton.YesNoCancel;
                        MessageBoxImage icon = MessageBoxImage.Question;
                        // Display message box
                        MessageBox.Show(messageBoxText, caption, button, icon);
                        // Process message box results
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

        // Professor + Color pairings (Used for persistent memory storage in xml file)
        public class pairs
        {
            [XmlArray("colorPairings"), XmlArrayItem(typeof(ProfColors), ElementName = "ProfColors")]
            public List<ProfColors> colorPairings { get; set; }
        } 
        [XmlRoot("pairs")]
        public class ProfColors
        {
            public string ProfName { get; set; }
            public string Color { get; set; }
        }
    }

}
