   M S B A M L   `   `   `  ���� H  DSRU Scheduler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:Schedule_WPFSchedule_WPF      Schedule_WPF.FileSelect   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:Schedule_WPF     Z VFontAwesome.WPF, Version=4.7.0.37774, Culture=neutral, PublicKeyToken=0758b07a11a4f466-fa$http://schemas.fontawesome.io/icons/    � Title$  SRU Semester Scheduler��$	��450��$	��800�� � Icon$ images\sru.png�� � 
ResizeMode$ NoResize=� �� BitmapScalingMode$ HighQuality=� � WindowStartupLocation$ CenterScreen=� � 	Resources    RoundCorner �� 
TargetType �� Property �� Value	 �� 
TargetType
 �Name ��Name �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName��%E  &	        �� " ��.���  � �� �� "	 ��.R�� $

 grid��.+��� $ border��$ 18��
�������$��0q�
�������._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ��� $ border�� � ��$ 1q��� $ border�� � ��
 ���� ��  � ��
 �������  � ��$ DemiBold"�.��� ����  �� 
StartPoint$	 0,0&� �� EndPoint$	 0,1&�.���  � Color$ #016648�� � Offset$	 0.0Q�� $ #218668��$	 1.0Q� � ColumnDefinitions �� $
��0.3* ��� $��1* ��� $
��0.3* � � RowDefinitions �� $
��0.3* ��� $��1* ��� $
��0.3* �.+�� $��1��$��1�� �� $��1* ��� $
��0.3* �.+�� $��0��$��Center=�.+��� $��images\sru.png��$ HighQuality=�� $��30,0,0,0q� �� $��2* ��� $��1* �.+��� $��images\sru-words.png��$��0��$	��250��$��Bottom=�$ HighQuality=��� $��1��
�������$��20(�$��Light"�$��81,2,0,0q�Semester Scheduler�� -   $��btn_OpenFile��$��1��$��36��$	��100��$��Bottom=� � OverridesDefaultStyle . #��[  .��Select File ..."  FontAwesome.WPF.ImageAwesome  -   $��loadingIcon��$��1��   Icon$	 Cog=�   Spin .    SpinDuration$ 4Q�$��35��$��Bottom=�   
Foreground
 �����$y�Hidden=�