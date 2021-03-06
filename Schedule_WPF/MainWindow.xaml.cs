﻿using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using Schedule_WPF.Properties;
using System.ComponentModel;
using Schedule_WPF.Models;
using Microsoft.Win32;
using ClosedXML.Excel;
using System.Linq;
using System.Security.Cryptography;                                                      
using System.Text;
using System.Windows.Data;
using System.Globalization;

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
        ProfessorPreferenceList professorPreferences = (ProfessorPreferenceList)Application.Current.FindResource("ProfPreference_List_View");
        ClassGroupList classGroupings = (ClassGroupList)Application.Current.FindResource("ClassGroup_List_View");
        ClassList classList = (ClassList)Application.Current.FindResource("Classes_List_View");
        ClassList unassignedClasses = (ClassList)Application.Current.FindResource("Unassigned_Classes_List_View");
        ClassList onlineClasses = (ClassList)Application.Current.FindResource("Online_Classes_List_View");
        ClassList appointmentClasses = (ClassList)Application.Current.FindResource("Appointment_Classes_List_View");
        ClassList appointment2Classes = (ClassList)Application.Current.FindResource("Appointment2_Classes_List_View");
        ClassList deletedClasses = (ClassList)Application.Current.FindResource("Deleted_Classes_List_View");
        List<string> excelHeaders = new List<string>();
        List<Type> excelTypes = new List<Type>();
        List<ClassesHash> hashedClasses = new List<ClassesHash>();
        string filePath, latestHashDigest, colorFilePath;
        RGB_Color[] colorPalette = { new RGB_Color(244, 67, 54), new RGB_Color(156, 39, 176), new RGB_Color(63, 81, 181), new RGB_Color(3, 169, 244), new RGB_Color(0, 150, 136), new RGB_Color(139, 195, 74), new RGB_Color(255, 235, 59), new RGB_Color(255, 152, 0), new RGB_Color(233, 30, 99), new RGB_Color(103, 58, 183), new RGB_Color(33, 150, 243), new RGB_Color(0, 188, 212), new RGB_Color(76, 175, 80), new RGB_Color(205, 220, 57), new RGB_Color(255, 193, 7), new RGB_Color(255, 87, 34) };
        Pairs colorPairs = (Pairs)Application.Current.FindResource("ColorPairs_List_View");
        string term, termString, termYear;

        ////////////// START OF EXECUTION ////////////////

        public MainWindow()
        {
            InitializeComponent();
            filePath = Application.Current.Resources["FilePath"].ToString(); // make local copy of path to excel file (initialized by FileSelect window)
            ReadExcel(filePath);
            AssignProfColors();
            DrawTimeTables();
            FillDerivedLists();
            UpdateProfessorCapacity();
            BindData();
            GenerateClassListHashes();
            latestHashDigest = ComputeSha256Hash(classList.Serialize()); // initialize hash digest of classlist (used to see if changes have been made before closing application)
            Helper.CloseUniqueWindow<FileSelect>(); // Close File Select window
        }

        ////////////// FUNCTIONS ////////////////

        public void ReadExcel(string file) // Read excel file, create classes objects and append them to classList 
        {

            using (var excelWorkbook = new XLWorkbook(file))
            {
                // Select Worksheet
                var worksheet = excelWorkbook.Worksheet(1);
                int columns = 33;
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                // Determine term
                string termCode = worksheet.Row(2).Cell(1).GetValue<string>();
                if (termCode.Length == 6)
                {
                    termYear = termCode.Substring(0, termCode.Length - 2);
                    TermYearBox.Text = termYear;
                    term = termCode.Substring(termCode.Length - 2);
                    switch (term)
                    {
                        case "01":
                            termString = "Spring";
                            TermComboBox.SelectedIndex = 0;
                            break;
                        case "06":
                            termString = "Summer";
                            TermComboBox.SelectedIndex = 1;
                            break;
                        case "09":
                            termString = "Fall";
                            TermComboBox.SelectedIndex = 2;
                            break;
                        case "12":
                            termString = "Winter";
                            TermComboBox.SelectedIndex = 3;
                            break;
                        default:
                            termString = "None";
                            TermComboBox.SelectedIndex = 0;
                            break;
                    }
                    //MessageBox.Show("Term: " + term + "\nYear: " + termYear);
                }
                else
                {
                    term = "00";
                    termString = "None";
                    termYear = "0000";
                }

                // Populate excel headers array
                var headerRow = worksheet.Row(1);
                string cellValue = "";
                for (int i = 0; i < columns; i++)
                {
                    cellValue = headerRow.Cell(i + 1).GetValue<string>();
                    for (int n = 0; n < excelHeaders.Count; n++)
                    {
                        if (excelHeaders[n].ToUpper() == cellValue.ToUpper()) // if there is a duplicate column name
                        {
                            cellValue = cellValue + "(2)";
                            break;
                        }
                    }
                    excelHeaders.Add(cellValue);
                }

                // Create Professors
                int sruid_indexer = 0;
                bool professorFound;
                foreach (var row in rows)
                {
                    string fullName, lastName, firstName, SRUID;
                    if (!row.Cell(22).IsEmpty())
                    {
                        fullName = row.Cell(22).GetValue<string>();
                        if (fullName != "" && fullName.Contains(","))
                        {
                            professorFound = false;
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
                                if (!row.Cell(23).IsEmpty() && row.Cell(23).GetValue<string>().Length == 9)
                                {
                                    SRUID = row.Cell(23).GetValue<string>();
                                }
                                else
                                {
                                    SRUID = "A0" + sruid_indexer;
                                    sruid_indexer++;
                                }
                                if (firstName != "None")
                                {
                                    professors.Add(new Professors(firstName, lastName, SRUID));
                                }
                            }
                        }
                    }
                }

                // Create Classrooms
                int parseResult, room, capacity;
                bool classroomFound;
                string bldg;
                foreach (var row in rows)
                {
                    room = -1;
                    capacity = 0;
                    if (!row.Cell(20).IsEmpty())
                    {
                        bldg = row.Cell(20).GetValue<string>().ToUpper();
                        if (bldg != "WEB" && !bldg.Contains("APPT"))
                        {
                            if (!row.Cell(21).IsEmpty() && int.TryParse(row.Cell(21).GetValue<string>(), out parseResult))
                            {
                                room = parseResult;
                                if (!row.Cell(19).IsEmpty() && int.TryParse(row.Cell(19).GetValue<string>(), out parseResult))
                                {
                                    capacity = parseResult;
                                }
                            }
                            classroomFound = false;
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
                                //MessageBox.Show("ROOM ADDED: " + bldg + " " + room + " " + capacity);
                            }
                        }
                    }
                }

                // Create Classes
                int ClassNum, Section, Credits, SeatsTaken;
                int duplicate_CRN_indexer = -1;
                string CRN, Dept, ClassName, ClassDay, classID, profName, notes, sectionNotes;
                bool Online, Appoint, Changed;
                List<string> CRN_List = new List<string>();
                List<string> CrossListCodes = new List<string>();
                foreach (var row in rows)
                {
                    ClassNum = -1;
                    Section = -1;
                    Credits = 0;
                    SeatsTaken = 0;
                    Dept = "";
                    ClassName = "";
                    ClassDay = "";
                    Professors prof = new Professors();
                    ClassRoom classroom = new ClassRoom();
                    Timeslot time = new Timeslot();
                    Online = false;
                    Appoint = false;
                    Changed = false;
                    notes = "";
                    sectionNotes = "";

                    // CRN 
                    // Primary Key, if CRN is empty, do not enter record.
                    // If CRN is not a number, assign a unique negative value, tracked by duplicate_CRN_indexer
                    if (!row.Cell(6).IsEmpty())
                    {
                        parseResult = -1;
                        if (int.TryParse(row.Cell(6).GetValue<string>(), out parseResult))
                        {
                            CRN = row.Cell(6).GetValue<string>();
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
                                MessageBox.Show("Duplicate CRN found in excel file./nCRN: " + CRN);
                                CRN = duplicate_CRN_indexer.ToString(); // HANDLE THIS BETTER
                                duplicate_CRN_indexer--;
                            }
                        }
                        else
                        {
                            string textCRN = row.Cell(6).GetValue<string>().ToUpper();
                            if (textCRN == "NEW")
                            {
                                CRN = textCRN;
                            }
                            else
                            {
                                MessageBox.Show("CRN field is not a number or a string!./nCRN: " + row.Cell(6).GetValue<string>());
                                CRN = duplicate_CRN_indexer.ToString();
                                duplicate_CRN_indexer--;
                            }
                        }
                        // DEPT
                        if (!row.Cell(3).IsEmpty())
                        {
                            Dept = row.Cell(3).GetValue<string>().ToUpper();
                        }
                        // CLASS NUM
                        if (!row.Cell(3).IsEmpty())
                        {
                            if (int.TryParse(row.Cell(4).GetValue<string>(), out parseResult))
                            {
                                ClassNum = parseResult;
                            }
                        }
                        // CLASS NAME
                        if (!row.Cell(7).IsEmpty())
                        {
                            ClassName = row.Cell(7).GetValue<string>();
                        }
                        // SECTION
                        if (!row.Cell(5).IsEmpty())
                        {
                            if (int.TryParse(row.Cell(5).GetValue<string>(), out parseResult))
                            {
                                if (parseResult > 0)
                                {
                                    Section = parseResult;
                                }
                            }
                        }
                        // CREDITS
                        if (!row.Cell(9).IsEmpty())
                        {
                            Credits = row.Cell(9).GetValue<int>();
                        }
                        // SEATS TAKEN
                        if (!row.Cell(13).IsEmpty())
                        {
                            SeatsTaken = row.Cell(13).GetValue<int>();
                        }
                        // CLASSDAY
                        if (!row.Cell(16).IsEmpty())
                        {
                            ClassDay = row.Cell(16).GetValue<string>().ToUpper();
                        }
                        // Determine Professor
                        if (!row.Cell(22).IsEmpty())
                        {
                            profName = row.Cell(22).GetValue<string>();
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
                        if (!row.Cell(20).IsEmpty())
                        {
                            bldg = row.Cell(20).GetValue<string>().ToUpper();
                            if (bldg != "WEB" && !bldg.Contains("APPT"))
                            {
                                room = -1;
                                if (!row.Cell(21).IsEmpty())
                                {
                                    if (int.TryParse(row.Cell(21).GetValue<string>(), out parseResult))
                                    {
                                        room = parseResult;
                                    }
                                }
                                classID = bldg + room;
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
                                    classroom = new ClassRoom("WEB", 999);
                                    Online = true;
                                }
                                else if (bldg.Contains("APPT"))
                                {
                                    if (bldg == "APPT")
                                    {
                                        classroom = new ClassRoom("APPT", 0);
                                    }
                                    else if (bldg == "APPT2")
                                    {
                                        classroom = new ClassRoom("APPT2", 0);
                                    }
                                    Appoint = true;
                                }
                            }
                        }
                        // Determine TimeSlot
                        if (!row.Cell(17).IsEmpty())
                        {
                            string rawTime = row.Cell(17).GetValue<string>();
                            string timePart = formatTime(rawTime.Split(' ')[0]);
                            time = DetermineTime(timePart, ClassDay);
                        }
                        // Determine if it is higlighted red (changed) or not in the excel file
                        if (row.Cell(1).Style.Fill.BackgroundColor == XLColor.FromHtml("#FFFFCFCF"))
                        {
                            //MessageBox.Show("Excel Read: Class set to RED");
                            Changed = true;
                        }
                        // Crosslist handling
                        bool isCrossFirst = false;
                        string crossCode = row.Cell(8).GetValue<string>();
                        if (crossCode != "")
                        {
                            for (int i = 0; i < CrossListCodes.Count; i++)
                            {
                                if (crossCode == CrossListCodes[i])
                                {
                                    break;
                                }
                                if (i == (CrossListCodes.Count - 1))
                                {
                                    isCrossFirst = true;
                                    CrossListCodes.Add(crossCode);
                                }
                            }
                        }
                        // Get remaining extra data
                        List<string> extras = new List<string>();
                        extras.Add(row.Cell(2).GetValue<string>()); // Session
                        extras.Add(row.Cell(8).GetValue<string>()); // CrossList
                        extras.Add(row.Cell(10).GetValue<string>()); // MaxSeats
                        extras.Add(row.Cell(11).GetValue<string>()); // WaitList
                        extras.Add(row.Cell(12).GetValue<string>()); // ProjList
                        extras.Add(row.Cell(14).GetFormattedString()); // CourseStartDate
                        extras.Add(row.Cell(15).GetFormattedString()); // CourseEndDate
                        for (int x = 24; x <= 31; x++) // last few misc fields
                        {
                            extras.Add(row.Cell(x).GetValue<string>());
                        }
                        sectionNotes = row.Cell(32).GetValue<string>();
                        notes = row.Cell(33).GetValue<string>();
                        string maxSeats = row.Cell(10).GetValue<string>();
                        string projSeats = row.Cell(12).GetValue<string>();
                        // Create class and add to classlist
                        Classes tmpClass = new Classes(CRN, Dept, ClassNum, Section, ClassName, Credits, ClassDay, time, SeatsTaken, classroom, prof, Online, Appoint, Changed, notes, sectionNotes, extras, maxSeats, projSeats);
                        tmpClass.isCrossFirst = isCrossFirst;
                        // check if it is a deleted class
                        if (row.Cell(1).Style.Font.Strikethrough == false)
                        {
                            classList.Add(tmpClass);
                        }
                        else
                        {
                            deletedClasses.Add(tmpClass);
                        }
                    }
                }
            }
        }
        public void DrawTimeTables() // Calls TimeTableSetup() for MWF and TR 
        {
            TimeTableSetup(MWF, times_MWF);
            TimeTableSetup(TR, times_TR);
        }
        public void TimeTableSetup(Grid parentGrid, Timeslot[] times) // Creates an empty GUI grid based on timeslots + classrooms, then calls PopulateTimeTable() 
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
        public void PopulateTimeTable(Grid timeTable, Timeslot[] times) // Populate a GUI grid based on classList 
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
                                if (classList[i].isHidden)
                                {
                                    lbl.Background = stripedBackground(classList[i].Prof.profRGB.colorBrush);
                                }
                                else
                                {
                                    lbl.Background = classList[i].Prof.Prof_Color;
                                }
                                lbl.Tag = classList[i].ClassID;
                                lbl.ContextMenu = Resources["ClassContextMenuGUI"] as ContextMenu;
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
        public void EmptyGrid(Grid timetable)  // Empties all entries of a GUI grid 
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
        public void FillDerivedLists() // Fill Unassigned/Online/APPT/APPT2 lists. (They are subsets of classList) 
        {
            // empty online and unassigned class lists
            unassignedClasses.Clear();
            onlineClasses.Clear();
            appointmentClasses.Clear();
            appointment2Classes.Clear();
            // add from classList
            for (int i = 0; i < classList.Count; i++)
            {
                if (!classList[i].isAssigned)
                {
                    if (classList[i].Online)
                    {
                        onlineClasses.Add(classList[i]);
                    }
                    else if (classList[i].isAppointment)
                    {
                        if (classList[i].Classroom.Location == "APPT")
                        {
                            appointmentClasses.Add(classList[i]);
                        }
                        else if (classList[i].Classroom.Location == "APPT2")
                        {
                            appointment2Classes.Add(classList[i]);
                        }
                        else
                        {
                            MessageBox.Show("DEBUG - ERROR: Couldnt assign appointed class to either APPT or APPT2");
                        }
                    }
                    else
                    {
                        //MessageBox.Show("fillUnassigned() -> Adding " + classList[i].TextBoxName + " to unassigned list.");
                        unassignedClasses.Add(classList[i]);
                    }
                }
            }
        }
        public void AssignProfColors() // Give professors a color key based on the palette defined above + Save assigned colors to XML file 
        {
            //MessageBox.Show("ColorIndex is currently: " + Settings.Default.ColorIndex);
            // Read from Colors file to see which professors we have already assigned a color. Store in colorPairings List.
            string tempPath = System.IO.Path.GetTempPath();
            string filename = "ColorConfigurations22.xml";
            colorFilePath = System.IO.Path.Combine(tempPath, filename);
            XmlSerializer ser = new XmlSerializer(typeof(Pairs));
            if (!File.Exists(colorFilePath))
            {
                Settings.Default.Reset();
                colorPairs = new Pairs();
                colorPairs.ColorPairings = new List<ProfColors>();

                using (FileStream fs = new FileStream(colorFilePath, FileMode.OpenOrCreate))
                {
                    ser.Serialize(fs, colorPairs);
                }
            }
            using (FileStream fs = new FileStream(colorFilePath, FileMode.OpenOrCreate))
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
                        RGB_Color tempColor = new RGB_Color((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));
                        while (isColorTaken(tempColor))
                        {
                            tempColor.R = (byte)rand.Next(256);
                            tempColor.G = (byte)rand.Next(256);
                            tempColor.B = (byte)rand.Next(256);
                        }
                        professors[i].profRGB = tempColor;
                    }
                    // Add it to pairings list
                    colorPairs.ColorPairings.Add(new ProfColors { ProfName = professors[i].FullName, Color = professors[i].profRGB.colorString });
                    //MessageBox.Show("Added " + professors[i].FullName + " + " + professors[i].profRGB.colorString);
                }
            }
            // Save changes to Colors.xml
            SerializePairs();
            // Save paletteIndex counter to application settings
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
        public void UpdatePairs() // Update colorPairs list to account for any additions
        {
            for (int i = 0; i < professors.Count; i++)
            {
                for (int n = 0; n < colorPairs.ColorPairings.Count; n++)
                {
                    if (colorPairs.ColorPairings[n].ProfName == professors[i].FullName)
                    {
                        break;
                    }
                    if (n == (colorPairs.ColorPairings.Count - 1))
                    {
                        //MessageBox.Show("Adding new color pair...\n\nProfessor: " + professors[i].FullName + "\nColor: " + professors[i].profRGB.colorString);
                        // Add prof + color pairing
                        colorPairs.ColorPairings.Add(new ProfColors { ProfName = professors[i].FullName, Color = professors[i].profRGB.colorString });
                    }
                }
            }
        }
        public void SerializePairs() // Save professor/color pairs to XML file 
        {
            // Update colorPairs to account for any new professors
            UpdatePairs();
            // Save changes to Colors.xml
            XmlSerializer ser = new XmlSerializer(typeof(Pairs));
            using (FileStream fs = new FileStream(colorFilePath, FileMode.OpenOrCreate))
            {
                ser.Serialize(fs, colorPairs);
            }
        }
        public void UpdateProfessorCapacity() // Update professor's numClasses and numPrep values
        {
            // for each professor
            for (int i = 0; i < professors.Count; i++) // Wont be a problem. professors wont + or -
            {
                // Reset class counters
                professors[i].NumClasses = 0;
                professors[i].NumPrep = 0;
                List<string> uniqueClasses = new List<string>();
                // check how many classes they are teaching
                for (int n = 0; n < classList.Count; n++) // Wont be a problem. class has already been added previously
                {
                    if (professors[i].SRUID == classList[n].Prof.SRUID)
                    {
                        bool unique = true;
                        //MessageBox.Show("" + uniqueClasses.Count);
                        
                        for (int j = 0; j < uniqueClasses.Count; j++)
                        {
                            if (uniqueClasses[j] == classList[n].ClassName)
                            {
                                unique = false;
                            }
                        }
                        
                        if (unique && !classList[n].excludeCredits)
                        {
                            if (classList[n].isCrossListed)
                            {
                                if (classList[n].isCrossFirst)
                                {
                                    professors[i].NumPrep++;
                                    professors[i].NumClasses += classList[n].Credits;
                                }
                                uniqueClasses.Add(classList[n].ClassName);
                            }
                            else
                            {
                                professors[i].NumPrep++;
                                professors[i].NumClasses += classList[n].Credits;
                                uniqueClasses.Add(classList[n].ClassName);
                            }
                        }
                        else if (!unique && !classList[n].excludeCredits)
                        {
                            if (classList[n].isCrossListed)
                            {
                                if (classList[n].isCrossFirst)
                                {
                                    professors[i].NumClasses += classList[n].Credits;
                                }
                            }
                            else
                            {
                                professors[i].NumClasses += classList[n].Credits;
                            }
                        }
                    }
                }
            }
        }
        public void BindData() // Bind class/professor lists to GUI data tables 
        {
            Online_Classes_Grid.ItemsSource = onlineClasses; // Online classes GUI list
            Unassigned_Classes_Grid.ItemsSource = unassignedClasses; // Unassigned classes GUI list
            Appointment_Classes_Grid.ItemsSource = appointmentClasses; // APPT classes GUI list
            Appointment2_Classes_Grid.ItemsSource = appointment2Classes; // APPT2 classes GUI list
            Professor_Key_List.ItemsSource = professors; // Professor Key GUI list
            Full_Classes_Grid.ItemsSource = classList;  // Classes GUI list (Classes tab)
            Full_Professors_Grid.ItemsSource = professors; // Professors GUI list (Professors tab)
        }
        public void RefreshGUI() // Empty GUI timetables, repopulate them and refresh derived lists 
        {
            Grid timetable_MWF = (Grid)FindName("MWF_");
            Grid timetable_TR = (Grid)FindName("TR_");
            EmptyGrid(timetable_MWF);
            EmptyGrid(timetable_TR);
            PopulateTimeTable(timetable_MWF, times_MWF);
            PopulateTimeTable(timetable_TR, times_TR);
            FillDerivedLists();
            UpdateProfessorCapacity();
            ProcessProfessorPreferences();
            ProcessClassGroupings();
        }
        public void SaveChanges() // Writes classList to an excel file 
        {
            string fileDir = getFileDirectory(Application.Current.Resources["FilePath"].ToString());
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = fileDir;
            saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";
            saveFileDialog.Title = "Save Excel File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                string fileName = saveFileDialog.FileName;
                XLWorkbook wb = new XLWorkbook();
                DataTable dt = getDataTableFromClasses();
                var ws = wb.Worksheets.Add(dt);

                // Colors
                XLColor empty = XLColor.NoColor;
                XLColor header = XLColor.FromHtml("#FF016648");
                XLColor edited = XLColor.FromHtml("#FFFFCFCF");
                XLColor added = XLColor.FromHtml("#FFD4FFC4");

                // Styling
                ws.Table(0).Theme = XLTableTheme.None;
                ws.Row(1).Style.Fill.BackgroundColor = header;
                ws.Row(1).Style.Font.Bold = true;
                ws.Row(1).Style.Font.FontColor = XLColor.White;
                /*
                ws.Column(7).AdjustToContents();
                ws.Column(22).AdjustToContents();
                ws.Column(23).AdjustToContents();
                */
                ws.Columns().AdjustToContents();
                ws.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                // Iterate over classList to format the background of each row appropriately
                for (int i = 0; i < classList.Count; i++)
                {
                    ws.Row(i + 2).Style.Fill.BackgroundColor = empty;
                    for (int j = 0; j < classList[i].ChangedData.Count; j++)
                    {
                        if (classList[i].ChangedData[j])
                        {
                            ws.Row(i + 2).Cell(j + 1).Style.Fill.BackgroundColor = edited;
                        }
                    }
                    /*
                    // match ClassID
                    for (int n = 0; n < hashedClasses.Count; n++)
                    {
                        if (classList[i].ClassID == hashedClasses[n].ClassID)
                        {
                            // if hash is different change color to edited
                            if (hashedClasses[n].Hash != ComputeSha256Hash(classList[i].Serialize()))
                            {
                                for (int j = 0; j < classList[i].ChangedData.Count; j++)
                                {
                                    if (classList[i].ChangedData[j])
                                    {
                                        ws.Row(i + 2).Cell(j + 1).Style.Fill.BackgroundColor = edited;
                                    }
                                }
                                //ws.Row(i + 2).Cell().Style.Fill.BackgroundColor = empty;
                            }
                            break;
                        }
                    }
                    */
                }
                // Iterate over deletedclasses to format the background of each row appropriately
                for (int i = 0; i < deletedClasses.Count; i++)
                {
                    ws.Row(classList.Count + i + 2).Style.Fill.BackgroundColor = edited;
                    ws.Row(classList.Count + i + 2).Style.Font.Strikethrough = true;
                    ws.Row(classList.Count + i + 2).Style.Font.FontColor = XLColor.Red;
                }

                wb.SaveAs(fileName);
            }
            SerializePairs();
        }
        public void Btn_SaveChanges_Click(object sender, RoutedEventArgs e) // Save changes button handler. Calls SaveChanges() 
        {
            SaveChanges();
            latestHashDigest = ComputeSha256Hash(classList.Serialize());
        }
        public void MainWindow_Closing(object sender, CancelEventArgs e) // Window close button handler. Prevents closing if user has unsaved changes 
        {
            string newDigest = ComputeSha256Hash(classList.Serialize());
            if (newDigest != latestHashDigest)
            {
                string messageBoxText = "You have unsaved changes!\nWould you like to Save and Exit?";
                string caption = "Unsaved changes";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Question;
                // Display + Process message box results
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveChanges();
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e) // When excludeCredits checkbox is clicked
        {
            RefreshGUI();
        }
        private void Browse_Prof_Prefs_Click(object sender, RoutedEventArgs e) // Locate file where professor preferences is stored
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                Professor_Preference_Source.Text = openFileDialog.FileName;
                try
                {
                    using (var excelWorkbook = new XLWorkbook(openFileDialog.FileName))
                    {
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Excel file is currently open!\n\nPlease close it before proceeding...");
                }
            }
        }
        private void Submit_Prof_Prefs_Click(object sender, RoutedEventArgs e) // Get data from professor preference spreadsheet and create the preference list
        {
            if (Professor_Preference_Source.Text == "")
            {
                MessageBox.Show("Please select a professor preference excel file first!");
            }
            else
            {
                //MessageBox.Show("Loading excel file into appropriate data structure...");
                try
                {
                    using (var excelWorkbook = new XLWorkbook(Professor_Preference_Source.Text))
                    {
                        // Select Worksheet
                        var worksheet = excelWorkbook.Worksheet(1);
                        var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
                        // Determine number of columns
                        var headerRow = worksheet.Row(1);
                        int columns = 3;
                        string headerString;
                        while (true)
                        {
                            headerString = headerRow.Cell(columns + 1).GetValue<string>();
                            if (headerString == "")
                            {
                                //MessageBox.Show("Ended prof col counts. Total : " + columns);
                                break;
                            }
                            else
                            {
                                columns++;
                            }
                        }
                        // Create one professor preference object for each professor in the preference file
                        string profName;
                        for (int i = 3; i < columns; i++)
                        {
                            profName = headerRow.Cell(i + 1).GetValue<string>(); // Currently last name is the best ID we have
                            professorPreferences.Add(new ProfessorPreference(profName));
                            //MessageBox.Show("Added Preference for Professor: " + profName);
                        }
                        // Create preference object for each class and add it to its respective professor preference list
                        string dept, classString;
                        int classNum;
                        foreach (var row in rows)
                        {
                            if (row.Cell(1).GetValue<string>() != "")
                            {
                                string code;
                                dept = row.Cell(1).GetValue<string>();
                                classString = row.Cell(2).GetValue<string>();
                                if (classString.Contains("/"))
                                {
                                    string[] classes = classString.Split('/');
                                    for (int i = 0; i < classes.Length; i++)
                                    {
                                        classNum = Int32.Parse(classes[i]);
                                        for (int n = 4; n <= columns; n++)
                                        {
                                            code = row.Cell(n).GetValue<string>();
                                            professorPreferences[n - 4].PreferenceList.Add(new Preference(dept, classNum, code));
                                            //MessageBox.Show("Added Preference\nProf: " + professorPreferences[n - 4].ProfessorID + "\nPrefs: " + dept + " " + classNum + " : " + code);
                                        }
                                    }
                                }
                                else
                                {
                                    classNum = row.Cell(2).GetValue<int>();
                                    for (int i = 4; i <= columns; i++)
                                    {
                                        code = row.Cell(i).GetValue<string>();
                                        professorPreferences[i - 4].PreferenceList.Add(new Preference(dept, classNum, code));
                                        //MessageBox.Show("Added Preference\nProf: " + professorPreferences[i - 4].ProfessorID + "\nPrefs: " + dept + " " + classNum + " : " + code);
                                    }
                                }
                            }
                        }
                    }
                    Loaded_URL_Preferences.Text = "Loaded file: " + Professor_Preference_Source.Text;
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Excel file is currently open!\n\nPlease close it before proceeding...");
                }
                MessageBox.Show("Preferences successfully submitted.");
                ProcessProfessorPreferences();
                RefreshGUI();
            }
        }
        private void ProcessProfessorPreferences() // Update classes to reflect the preferences of professors, (if any) 
        {
            // Update preference level for each class in classList
            for (int i = 0; i < classList.Count; i++)
            {
                // Try and find a preference for this class+professor combo (professors are identified by last name in preference list)
                string profID = classList[i].Prof.LastName;
                string dept = classList[i].DeptName;
                int num = classList[i].ClassNumber;
                for (int n = 0; n < professorPreferences.Count; n++) // find the prof
                {
                    if (profID == professorPreferences[n].ProfessorID)
                    {
                        for (int x = 0; x < professorPreferences[n].PreferenceList.Count; x++) // find the class
                        {
                            if (professorPreferences[n].PreferenceList[x].Dept == dept && professorPreferences[n].PreferenceList[x].ClassNum == num)
                            {
                                classList[i].PreferenceLevel = professorPreferences[n].PreferenceList[x].Sentiment;
                                //MessageBox.Show("" + classList[i].ClassID + " : " + classList[i].PreferenceLevel);
                                classList[i].PreferenceMessage = professorPreferences[n].PreferenceList[x].Message;
                                //MessageBox.Show("Found preference for " + profID + " in " + dept + " " + num);
                                classList[i].PreferenceCode = professorPreferences[n].PreferenceList[x].Code;
                                if (professorPreferences[n].PreferenceList[x].Message == "Taught before but prefer to teach on-line" && !classList[i].Online)
                                {
                                    classList[i].PreferenceLevel = -1;
                                }
                                else if (professorPreferences[n].PreferenceList[x].Message == "Prefer not to teach this class in the Fall" && termString != "Fall")
                                {
                                    classList[i].PreferenceLevel = 0;
                                }
                                else if (professorPreferences[n].PreferenceList[x].Message == "Prefer not to teach this class in the Spring" && termString != "Spring")
                                {
                                    classList[i].PreferenceLevel = 0;
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }
        private void Browse_Class_Groups_Click(object sender, RoutedEventArgs e) // Locate file where professor preferences is stored
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                Class_Groups_Source.Text = openFileDialog.FileName;
                try
                {
                    using (var excelWorkbook = new XLWorkbook(openFileDialog.FileName))
                    {
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Excel file is currently open!\n\nPlease close it before proceeding...");
                }
            }
        }
        private void Submit_Class_Groups_Click(object sender, RoutedEventArgs e) // Get data from professor preference spreadsheet and create the preference list
        {
            if (Class_Groups_Source.Text == "")
            {
                MessageBox.Show("Please select a class groupings excel file first!");
            }
            else
            {
                //MessageBox.Show("Loading excel file into appropriate data structure...");
                try
                {
                    using (var excelWorkbook = new XLWorkbook(Class_Groups_Source.Text))
                    {
                        // Select Worksheet
                        var worksheet = excelWorkbook.Worksheet(1);
                        var rows = worksheet.RangeUsed().RowsUsed();
                        //MessageBox.Show("" + rows.Count());

                        // Create a class group object for each row in grouping file
                        foreach (var row in rows)
                        {
                            if (row.Cell(1).GetValue<string>() != "")
                            {
                                ClassGroup group = new ClassGroup();
                                // Add the classes to the newly created group
                                bool rowEnd = false;
                                int i = 1;
                                while (!rowEnd)
                                {
                                    if (row.Cell(i).GetValue<string>() != "")
                                    {
                                        string classInfo = row.Cell(i).GetValue<string>();
                                        // Separte dept and classnum
                                        string dept = classInfo.Split(' ')[0].ToUpper();
                                        string num = classInfo.Split(' ')[1];
                                        // insert data into classgroup
                                        group.AddEntry(dept, num);
                                        i++;
                                    }
                                    else
                                    {
                                        rowEnd = true;
                                    }
                                }
                                classGroupings.Add(group);
                            }
                        }
                    }
                    Loaded_URL_Groups.Text = "Loaded file: " + Class_Groups_Source.Text;
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Excel file is currently open!\n\nPlease close it before proceeding...");
                }
                MessageBox.Show("Class groupings successfully submitted.");
                RefreshGUI();
            }
        }
        private void ProcessClassGroupings() // Update classes to reflect the preferences of professors, (if any) 
        {
            string oldText = soft_constraint_log.Text;
            soft_constraint_log.Text = "";
            // Go through classlist and see if any class is scheduled at the same time as another in the same group
            for (int i = 0; i < classList.Count; i++)
            {
                for (int n = 0; n < classGroupings.Count; n++)
                {
                    if (isGroupMember(classList[i], n))
                    {
                        // class found inside this particular group -> check if scheduled at the same day / time as others
                        checkGroupConflict(classList[i], n);
                    }
                }
            }
            if (soft_constraint_log.Text == "")
            {
                soft_constraint_log.Text = "> None";
            }
            else
            {
                if (oldText != soft_constraint_log.Text)
                {
                    MessageBox.Show("Class group conflicts detected.\nPlease refer to the Conflicts tab for details.");
                }
            }
        }
        private void SubmitChangeTerm_Click(object sender, RoutedEventArgs e) // update term and year from user input
        {
            if (TermYearBox.Text.Length == 4)
            {
                termString = TermComboBox.Text;
                switch (termString)
                {
                    case "Spring":
                        term = "01";
                        break;
                    case "Summer":
                        term = "06";
                        break;
                    case "Fall":
                        term = "09";
                        break;
                    case "Winter":
                        term = "12";
                        break;
                    default:
                        MessageBox.Show("Unexpected term name!");
                        term = "00";
                        termString = "None";
                        break;
                }
                termYear = TermYearBox.Text;
                ProcessProfessorPreferences();
                MessageBox.Show("Changed Term to: " + termString + " " + termYear);
                RefreshGUI();
            }
            else
            {
                MessageBox.Show("Please enter a valid year (e.g. 2020)");
            }
        }

        // ADD / REMOVE / EDIT functionality (Professors, Classrooms, Classes)
        // Professors
        public void AddProfessor(Professors prof)
        {
            professors.Add(prof);
            colorPairs.ColorPairings.Add(new ProfColors { ProfName = prof.FullName, Color = prof.profRGB.colorString });
        }
        private void Btn_AddProfessor_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(Application.Current.Resources["setProf"].ToString());

            Unfocus_Overlay.Visibility = Visibility.Visible;
            AddProfessorDialog addProfDialog = new AddProfessorDialog();
            addProfDialog.Owner = this;
            addProfDialog.ShowDialog();
            Unfocus_Overlay.Visibility = Visibility.Hidden;

            if ((bool)Application.Current.Resources["Set_Prof_Success"])
            {
                string fName = (string)Application.Current.Resources["Set_Prof_FN"];
                string lName = (string)Application.Current.Resources["Set_Prof_LN"];
                string id = (string)Application.Current.Resources["Set_Prof_ID"];
                string colorString = (string)Application.Current.Resources["Set_Prof_Color"];
                Professors tmpProf = new Professors(fName, lName, id);
                tmpProf.profRGB = new RGB_Color(colorString);
                AddProfessor(tmpProf);
                Application.Current.Resources["Set_Prof_Success"] = false;
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
            editProfessorDialog.Owner = this;
            editProfessorDialog.ShowDialog();
            // Edit ColorPairs entry
            for (int i = 0; i < colorPairs.ColorPairings.Count; i++)
            {
                if (colorPairs.ColorPairings[i].ProfName == prof.FullName)
                {
                    colorPairs.ColorPairings[i].Color = prof.profRGB.colorString;
                }
            }
            Unfocus_Overlay.Visibility = Visibility.Hidden;
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
                    if (source != null) // Being called from a Professor Color Key item
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
            if (Application.Current.Resources["Set_ClassRoom_Success"] != null && (bool)Application.Current.Resources["Set_ClassRoom_Success"] == true)
            {
                string bldg = Application.Current.Resources["Set_ClassRoom_Bldg"].ToString();
                int roomNum = Int32.Parse(Application.Current.Resources["Set_ClassRoom_Num"].ToString());
                int capacity = Int32.Parse(Application.Current.Resources["Set_ClassRoom_Seats"].ToString());
                AddClassroom(new ClassRoom(bldg, roomNum, capacity));
                Application.Current.Resources["Set_ClassRoom_Success"] = false;
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
        }
        private void Btn_RemoveClassroom_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Yet to be implemented");
        }
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
                if (newClass.isAppointment)
                {
                    if (newClass.Classroom.Location == "APPT")
                    {
                        appointmentClasses.Add(newClass);
                    }
                    else if (newClass.Classroom.Location == "APPT2")
                    {
                        appointment2Classes.Add(newClass);
                    }
                }
                else
                {
                    unassignedClasses.Add(newClass);
                }
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

            if (Application.Current.Resources["Set_Class_Success"] != null && (bool)Application.Current.Resources["Set_Class_Success"] == true)
            {
                string crn = Application.Current.Resources["Set_Class_CRN"].ToString();
                string dpt = Application.Current.Resources["Set_Class_Dept"].ToString();
                int number = Int32.Parse(Application.Current.Resources["Set_Class_Number"].ToString());
                int sect = Int32.Parse(Application.Current.Resources["Set_Class_Section"].ToString());
                string name = Application.Current.Resources["Set_Class_Name"].ToString();
                int credits = Int32.Parse(Application.Current.Resources["Set_Class_Credits"].ToString());
                string prof = Application.Current.Resources["Set_Class_Professor"].ToString();
                bool online = Boolean.Parse(Application.Current.Resources["Set_Class_Online"].ToString());
                bool appt = Boolean.Parse(Application.Current.Resources["Set_Class_Appointment"].ToString());
                bool appt2 = Boolean.Parse(Application.Current.Resources["Set_Class_Appointment2"].ToString());
                bool appointment = false;
                ClassRoom CRoom = null;
                if (appt || appt2)
                {
                    appointment = true;
                    if (appt)
                    {
                        CRoom = new ClassRoom("APPT", 0);
                    }
                    else
                    {
                        CRoom = new ClassRoom("APPT2", 0);
                    }
                }
                else if (online)
                {
                    CRoom = new ClassRoom("WEB", 999);
                }
                else
                {
                    CRoom = new ClassRoom();
                }
                AddClass(new Classes(crn, dpt, number, sect, name, credits, "", new Timeslot(), 0, CRoom, DetermineProfessor(prof), online, appointment, false, "", "", new List<string>(), "0", "0"));
                Application.Current.Resources["Set_Class_Success"] = false;
                RefreshGUI();
            }
        }
        public void RemoveClass(string ID)
        {
            Classes removalTarget;
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].ClassID == ID)
                {
                    removalTarget = classList[i];
                    deletedClasses.Add(removalTarget);
                    classList.RemoveAt(i);
                    break;
                }
            }
        }
        private void Btn_RemoveClass_Click(object sender, RoutedEventArgs e)
        {
            // find the class
            string classID = "";
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    Label source = cm.PlacementTarget as Label;
                    if (source != null) // Being called from a Label
                    {
                        classID = source.Tag.ToString();
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock classCRN = parentGrid.Columns[0].GetCellContent(sourceRow) as TextBlock;
                        TextBlock className = parentGrid.Columns[4].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classSection = parentGrid.Columns[3].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classNumber = parentGrid.Columns[2].GetCellContent(sourceRow) as TextBlock;
                        classID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    }
                    RemoveClass(classID);
                    RefreshGUI();
                }
            }
        }
        public void EditClass(string ID)
        {
            Unfocus_Overlay.Visibility = Visibility.Visible;
            Classes toEdit = DetermineClass(ID);
            EditClassDialog editClassDialog = new EditClassDialog(toEdit);
            editClassDialog.Owner = this;
            editClassDialog.ShowDialog();

            if ((bool)Application.Current.Resources["Set_Class_Success"])
            {
                bool conflict = false;
                bool check_conflicts = (bool)Application.Current.Resources["Edit_Class_Check"];
                if (check_conflicts)
                {
                    Classes temp = toEdit.DeepCopy();
                    Professors temp_Prof = DetermineProfessor((string)Application.Current.Resources["Set_Class_Professor"]);
                    temp.Prof = temp_Prof;
                    conflict = DetermineTimeConflict(temp, temp.ClassDay, temp.StartTime.TimeID);
                    // flag down
                    Application.Current.Resources["Edit_Class_Check"] = false;
                }
                if (!conflict)
                {
                    bool originalOnline = toEdit.Online;
                    bool originalAssigned = toEdit.isAssigned;
                    string originalCRN = toEdit.CRN;
                    string originalBldg = toEdit.Classroom.Location;

                    if ((string)Application.Current.Resources["Set_Class_CRN"] != toEdit.CRN)
                    {
                        toEdit.CRN = (string)Application.Current.Resources["Set_Class_CRN"];
                    }
                    if ((string)Application.Current.Resources["Set_Class_Dept"] != toEdit.DeptName)
                    {
                        toEdit.DeptName = (string)Application.Current.Resources["Set_Class_Dept"];
                    }
                    if ((int)Application.Current.Resources["Set_Class_Number"] != toEdit.ClassNumber)
                    {
                        toEdit.ClassNumber = (int)Application.Current.Resources["Set_Class_Number"];
                    }
                    if ((int)Application.Current.Resources["Set_Class_Section"] != toEdit.SectionNumber)
                    {
                        toEdit.SectionNumber = (int)Application.Current.Resources["Set_Class_Section"];
                    }
                    if ((string)Application.Current.Resources["Set_Class_Name"] != toEdit.ClassName)
                    {
                        toEdit.ClassName = (string)Application.Current.Resources["Set_Class_Name"];
                    }
                    if ((int)Application.Current.Resources["Set_Class_Credits"] != toEdit.Credits)
                    {
                        toEdit.Credits = (int)Application.Current.Resources["Set_Class_Credits"];
                    }
                    if ((string)Application.Current.Resources["Set_Class_Professor"] != toEdit.Prof.SRUID)
                    {
                        toEdit.Prof = DetermineProfessor((string)Application.Current.Resources["Set_Class_Professor"]);
                    }
                    if ((bool)Application.Current.Resources["Set_Class_Online"] != toEdit.Online)
                    {
                        toEdit.Online = (bool)Application.Current.Resources["Set_Class_Online"];
                    }
                    bool appointment = false;
                    bool appt = (bool)Application.Current.Resources["Set_Class_Appointment"];
                    bool appt2 = (bool)Application.Current.Resources["Set_Class_Appointment2"];
                    if (appt || appt2)
                    {
                        appointment = true;
                    }
                    toEdit.isAppointment = appointment;

                    if (toEdit.Online)
                    {
                        toEdit.StartTime = new Timeslot();
                        toEdit.Classroom = new ClassRoom("WEB", 999);
                        toEdit.ClassDay = "";
                        toEdit.isAssigned = false;
                        toEdit.isAppointment = false;
                    }
                    else if (toEdit.isAppointment)
                    {
                        toEdit.StartTime = new Timeslot();
                        toEdit.ClassDay = "";
                        toEdit.isAssigned = false;
                        toEdit.Online = false;
                        if (appt)
                        {
                            toEdit.Classroom = new ClassRoom("APPT", 0);
                        }
                        else
                        {
                            toEdit.Classroom = new ClassRoom("APPT2", 0);
                        }
                    }
                    Application.Current.Resources["Set_Class_Success"] = false;
                }
                else
                {
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Exclamation;
                    MessageBox.Show("Professor is already teaching at that time!\n\nReverting Changes...", "Invalid Edit", button, icon);
                }
            }
            Unfocus_Overlay.Visibility = Visibility.Hidden;
        }
        private void Btn_EditClass_Click(object sender, RoutedEventArgs e)
        {
            // find the class
            string ID = "";
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    Label source = cm.PlacementTarget as Label;
                    if (source != null) // Being called from a Label
                    {
                        ID = source.Tag.ToString();
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock classCRN = parentGrid.Columns[0].GetCellContent(sourceRow) as TextBlock;
                        TextBlock className = parentGrid.Columns[4].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classSection = parentGrid.Columns[3].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classNumber = parentGrid.Columns[2].GetCellContent(sourceRow) as TextBlock;
                        ID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    }
                    EditClass(ID);
                    RefreshGUI();
                }
            }
        }
        public void EditClassTime(string ID)
        {
            Unfocus_Overlay.Visibility = Visibility.Visible;
            Classes toEdit = DetermineClass(ID);
            EditClassTimeDialog editClassDialog = new EditClassTimeDialog(toEdit);
            editClassDialog.Owner = this;
            editClassDialog.ShowDialog();
            Unfocus_Overlay.Visibility = Visibility.Hidden;
        }
        private void Btn_EditClassTime_Click(object sender, RoutedEventArgs e)
        {
            // find the class
            string ID = "";
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    Label source = cm.PlacementTarget as Label;
                    if (source != null) // Being called from a Label
                    {
                        ID = source.Tag.ToString();
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock classCRN = parentGrid.Columns[0].GetCellContent(sourceRow) as TextBlock;
                        TextBlock className = parentGrid.Columns[4].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classSection = parentGrid.Columns[3].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classNumber = parentGrid.Columns[2].GetCellContent(sourceRow) as TextBlock;
                        ID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    }
                    EditClassTime(ID);
                    RefreshGUI();
                }
            }
        }
        public void CopyClass(string ID)
        {
            Classes copy;
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].ClassID == ID)
                {
                    copy = classList[i].DeepCopy();
                    // Change copy properties
                    copy.CRN = "NEW";
                    copy.isAssigned = false;
                    copy.ClassDay = "";
                    copy.StartTime = new Timeslot();
                    copy.Classroom = new ClassRoom();
                    // Add to classlist
                    classList.Add(copy);
                    break;
                }
            }
        }
        private void Btn_CopyClass_Click(object sender, RoutedEventArgs e)
        {
            // find the class
            string classID = "";
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    Label source = cm.PlacementTarget as Label;
                    if (source != null) // Being called from a Label
                    {
                        classID = source.Tag.ToString();
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock classCRN = parentGrid.Columns[0].GetCellContent(sourceRow) as TextBlock;
                        TextBlock className = parentGrid.Columns[4].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classSection = parentGrid.Columns[3].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classNumber = parentGrid.Columns[2].GetCellContent(sourceRow) as TextBlock;
                        classID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    }
                    CopyClass(classID);
                    RefreshGUI();
                }
            }
        }
        public void EditNotes(string ID)
        {
            Classes c1 = DetermineClass(ID);
            Unfocus_Overlay.Visibility = Visibility.Visible;
            EditNotesDialog editNotesDialog = new EditNotesDialog(c1);
            editNotesDialog.Owner = this;
            editNotesDialog.ShowDialog();
            Unfocus_Overlay.Visibility = Visibility.Hidden;
        }
        private void Btn_EditNotes_Click(object sender, RoutedEventArgs e)
        {
            // find the class
            string classID = "";
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    Label source = cm.PlacementTarget as Label;
                    if (source != null) // Being called from a Label
                    {
                        classID = source.Tag.ToString();
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock classCRN = parentGrid.Columns[0].GetCellContent(sourceRow) as TextBlock;
                        TextBlock className = parentGrid.Columns[4].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classSection = parentGrid.Columns[3].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classNumber = parentGrid.Columns[2].GetCellContent(sourceRow) as TextBlock;
                        classID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    }
                    EditNotes(classID);
                    RefreshGUI();
                }
            }
        }
        public void HideClass(string ID)
        {
            Classes c1 = DetermineClass(ID);
            // maxseats = ExtraData[2]
            // if hidden -> unhide / else hide
            if (c1.isHidden)
            {
                c1.MaxSeats = "1";
            }
            else
            {
                c1.MaxSeats = "0";
            }
        }
        private void Btn_HideClass_Click(object sender, RoutedEventArgs e)
        {
            // find the class
            string classID = "";
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    Label source = cm.PlacementTarget as Label;
                    if (source != null) // Being called from a Label
                    {
                        classID = source.Tag.ToString();
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock classCRN = parentGrid.Columns[0].GetCellContent(sourceRow) as TextBlock;
                        TextBlock className = parentGrid.Columns[4].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classSection = parentGrid.Columns[3].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classNumber = parentGrid.Columns[2].GetCellContent(sourceRow) as TextBlock;
                        classID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    }
                    HideClass(classID);
                    RefreshGUI();
                }
            }
        }
        private void EditSeats(string ID)
        {
            Unfocus_Overlay.Visibility = Visibility.Visible;
            Classes toEdit = DetermineClass(ID);
            EditClassSeating editClassSeating = new EditClassSeating(toEdit);
            editClassSeating.Owner = this;
            editClassSeating.ShowDialog();
            Unfocus_Overlay.Visibility = Visibility.Hidden;
        }
        private void Btn_EditSeats_Click(object sender, RoutedEventArgs e)
        {
            // find the class
            string classID = "";
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    Label source = cm.PlacementTarget as Label;
                    if (source != null) // Being called from a Label
                    {
                        classID = source.Tag.ToString();
                    }
                    else // Being called from a GridRow
                    {
                        DataGridRow sourceRow = cm.PlacementTarget as DataGridRow;
                        DataGrid parentGrid = GetParent<DataGrid>(sourceRow as DependencyObject);
                        TextBlock classCRN = parentGrid.Columns[0].GetCellContent(sourceRow) as TextBlock;
                        TextBlock className = parentGrid.Columns[4].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classSection = parentGrid.Columns[3].GetCellContent(sourceRow) as TextBlock;
                        TextBlock classNumber = parentGrid.Columns[2].GetCellContent(sourceRow) as TextBlock;
                        classID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    }
                    EditSeats(classID);
                    RefreshGUI();
                }
            }
        }

        // DRAG/DROP functionality
        private void MouseMoveOnGridRow(object sender, MouseEventArgs e) // Handles DRAG operation on class list items 
        {
            TextBlock cellUnderMouse = sender as TextBlock;
            if (cellUnderMouse != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DataGridRow row = DataGridRow.GetRowContainingElement(cellUnderMouse);
                DragDrop.DoDragDrop(Unassigned_Classes_Grid, row, DragDropEffects.Copy);
            }
        }
        private void MouseMoveOnAssignedClass(object sender, MouseEventArgs e) // Handles DRAG operation on GUI classes box 
        {
            Label labelUnderMouse = sender as Label;
            int classIndex = -1;
            if ((labelUnderMouse != null) && (e.LeftButton == MouseButtonState.Pressed) && (labelUnderMouse.Tag != null) && (labelUnderMouse.Tag.ToString() != ""))
            {
                // find index of class being represented by the label
                for (int i = 0; i < classList.Count; i++)
                {
                    if (classList[i].ClassID == labelUnderMouse.Tag.ToString())
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
            }
            else
            {
                string ID = "";
                DataGridRow droppedRow = (DataGridRow)e.Data.GetData(typeof(DataGridRow));
                if (droppedRow != null)
                {
                    TextBlock classCRN = Unassigned_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                    TextBlock className = Unassigned_Classes_Grid.Columns[4].GetCellContent(droppedRow) as TextBlock;
                    TextBlock classSection = Unassigned_Classes_Grid.Columns[3].GetCellContent(droppedRow) as TextBlock;
                    TextBlock classNumber = Unassigned_Classes_Grid.Columns[2].GetCellContent(droppedRow) as TextBlock;
                    ID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    Classes theClass = DetermineClass(ID);
                    string classType = "";
                    if (theClass.Online || theClass.isAppointment)
                    {
                        if (theClass.Online)
                        {
                            classType = "Online";
                        }
                        else if (theClass.isAppointment)
                        {
                            classType = "Appointment";
                        }
                        string messageBoxText = "Are you sure you want to change this class\nfrom " + classType + " to In-Class?";
                        string caption = classType + " class alteration";
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
                                    if (classList[i].ClassID == ID)
                                    {
                                        if (classType == "Online")
                                        {
                                            classList[i].Online = false;
                                        }
                                        else if (classType == "Appointment")
                                        {
                                            classList[i].isAppointment = false;
                                        }
                                        classList[i].Classroom = new ClassRoom();
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
            RefreshGUI();
        }
        private void HandleDropToCell(Object sender, DragEventArgs e) // Handles DROP operation to GUI classes box 
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
                    string ID = "";
                    DataGridRow droppedRow = (DataGridRow)e.Data.GetData(typeof(DataGridRow));
                    if (droppedRow == null)
                    {
                        MessageBox.Show("dropped row was null");
                    }
                    else
                    {
                        TextBlock classCRN = Unassigned_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                        TextBlock className = Unassigned_Classes_Grid.Columns[4].GetCellContent(droppedRow) as TextBlock;
                        TextBlock classSection = Unassigned_Classes_Grid.Columns[3].GetCellContent(droppedRow) as TextBlock;
                        TextBlock classNumber = Unassigned_Classes_Grid.Columns[2].GetCellContent(droppedRow) as TextBlock;
                        ID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    }

                    /// VALIDATION CHECKS GO HERE ///
                    // check if its online class
                    bool validOperation = true;
                    int classIndex = -1;
                    for (int i = 0; i < classList.Count; i++)
                    {
                        if (classList[i].ClassID == ID)
                        {
                            if (classList[i].Online || classList[i].isAppointment)
                            {
                                string classType = "";
                                if (classList[i].Online)
                                {
                                    classType = "Online";
                                }
                                else if (classList[i].isAppointment)
                                {
                                    classType = "Appointment";
                                }
                                string messageBoxText = "Are you sure you want to change this class from " + classType + " to In-Class?";
                                string caption = classType + " class warning";
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
                                if (classList[classIndex].Classroom.Location.Contains("APPT")) // its by appointment
                                {
                                    if (classList[classIndex].Classroom.Location == "APPT")
                                    {
                                        classList[classIndex].isAssigned = true;
                                        classList[classIndex].isAppointment = false;
                                    }
                                    else if (classList[classIndex].Classroom.Location == "APPT2")
                                    {
                                        classList[classIndex].isAssigned = true;
                                        classList[classIndex].isAppointment = false;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Couldnt remove appointed class from respective list");
                                    }
                                }
                                else // its unassigned
                                {
                                    classList[classIndex].isAssigned = true;
                                }
                            }
                            else // its online
                            {
                                classList[classIndex].Online = false;
                            }
                            // Update class in masterlist = give it a start time + classroom
                            classList[classIndex].ClassDay = days;
                            classList[classIndex].StartTime = DetermineTime(start, days);
                            classList[classIndex].Classroom = DetermineClassroom(bldg, room);
                            // Give the Label the class information
                            receiver.Content = classList[classIndex].TextBoxName;
                            if (classList[classIndex].isHidden)
                            {
                                receiver.Background = stripedBackground(classList[classIndex].Prof.profRGB.colorBrush);
                            }
                            else
                            {
                                receiver.Background = classList[classIndex].Prof.Prof_Color;
                            }
                            receiver.Tag = classList[classIndex].ClassID;
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
                RefreshGUI();
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
        private void HandleDropToOnlineList(Object sender, DragEventArgs e) // Handles DROP operation to online classes list 
        {
            Label sourceLabel = (Label)e.Data.GetData(typeof(object));
            if (sourceLabel != null)
            {
                string messageBoxText = "Are you sure you want to change this\nIn-Class course to Online format?";
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
                        classList[classIndex].Classroom = new ClassRoom("WEB", 999);
                        classList[classIndex].ClassDay = "";
                        classList[classIndex].StartTime = new Timeslot();
                        classList[classIndex].isAssigned = false;
                        classList[classIndex].Online = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else // Comes from unassigned list
            {
                DataGridRow droppedRow = (DataGridRow)e.Data.GetData(typeof(DataGridRow));
                if (droppedRow != null)
                {
                    TextBlock classCRN = Online_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                    TextBlock className = Online_Classes_Grid.Columns[4].GetCellContent(droppedRow) as TextBlock;
                    TextBlock classSection = Online_Classes_Grid.Columns[3].GetCellContent(droppedRow) as TextBlock;
                    TextBlock classNumber = Online_Classes_Grid.Columns[2].GetCellContent(droppedRow) as TextBlock;
                    string classID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    Classes theClass = DetermineClass(classID);
                    if (!theClass.Online)
                    {
                        string messageBoxText = "Are you sure you want to change this\nCourse to Online format?";
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
                                    if (classList[i].ClassID == classID)
                                    {
                                        classList[i].Online = true;
                                        classList[i].Classroom = new ClassRoom("WEB", 999);
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
            RefreshGUI();
        }
        private void HandleDropToAppointmentList(Object sender, DragEventArgs e) // Handles DROP operation to appointment classes list 
        {
            Label sourceLabel = (Label)e.Data.GetData(typeof(object));
            if (sourceLabel != null)
            {
                string messageBoxText = "Are you sure you want to change this\nIn-Class course to 'By Appointment' format?";
                string caption = "By Appointment class alteration";
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
                        classList[classIndex].Classroom = new ClassRoom("APPT", 0);
                        classList[classIndex].ClassDay = "";
                        classList[classIndex].StartTime = new Timeslot();
                        classList[classIndex].isAssigned = false;
                        classList[classIndex].Online = false;
                        classList[classIndex].isAppointment = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else // Its from unassigned list
            {
                DataGridRow droppedRow = (DataGridRow)e.Data.GetData(typeof(DataGridRow));
                if (droppedRow != null)
                {
                    TextBlock classCRN = Online_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                    TextBlock className = Online_Classes_Grid.Columns[4].GetCellContent(droppedRow) as TextBlock;
                    TextBlock classSection = Online_Classes_Grid.Columns[3].GetCellContent(droppedRow) as TextBlock;
                    TextBlock classNumber = Online_Classes_Grid.Columns[2].GetCellContent(droppedRow) as TextBlock;
                    string classID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    Classes theClass = DetermineClass(classID);
                    if (!theClass.isAppointment)
                    {
                        string messageBoxText = "Are you sure you want to change this\nCourse to 'By Appointment' format?";
                        string caption = "By Appointment class alteration";
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
                                    if (classList[i].ClassID == classID)
                                    {
                                        classList[i].isAppointment = true;
                                        classList[i].Classroom = new ClassRoom("APPT", 0);
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
            RefreshGUI();
        }
        private void HandleDropToAppointment2List(Object sender, DragEventArgs e) // Handles DROP operation to appointment2 classes list 
        {
            Label sourceLabel = (Label)e.Data.GetData(typeof(object));
            if (sourceLabel != null)
            {
                string messageBoxText = "Are you sure you want to change this\nIn-Class course to 'By Appointment' format?";
                string caption = "Appointment class alteration";
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
                        classList[classIndex].Classroom = new ClassRoom("APPT2", 0);
                        classList[classIndex].ClassDay = "";
                        classList[classIndex].StartTime = new Timeslot();
                        classList[classIndex].isAssigned = false;
                        classList[classIndex].Online = false;
                        classList[classIndex].isAppointment = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else // Its from unassigned list
            {
                DataGridRow droppedRow = (DataGridRow)e.Data.GetData(typeof(DataGridRow));
                if (droppedRow != null)
                {
                    TextBlock classCRN = Online_Classes_Grid.Columns[0].GetCellContent(droppedRow) as TextBlock;
                    TextBlock className = Online_Classes_Grid.Columns[4].GetCellContent(droppedRow) as TextBlock;
                    TextBlock classSection = Online_Classes_Grid.Columns[3].GetCellContent(droppedRow) as TextBlock;
                    TextBlock classNumber = Online_Classes_Grid.Columns[2].GetCellContent(droppedRow) as TextBlock;
                    string classID = classCRN.Text + className.Text + classSection.Text + classNumber.Text;
                    Classes theClass = DetermineClass(classID);
                    if (!theClass.isAppointment)
                    {
                        string messageBoxText = "Are you sure you want to change this\nCourse to 'By Appointment' format?";
                        string caption = "Appointment class alteration";
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
                                    if (classList[i].ClassID == classID)
                                    {
                                        classList[i].isAppointment = true;
                                        classList[i].Classroom = new ClassRoom("APPT2", 0);
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
            RefreshGUI();
        }

        // Utility functions
        public RGB_Color StringToRGB(string s) // Converts rgb string to a RGB_Color object 
        {
            RGB_Color color;
            String[] parts = s.Split('.');
            color = new RGB_Color(Byte.Parse(parts[0]), Byte.Parse(parts[1]), Byte.Parse(parts[2]));
            return color;
        }
        public bool isColorTaken(RGB_Color color)
        {
            for (int i = 0; i < professors.Count; i++)
            {
                if (withinColorRange(color, professors[i].profRGB))
                {
                    return true;
                }
            }
            return false;
        }
        public bool withinColorRange(RGB_Color c1, RGB_Color c2)
        {
            int threshold = 65;
            if (Math.Abs(c1.R - c2.R) <= threshold && Math.Abs(c1.G - c2.G) <= threshold && Math.Abs(c1.B - c2.B) <= threshold)
            {
                return true;
            }
            return false;
        }
        public Timeslot DetermineTime(string startTime, string classDay) // Finds corresponding Timeslot object based on start time and class day 
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
        public ClassRoom DetermineClassroom(string building, int roomNum) // Finds corresponding ClassRoom object based on building name and room number 
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
        public Professors DetermineProfessor(string sruID) // Finds corresponding Professor object based on SRUID 
        {
            for (int i = 0; i < professors.Count; i++)
            {
                if (professors[i].SRUID == sruID)
                {
                    return professors[i];
                }
            }
            //MessageBox.Show("DEBUG: Couldnt find the referenced professor!");
            return new Professors();
        }
        public Classes DetermineClass(string classID) // Finds corresponding Class object based on CRN 
        {
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].ClassID == classID)
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
        } // Finds the closest <T> type parent of the passed XAML element
        public bool DetermineTimeConflict(Classes _class, string days, string timeID) // Determines if professor is already teaching at that time before he/she is asssigned to a timeslot 
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
                string classID = "";
                for (int i = 0; i < classrooms.Count; i++)
                {
                    labelID = rowID + "_" + classrooms[i].ClassID;
                    lbl = (Label)FindName(labelID);
                    if (lbl != null)
                    {
                        if (lbl.Tag != null)
                        {
                            classID = lbl.Tag.ToString();
                            for (int n = 0; n < classList.Count; n++)
                            {
                                if (classList[n].ClassID == classID)
                                {
                                    if (classList[n].Prof.FullName == profName && _class.ClassID != classID)
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
                // go through classlist and see if the professor is assigned at the same time
                Timeslot targetTime = DetermineTime(timeID, days);
                for (int i = 0; i < classList.Count; i++)
                {
                    if (_class.ClassID != classList[i].ClassID)
                    {
                        if (classList[i].Prof.FullName == _class.Prof.FullName)
                        {
                            //MessageBox.Show("Prof Hit: " + _class.Prof.LastName);
                            if (classList[i].StartTime.FullTime == targetTime.FullTime)
                            {
                                isConflict = true;
                                if (classList[i].Online)
                                {
                                    MessageBox.Show("Professor is teaching an ONLINE class at that time...");
                                }
                                break;
                            }
                        }
                    }
                }
                return isConflict;
            }
        }
        public string formatTime(string time) // Standardizes time format being read from excel file to prevent errors when creating the classes 
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
        public string getFileDirectory(string filePath) // Extracts directory string from full filepath string 
        {
            string directory = "";
            for (int i = (filePath.Length - 1); i >= 0; i--)
            {
                if (filePath[i] == '\\')
                {
                    directory = filePath.Substring(0, (i + 1));
                    break;
                }
            }
            //MessageBox.Show("Directory: " + directory);
            return directory;
        }
        public DataTable getDataTableFromClasses() // Creates a datatable based on classList 
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            //Setiing Table Name  
            dt.TableName = "Sheet 1";
            // Determine Types
            for (int i = 0; i < excelHeaders.Count; i++)
            {
                Type colType = typeof(string);
                if (i == 3 || i == 8 || i == 12 || i == 18 || i == 20)
                {
                    colType = typeof(int);
                }
                excelTypes.Add(colType);
            }
            //Add Columns
            for (int i = 0; i < excelHeaders.Count; i++)
            {
                dt.Columns.Add(excelHeaders[i], excelTypes[i]);
            }
            //Add Rows in DataTable
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].ExtraData.Count == 0)
                {
                    int extraFields = 15; // number of extra fields in classes
                    for (int n = 0; n < extraFields; n++)
                    {
                        classList[i].ExtraData.Add("");
                    }
                }
                string start = classList[i].StartTime.Start;
                string end = classList[i].StartTime.End;
                if (start == "-- ")
                {
                    start = "";
                    end = "";
                }
                dt.Rows.Add(termYear + term, classList[i].ExtraData[0], classList[i].DeptName, classList[i].ClassNumber,
                    classList[i].getSectionString(), classList[i].CRN, classList[i].ClassName, classList[i].ExtraData[1], classList[i].Credits,
                    classList[i].MaxSeats, classList[i].ExtraData[3], classList[i].ProjSeats, classList[i].SeatsTaken,
                    classList[i].ExtraData[5], classList[i].ExtraData[6], classList[i].ClassDay, start, end, classList[i].Classroom.AvailableSeats,
                    classList[i].Classroom.Location, classList[i].Classroom.RoomNum, classList[i].Prof.FullName, classList[i].Prof.SRUID,
                    classList[i].ExtraData[7], classList[i].ExtraData[8], classList[i].ExtraData[9], classList[i].ExtraData[10],
                    classList[i].ExtraData[11], classList[i].ExtraData[12], classList[i].ExtraData[13], classList[i].ExtraData[14],
                    classList[i].SectionNotes, classList[i].Notes);
            }
            //Add Deleted classes to DataTable
            for (int i = 0; i < deletedClasses.Count; i++)
            {
                if (deletedClasses[i].ExtraData.Count == 0)
                {
                    int extraFields = 15; // number of extra fields in classes
                    for (int n = 0; n < extraFields; n++)
                    {
                        deletedClasses[i].ExtraData.Add("");
                    }
                }
                string start = deletedClasses[i].StartTime.Start;
                string end = deletedClasses[i].StartTime.End;
                if (start == "-- ")
                {
                    start = "";
                    end = "";
                }
                dt.Rows.Add(termYear + term, deletedClasses[i].ExtraData[0], deletedClasses[i].DeptName, deletedClasses[i].ClassNumber,
                    deletedClasses[i].getSectionString(), deletedClasses[i].CRN, deletedClasses[i].ClassName, deletedClasses[i].ExtraData[1], deletedClasses[i].Credits,
                    deletedClasses[i].MaxSeats, deletedClasses[i].ExtraData[3], deletedClasses[i].ProjSeats, deletedClasses[i].SeatsTaken,
                    deletedClasses[i].ExtraData[5], deletedClasses[i].ExtraData[6], deletedClasses[i].ClassDay, start, end, deletedClasses[i].Classroom.AvailableSeats,
                    deletedClasses[i].Classroom.Location, deletedClasses[i].Classroom.RoomNum, deletedClasses[i].Prof.FullName, deletedClasses[i].Prof.SRUID,
                    deletedClasses[i].ExtraData[7], deletedClasses[i].ExtraData[8], deletedClasses[i].ExtraData[9], deletedClasses[i].ExtraData[10],
                    deletedClasses[i].ExtraData[11], deletedClasses[i].ExtraData[12], deletedClasses[i].ExtraData[13], deletedClasses[i].ExtraData[14],
                    deletedClasses[i].SectionNotes, deletedClasses[i].Notes);
            }
            dt.AcceptChanges();
            return dt;
        }
        public string ComputeSha256Hash(byte[] rawData) // Compute the SHA256 hash digest of the passed byte buffer. Then convert it to string format. 
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(rawData);

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public void GenerateClassListHashes() // Generate initial hashes for class list read from excel file (for comparison when writing to new file)
        {
            string hash;
            for (int i = 0; i < classList.Count; i++)
            {
                hash = ComputeSha256Hash(classList[i].Serialize());
                hashedClasses.Add(new ClassesHash(classList[i].ClassID, hash));
            }
        }
        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e) // Set the scrolling speed for the lists using mousewheel 
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 10);
            e.Handled = true;
        }
        public bool isGroupMember(Classes _class, int groupIndex)
        {
            for (int i = 0; i < classGroupings[groupIndex].ClassDept.Count; i++)
            {
                if (_class.DeptName == classGroupings[groupIndex].ClassDept[i] && _class.ClassNumber == Int32.Parse(classGroupings[groupIndex].ClassNum[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public void checkGroupConflict(Classes _class, int groupIndex)
        {
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].DeptName != _class.DeptName || classList[i].ClassNumber != _class.ClassNumber)
                {
                    if (isGroupMember(classList[i], groupIndex))
                    {
                        if (classList[i].StartTime.FullTime == _class.StartTime.FullTime && classList[i].StartTime.Time != "--")
                        {
                            // check if there is another section at another time
                            if (!hasAlternativeSection(_class) && !hasAlternativeSection(classList[i]))
                            {
                                // Log conflict
                                soft_constraint_log.Text = soft_constraint_log.Text + "\n> " + "[Group Conflict] " + classList[i].DeptName + " " + classList[i].ClassNumber + " section " +
                                    classList[i].SectionNumber + "  <---> " + _class.DeptName + " " + _class.ClassNumber + " section " + _class.SectionNumber;
                                break;
                            }    
                        }
                    }
                }
            }
        }
        public bool hasAlternativeSection(Classes _class)
        {
            for (int i = 0; i < classList.Count; i++)
            {
                if (classList[i].DeptName == _class.DeptName && classList[i].ClassNumber == _class.ClassNumber)
                {
                    if (classList[i].StartTime != null && _class.StartTime != null)
                    {
                        if (classList[i].StartTime.FullTime != _class.StartTime.FullTime)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // Custom Brushes
        LinearGradientBrush stripedBackground(Color baseBackground)
        {
            LinearGradientBrush output = new LinearGradientBrush();
            
            output.MappingMode = BrushMappingMode.Absolute;
            output.SpreadMethod = GradientSpreadMethod.Repeat;
            output.StartPoint = new Point(0, 0);
            output.EndPoint = new Point(8, 8);
            
            output.GradientStops.Add(new GradientStop(baseBackground, 0.0));
            output.GradientStops.Add(new GradientStop(baseBackground, 0.5));
            output.GradientStops.Add(new GradientStop(Colors.LightGray, 0.5));
            output.GradientStops.Add(new GradientStop(Colors.LightGray, 1.0));
            return output;
        }
    }

    // XAML converters
    public class ColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            string input = value.ToString();

            // split string into numbers
            string[] nums = input.Split(new string[] { " / " }, StringSplitOptions.None);

            //custom condition is checked based on data.
            int current = Int32.Parse(nums[0].ToString());
            int max = Int32.Parse(nums[1].ToString());

            if (current > max)
            {
                //return "SeaGreen";
                return new SolidColorBrush(Colors.LightPink);
            }
            else if (current <= max)
                //return "LightGreen";
                return new SolidColorBrush(Colors.LightGreen);
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PreferenceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int input = Int32.Parse(value.ToString());

            if (input == -1)
            {
                return new SolidColorBrush(Colors.PaleGoldenrod);
            }
            else if (input == -2)
                return new SolidColorBrush(Colors.Pink);
            else
                return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PreferenceMessageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string input = value.ToString();

            if (input != "")
            {
                return input;
            }
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
