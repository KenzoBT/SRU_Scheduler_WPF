   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  H DSRU Scheduler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:Schedule_WPFSchedule_WPF +clr-namespace:Schedule_WPFSchedule_WPF    Schedule_WPF.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6sys&clr-namespace:System;assembly=mscorlib     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:Schedule_WPF     (cclr-namespace:Schedule_WPF     -     � Title$  SRU Term Scheduler��$	��900��$
��1400�� � Icon$ images/sru.png�� �� BitmapScalingMode$ HighQuality=� � WindowStartupLocation$ CenterScreen=� � 	Resources !  Schedule_WPF.ColorConverter   	converter �� 
TargetType  	LeftAlign �� Property �� Value  CenterAlign  DragRow  	DragLabel  TruncateOverflow  LargeTab �� 
TargetType	 ��Name
 �� Triggers P� Property P� Value �� 
TargetName  SmallTab  ClassContextMenu m� CommandParameter �� RelativeSource �� Path m� Icon 	 ClassContextMenuGUI 
 ProfessorContextMenu��%�  &	        &	      &	 S     &	 �     &	 �     &	 �     &	 �     &	 <    &	 �    &		 �    &	
 t      �� " ��.���  �@��TextAlignment$
 Left=���  � ��$ 5,0,0,5q��� " ��.���  �@��TextAlignment$ Center=��� " ��.�-   �� " ��.�-   �� " ��.���  � ��$ CharacterEllipsis=�-   �� " ��.���  � ��
 �������  � ��$ 32����  � ��$ 75����  � �� �� " ��.R�� $��Panel��.+��� $	 ContentSite��$��Center=�$��Center=�$��Header��
 P�  �@��
IsSelected . . ��� $ Panel�� � �� �#016648P�  �@��
IsSelected .  . ��� $ Panel�� � �� �#505050�� " ��.���  � ��
 �������  � ��$ 22����  � ��$ 50����  � �� �� " ��.R�� $��Panel��.+��� $	 ContentSite��$��Center=�$��Center=�$��Header��
 P�  �@��
IsSelected . . ��� $ Panel�� � �� �#016648P�  �@��
IsSelected .  . ��� $ Panel�� � �� �#505050�� .@�m� -   $��
Edit Class�� ��  �� *Self+$ Parent� �� $��images/edit.png��m� -   $��Remove Class�� ��  �� *Self+$ Parent� �� $��images/delete.png��m� -   $��View/Edit Notes�� ��  �� *Self+$ Parent� �� $��images/notes.png���� .@�m� -   $��
Edit Class�� ��  �� *Self+$ Parent� �� $��images/edit.png��m� -	   $��Remove Class�� ��  �� *Self+$ Parent� �� $��images/delete.png��m� -
   $��View/Edit Notes�� ��  �� *Self+$ Parent� �� $��images/notes.png��m� -   $��Copy to Unassigned List�� ��  �� *Self+$ Parent� �� $��images/copy.png���� .@�m� -   $��Edit Professor�� ��  �� *Self+$ Parent� �� $��images/edit.png��m� -   $��Remove Professor�� ��  �� *Self+$ Parent� �� $��images/delete.png��.��� ���#272727 � RowDefinitions �� $��1* �.+�� -   $��Unfocus_Overlay��$��0��
���   � K� ZIndex$ 1�� J� Opacity$	 0.4Q�$y�Hidden=��� $��0�� �� TabStripPlacement$	 Top=�$ 0��.@��� $��	Scheduler��$��-3q�#��[ .��� -   $��ScheduleTabGrid��$��-3q� �� $��3* ��� $
��1.2* � � ColumnDefinitions �� $��1* ��� $��1* �.+�� -   $	��MWF��$��0�����#CACACA �� $��1* ��� $	��10* ��� $��1* � �� $��1* ��� $	��10* ��� $��1* �.+�(� $��Center=�$��1��$��Center=�.���� $��0 7 0 7q�
���   �MWF Timetable� -   $��TR��$��1�����#DDDDDD �� $��1* ��� $	��10* ��� $��1* � �� $��1* ��� $	��10* ��� $��1* �.+�(� $��Center=�$��1��$��Center=�$��40��$��80��.���� $��0 7 0 7q�
���   �TR Timetable� -   $��Unassigned_Classes��$��1��$��2�����#AAAAAA �� $��32 ��� $��1* � �� $��3* ��� $
��2.8* ��� $��1* ��� $
��0.4* �.+��� $��0��$��0��$��Center=�$��Center=�.��Unassigned Classes�� -   $
��Auto=�$��1��$��0��$��
15,0,15,15q�.��&   System.Windows.Controls.DataGrid  -     Name$ Unassigned_Classes_Grid����.    AutoGenerateColumns .   J� 	AllowDrop .    CanUserAddRows .     CanUserDeleteRows .     RowStyle �� )  #System.Windows.Controls.DataGridRow"  .���  �@�ContextMenu# [    Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$	 CRN��1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [     
IsReadOnly  . !   Binding! �� *CRN+  $ Dept.��# [   . ! �� *
DeptName+  $ Class #��# [   . ! �� *ClassNumber+  $ Sect.��# [   . ! �� *SectionNumber+  $ 
Class Name��# [   . ! �� *	ClassName+  $ 	Professor��# [   . ! �� *Prof.FullName+  $ Credits��# [   . ! �� *	Credits+�� $��0��$��1��$��2��$	 Top=�$��0,8,0,15q�.@��� $	��WEB��$��-3q�#��[ .���� -   $
��Auto=�$��-3q�.��  -   $ Online_Classes_Grid����.  .   .  .   .   �� "  .���  �@�ContextMenu# [    $	 CRN��# [   . ! �� *CRN+  $ Dept.��# [   . ! �� *
DeptName+  $ Class #��# [   . ! �� *ClassNumber+  $ Sect.��# [   . ! �� *SectionNumber+  $ 
Class Name��# [   . ! �� *	ClassName+  $ 	Professor��# [   . ! �� *Prof.FullName+  $ Credits��# [   . ! �� *	Credits+�� $
��APPT��$��-3q�#��[ .���� -   $
��Auto=�$��-3q�.��  -   $ Appointment_Classes_Grid����.  .   .  .   .   �� "  .���  �@�ContextMenu# [    $	 CRN��# [   . ! �� *CRN+  $ Dept.��# [   . ! �� *
DeptName+  $ Class #��# [   . ! �� *ClassNumber+  $ Sect.��# [   . ! �� *SectionNumber+  $ 
Class Name��# [   . ! �� *	ClassName+  $ 	Professor��# [   . ! �� *Prof.FullName+  $ Credits��# [   . ! �� *	Credits+�� $��APPT2��$��-3q�#��[ .���� -   $
��Auto=�$��-3q�.��  -   $ Appointment2_Classes_Grid����.  .   .  .   .   �� "  .���  �@�ContextMenu# [    $	 CRN��# [   . ! �� *CRN+  $ Dept.��# [   . ! �� *
DeptName+  $ Class #��# [   . ! �� *ClassNumber+  $ Sect.��# [   . ! �� *SectionNumber+  $ 
Class Name��# [   . ! �� *	ClassName+  $ 	Professor��# [   . ! �� *Prof.FullName+  $ Credits��# [   . ! �� *	Credits+�� $��0��$��2��$��Center=�$��Center=�.��Professor Key�� -   $
��Auto=�$��1��$��2��$��
15,0,15,15q�.���� -   $��Professor_Key_List����. ���� .R��  �� $��15 ��� $��1* �.+��� $��0��$�� ��$��15������ *
Prof_Color+�� $��1��$��Center=�$��Center=�$	��150��$��5,0,0,0q����� *
FullName+
" � Tag" �� *SRUID+���� " ��.���  �@�Tag �� *SRUID+��  �@�ContextMenu# [
 � $��1��$��3�� �� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��15 � �� $
��0.5* ��� $��4* ��� $��2* �.+��� -   $��btn_AddClassRoom��# � ToolTip$# Add a Classroom��$��1��$��0��$��Center=�$��40��$��40��.���� $��/images/classroom.png��$��23��$��27��$��Center=�$��Center=�$ HighQuality=��� -   $��btn_AddClass��$# Add a Class��$��1��$��1��$��Center=�$��40��$��40��.���� $��/images/class.png��$��23��$��27��$��Center=�$��Center=�$ HighQuality=��� -   $��btn_AddProfessor��$# Add a Professor��$��1��$��2��$��Center=�$��40��$��40��.���� $��/images/id.png��$��24��$��27��$��Center=�$��Center=�$ HighQuality=��� -    $��btn_SaveChanges��$# Save Changes��$��1��$��3��$��Center=�$��40��$��40��.���� $��/images/save.png��$��23��$��27��$��Center=�$��Center=�$ HighQuality=��� $��
Class List��#��[ .��� $��-3q����#DDDDDD �� $��30 ��� $��50 ��� $��20 ��� $��1* ��� $��60 � �� $��50 ��� $��1* ��� $��50 ��� $��30 �.+�(� $
��Left=�$��1��$��1��$��Center=�$��40��$��80��.���� $��10 6 0 6q�
���   �Detailed List of Classes�� -!   $
��Auto=�$��Visible=�$��3��$��1��$��
15,0,15,15q�.��  -"   $ Full_Classes_Grid����.  .   .   .   .   �� "  .���  �@�ContextMenu# [    $	 CRN��# [   . ! �� *CRN+  $ Dept.��# [   . ! �� *
DeptName+  $ Class #��# [   . ! �� *ClassNumber+  $ Sect.��# [   . ! �� *SectionNumber+  $ 
Class Name��# [   . ! �� *	ClassName+  $ Credits��# [   . ! �� *	Credits+  $ 	Professor��# [   . ! �� *Prof.FullName+  $
 Days��# [   . ! �� *
ClassDay+  $
 Time��# [   . ! �� *StartTime.FullTime+  $ Building��# [   . ! �� *Classroom.Location+  $ Room Number��# [   . ! �� *Classroom.RoomNum+  $ Capacity��# [   . ! �� *Classroom.AvailableSeats+  $ Section Notes��# [   . ! �� *SectionNotes+  $ Notes��# [   . ! �� *Notes+�� $��
Professors��#��[ .��� $��-3q����#DDDDDD �� $��30 ��� $��50 ��� $��20 ��� $��1* ��� $��60 � �� $��50 ��� $��1* ��� $��50 ��� $��30 �.+�(� $
��Left=�$��1��$��1��$��Center=�$��40��$��80��.���� $��10 6 0 6q�
���   �Detailed List of Professors�� -#   $
��Auto=�$��3��$��1��$��
15,0,15,15q�.��  -$   $ Full_Professors_Grid����.  .   .   .   .   �� "  .���  �@�ContextMenu# [
    $ 
First Name��# [   . ! �� *	FirstName+  $ 	Last Name��# [   . ! �� *
LastName+  $ SRU ID��# [   . ! �� *SRUID+4  .System.Windows.Controls.DataGridTemplateColumn  $ Color��$   CellTemplate$ �� .R��� $��30������ *
Prof_Color+  $ Classes��# [   . ! �� *
classRatio+%   	CellStyle% �� *  $System.Windows.Controls.DataGridCell"  .���  � �� �� $ 
classRatio�& �� 	Converter#& [    $
 Prep��# [   . ! �� *	prepRatio+