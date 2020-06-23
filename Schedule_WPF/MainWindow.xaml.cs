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

namespace Schedule_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ////////////// GLOBAL VARIABLES ////////////////
        Timeslot[] times_MWF = { new Timeslot("08:00 AM","08"), new Timeslot("09:00 AM", "09"), new Timeslot("10:00 AM", "10"), new Timeslot("11:00 AM", "11"), new Timeslot("12:00 PM", "12"), new Timeslot("01:00 PM", "01"), new Timeslot("02:00 PM", "02"), new Timeslot("03:00 PM", "03"), new Timeslot("04:00 PM", "04"), new Timeslot("05:00 PM", "05"), new Timeslot("06:00 PM", "06") };
        Timeslot[] times_TR = { new Timeslot("08:00 AM", "08"), new Timeslot("09:30 AM", "09"), new Timeslot("11:00 AM", "11"), new Timeslot("12:30 PM", "12"), new Timeslot("02:00 PM", "02"), new Timeslot("03:30 PM", "03"), new Timeslot("05:00 PM", "05") };
        ObservableCollection<ClassRoom> classrooms = new ObservableCollection<ClassRoom>(new ClassRoom[] { new ClassRoom("ATS", 215), new ClassRoom("ATS", 347), new ClassRoom("ATS", 117), new ClassRoom("ATS", 999) });
        ProfessorList professors = new ProfessorList();
        RGB_Color[] colorPalette = { new RGB_Color(244,67,54), new RGB_Color(156,39,176), new RGB_Color(63,81,181), new RGB_Color(3,169,244), new RGB_Color(0,150,136), new RGB_Color(139,195,74), new RGB_Color(255,235,59), new RGB_Color(255,152,0), new RGB_Color(233,30,99), new RGB_Color(103,58,183), new RGB_Color(33,150,243), new RGB_Color(0,188,212), new RGB_Color(76,175,80), new RGB_Color(205,220,57), new RGB_Color(255,193,7), new RGB_Color(255,87,34) };
        pairs colorPairs;
        ClassList classList = new ClassList();
        ClassList unassignedClasses = new ClassList(1);
        ClassList onlineClasses = new ClassList(1);


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

            // Bind classlist to the "Classes" tab of the GUI
            bindClassList();
            bindProfList();
        }

        public void readExcel() // Read from excel to fill up classList + classrooms + professors (Called by MainWindow)
        {

        }

        public void drawTimeTables() // Draw the GUI grids for MWF / TR (Called by MainWindow)
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
                    timeLabel.Content = times[i - 1].Time;
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
                    if (classList[i].StartTime != "--" && classList[i].Classroom.Location != "N/A")
                    {
                        string targetBoxID = days + '_' + classList[i].StartTime + '_' + classList[i].Classroom.ClassID;
                        //MessageBox.Show(targetBoxID);
                        //Label targetBox = Resources[targetBoxID] as Label;
                        Label lbl = (Label)FindName(targetBoxID);
                        // !!!!! ------ VALIDATION (Already contains class?) -------- !!!!!!! //
                        lbl.Content = classList[i].TextBoxName;
                        lbl.Background = classList[i].Prof.Prof_Color; // !! AINT BEING ASSIGNED YET!
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
                        //MessageBox.Show("fillUnassigned() -> Adding " + classList[i].TextBoxName + "To unassigned list.");
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
            string fName = Resources["Set_Prof_FN"].ToString();
            string lName = Resources["Set_Prof_LN"].ToString();
            string id = Resources["Set_Prof_ID"].ToString();
            addProfessor(new Professors(fName, lName, id));
        }
        public void removeProfessor(string name)
        {

        }
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
            string bldg = Resources["Set_ClassRoom_Bldg"].ToString();
            int roomNum = Int32.Parse(Resources["Set_ClassRoom_Num"].ToString());
            addClassroom(new ClassRoom(bldg, roomNum));
        }
        public void removeClassroom(string classID)
        {

        }
        public void addClass(Classes _class)
        {

        }
        private void Btn_AddClass_Click(object sender, RoutedEventArgs e)
        {

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
                classList[classIndex].StartTime = start;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                classList[classIndex].Classroom = new ClassRoom(bldg, room);
                // Give the newLabel the class information
                MessageBox.Show("Here!");
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
                    //MessageBox.Show(receiver.Name);
                    string days = receiver.Name.Split('_')[0];
                    string start = receiver.Name.Split('_')[1];
                    string roomInfo = receiver.Name.Split('_')[2];
                    string bldg = roomInfo.Substring(0, 3);
                    int room = Int32.Parse(roomInfo.Substring(3, (roomInfo.Length - 3)));
                    classList[classIndex].ClassDay = days;
                    classList[classIndex].StartTime = start;
                    classList[classIndex].Classroom = new ClassRoom(bldg, room);
                    // Give the Label the class information
                    receiver.Content = classList[classIndex].TextBoxName;
                    receiver.Background = classList[classIndex].Prof.Prof_Color;
                    receiver.Tag = classCRN;
                }
            }
            Full_Classes_Grid.Items.Refresh();
            Full_Professors_Grid.Items.Refresh();
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
                classList[classIndex].StartTime = "--";
                classList[classIndex].isAssigned = false;
                unassignedClasses.Add(classList[classIndex]);
            }
            else
            {
                int classCRN = 0;
                DataGridRow droppedRow = (DataGridRow)e.Data.GetData(typeof(DataGridRow));
                if (droppedRow != null)
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
                            // User pressed Yes button
                            TextBlock crn_number = Unassigned_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                            //MessageBox.Show(crn_number.Text);
                            classCRN = Int32.Parse(crn_number.Text);
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
            Full_Classes_Grid.Items.Refresh();
            Full_Professors_Grid.Items.Refresh();
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
                        classList[classIndex].StartTime = "--";
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
                            // Add the class item to the online class list
                            TextBlock crn_number = Online_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                            //MessageBox.Show(crn_number.Text);
                            int classCRN = Int32.Parse(crn_number.Text);
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
            Full_Classes_Grid.Items.Refresh();
            Full_Professors_Grid.Items.Refresh();
        }

        ////////////// CUSTOM CLASSES //////////////////
        // ClassRoom
        public class ClassRoom
        {
            private int AvailableSeats;
            private int SeatsTaken;
            private int SeatsLeft;
            private int _RoomNum;
            private string ClassLocation;
            private string classID;

            public ClassRoom()
            {
                _RoomNum = 000;
                AvailableSeats = -1;
                SeatsTaken = -1;
                SeatsLeft = -1;
                ClassLocation = "N/A";
                classID = ClassLocation + _RoomNum;
            }

            public ClassRoom(string bldg, int num)
            {
                _RoomNum = num;
                AvailableSeats = -1;
                SeatsTaken = -1;
                SeatsLeft = -1;
                ClassLocation = bldg;
                classID = ClassLocation + _RoomNum;
            }

            public string ClassID { get { return classID; } }
            public string Location { get { return ClassLocation; } set { ClassLocation = value; classID = ClassLocation + _RoomNum; } }
            public int RoomNum { get { return _RoomNum; } set { _RoomNum = value; classID = ClassLocation + _RoomNum; } }
        }

        // Professors
        public class Professors : INotifyPropertyChanged
        {
            // VARIABLES RELATED TO THE PROFESSOR
            private string profFirstName;
            private string profLastName;
            private string profSRUID;
            private RGB_Color profColor;

            // Empty Professor constructor
            public Professors()
            {
                profFirstName = "None";
                profLastName = "None";
                profSRUID = "---";
                profColor = new RGB_Color(255, 255, 255);
            }

            // CONSTRUCTOR FOR ADDING PROFESSORS
            public Professors(string profFN, string profLN, string profID)
            {
                profFirstName = profFN;
                profLastName = profLN;
                profSRUID = profID;
                profColor = new RGB_Color(255, 255, 255);
            }

            public string FirstName { get { return profFirstName; } }
            public string LastName { get { return profLastName; } }
            public string FullName { get { return profLastName + ", " + profFirstName; } }
            public string SRUID { get { return profSRUID; } }
            public Brush Prof_Color { get { return profRGB.colorBrush2; } }
            public string colorString { get { return profColor.colorString; } }
            public RGB_Color profRGB { get { return profColor; } set { profColor = value; } }

            public event PropertyChangedEventHandler PropertyChanged;

        }

        public class ProfessorList : ObservableCollection<Professors>
        {
            public ProfessorList() : base()
            {
                Add(new Professors("Sam", "Thangiah", "A09999"));
                Add(new Professors("Abdullah", "Wahbeh", "A01223"));
                Add(new Professors("Raed", "Seetan", "A01717"));
                Add(new Professors("Nitin", "Sukhija", "A07819"));
                Add(new Professors("Yili", "Tseng", "A09192"));
                Add(new Professors("Deborah", "Whitfield", "A06486"));
            }

        }

        // Classes
        public class Classes : INotifyPropertyChanged
        {
            // VARIABLES RELATED TO THE CLASS
            private int _ClassNumber;
            private int _SectionNumber;
            private int _CRN;
            private int _Credits;
            private int _AvailableSeats;
            private int _SeatsTaken;
            private int _SeatsLeft;
            private string _StartTime;
            private string _ClassDay;
            private string _DeptName;
            private string _ClassName;
            private bool _Online;
            private bool _inCoursesBox;
            private bool _Assigned;
            private Professors _Prof;
            private ClassRoom _Classroom;

            // INITIAL CONSTRUCTOR (FROM EXCEL SHEET)
            public Classes(int crn, string deptName, int classNum, int secNum, string className, int credits,
                string classDay, string startTime, int availableSeats, int seatsTaken, ClassRoom classroom, Professors professor, bool online)
            {
                CRN = crn;
                _DeptName = deptName;
                _ClassNumber = classNum;//these marks indicate the excel's format must be a certain way for these columns
                _SectionNumber = secNum;
                _ClassName = className;
                _Credits = credits;
                _ClassDay = classDay;//
                _StartTime = startTime.Substring(0, 2);//
                _AvailableSeats = availableSeats;
                _SeatsTaken = seatsTaken;
                _SeatsLeft = _AvailableSeats - _SeatsTaken;
                _Classroom = classroom;
                _Prof = professor;//
                _Assigned = false;
                _Online = online;

                //printClasses();
            }

            // CONSTRUCTOR FOR ADDING CLASSES
            public Classes(int classNum, int secNum, string classDay, string startTime, Professors professor, string deptName,
                string className, int crn, int credits, int availableSeats, int seatsTaken, bool addAClass)
            {
                _DeptName = deptName;
                _ClassNumber = classNum;//
                _SectionNumber = secNum;
                _ClassDay = classDay;//
                _StartTime = startTime;//
                _Prof = professor;//
                _ClassName = className;
                _CRN = crn;
                _Credits = credits;
                _AvailableSeats = availableSeats;
                _SeatsTaken = seatsTaken;
                _SeatsLeft = _AvailableSeats - _SeatsTaken;

                //Application.OpenForms["Form2"].Close();

                //MessageBox.Show(Form.ActiveForm.ToString());
                //!!!!addClass(this);

            }

            public void printClasses()
            {
                Console.WriteLine("\n\nProfessor: " + _Prof.FullName
                    + "\nClass Number: " + _ClassNumber + "-" + _SectionNumber);
                if (Classroom.RoomNum == -100)
                {
                    Console.Write("Room Number: Off-Campus");
                }
                else if (Classroom.RoomNum == -200)
                {
                    Console.Write("Room Number: EMPTY");
                }
                else
                {
                    Console.Write("Room Number: " + Classroom.RoomNum);
                }
                Console.WriteLine("\nClass Days: " + _ClassDay
                    + "\nClass Time: " + StartTime
                    + "\nDepartment: " + _DeptName
                    + "\nSection: " + _SectionNumber
                    + "\nClass Name: " + _ClassName
                    + "\nCRN: " + _CRN
                    + "\nCredits: " + _Credits
                    + "\nAvailable Seats: " + _AvailableSeats
                    + "\nSeats Taken: " + _SeatsTaken
                    + "\nSeats Remaining: " + _SeatsLeft
                    + "\nClass Location: " + Classroom.Location);
            }

            public string DeptName { get { return _DeptName; } set { _DeptName = value; } }
            public int ClassNumber { get { return _ClassNumber; } set { _ClassNumber = value; } }
            public int SectionNumber { get { return _SectionNumber; } set { _SectionNumber = value; } }
            public string ClassName { get { return _ClassName; } set { _ClassName = value; } }
            public string ClassDay { get { return _ClassDay; } set { _ClassDay = value; } }
            public string StartTime { get { return _StartTime; } set { _StartTime = value; } }
            public int AvailableSeats { get { return _AvailableSeats; } set { _AvailableSeats = value; } }
            public int SeatsTaken { get { return _SeatsTaken; } set { _SeatsTaken = value; } }
            public int SeatsLeft { get { return _SeatsLeft; } set { _SeatsLeft = value; } }
            public int Credits { get { return _Credits; } set { _Credits = value; } }
            public int CRN { get { return _CRN; } set { _CRN = value; } }
            public bool Online { get { return _Online; } set { _Online = value; } }
            public bool inCoursesBox { get { return _inCoursesBox; } set { _inCoursesBox = value; } }
            public bool isAssigned { get { return _Assigned; } set { _Assigned = value; } }
            public Professors Prof { get { return _Prof; } set { _Prof = value; } }
            public ClassRoom Classroom { get { return _Classroom; } set { _Classroom = value; } }
            public string TextBoxName { get { return DeptName + " " + ClassNumber + " [" + SectionNumber + "]"; } }
            public string ProfessorName { get { return Prof.FullName; } }
            public string BuildingName { get { return Classroom.Location; } }
            public int RoomNumber { get { return Classroom.RoomNum; } }

            

            public event PropertyChangedEventHandler PropertyChanged;

            public void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

        }

        public class ClassList : ObservableCollection<Classes>
        {
            public ClassList() : base()
            {
                Add(new Classes(90210, "CPSC", 246, 01, "Advanced Programming Principles", 3, "MWF", "10:00", 40, 0, new ClassRoom("ATS", 215), new Professors("Sam", "Thangiah", "A09999"), false));
                Add(new Classes(1078, "CPSC", 217, 02, "Structured & Dynamic Web Programming", 3, "MWF", "11:00", 40, 0, new ClassRoom("ATS", 347), new Professors("Abdullah", "Wahbeh", "A01223"), false));
                Add(new Classes(2099, "CPSC", 311, 01, "Discrete Computational Structures", 3, "TR", "02:00", 40, 0, new ClassRoom("ATS", 117), new Professors("Raed", "Seetan", "A01717"), false));
                Add(new Classes(1097, "CPSC", 400, 01, "Computer Networks", 3, "TR", "12:30", 40, 0, new ClassRoom("ATS", 999), new Professors("Nitin", "Sukhija", "A07819"), false));
                Add(new Classes(10945, "CPSC", 374, 02, "Administration & Security", 3, "", "--", 40, 0, new ClassRoom(), new Professors("Yili", "Tseng", "A09192"), false));
                Add(new Classes(16002, "CPSC", 278, 02, "Programming Language & Theory", 3, "", "--", 40, 0, new ClassRoom(), new Professors("Deborah", "Whitfield", "A06486"), false));
                Add(new Classes(8501, "CPSC", 300, 01, "Challenges of Computing", 3, "", "--", 40, 0, new ClassRoom(), new Professors("Raed", "Seetan", "A01717"), true));
            }
            public ClassList(int n)
            {

            }
        }

        // RGB_Color class
        public class RGB_Color
        {
            private byte R;
            private byte G;
            private byte B;

            public RGB_Color()
            {
                R = 50;
                G = 50;
                B = 50;
            }
            public RGB_Color(byte r, byte g, byte b)
            {
                R = r;
                G = g;
                B = b;
            }

            public string colorString { get { return ("" + R + "." + G + "." + B); } }
            public Color colorBrush { get { return Color.FromRgb(R, G, B); } }
            public Brush colorBrush2 { get { return new SolidColorBrush(Color.FromRgb(R, G, B)); } }
        }

        // Time class
        public class Timeslot
        {
            public Timeslot(string time, string timeID)
            {
                Time = time;
                TimeID = timeID;
            }

            public string Time { get; set; }
            public string TimeID { get; set; }
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
