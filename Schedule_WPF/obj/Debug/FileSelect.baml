   M S B A M L   `   `   `  ����H  DSRU Scheduler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:Schedule_WPFSchedule_WPF  5   	       Schedule_WPF.FileSelect   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:Schedule_WPF     5   	   Z VFontAwesome.WPF, Version=4.7.0.37774, Culture=neutral, PublicKeyToken=0758b07a11a4f466-fa$http://schemas.fontawesome.io/icons/  5   	     � Title$  SRU Semester Scheduler��5	   	   $	��450��6(   $	��800��65    � Icon$ images\sru.png��6A    � 
ResizeMode$ NoResize=�6W    �� BitmapScalingMode$ HighQuality=�6n    � WindowStartupLocation$ CenterScreen=�6�    � 	Resources 5
         RoundCorner �� 
TargetType �� Property �� Value	 �� 
TargetType
 �Name ��Name �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName��%6  &	        �� 5   
   " ��6$   .�5      ��  � �� 5      �� 5      "	 ��6&   .R�5      � $

 grid��6   .+�5      �� $ border��6%   $ 18��65   
�������$��0q�6[   
�������._�5   "   �� $��Center=�63   $��Center=�6P   5      5       5      P� 5       �@J�IsMouseOver . . �5   "   �� $ border��6D    � ��$ 1q�6X   �� 5   "   $ border��6?    � ��
 ���� 6S   �� 5   "    � ��
 �����6?   �� 5   "    � ��$ DemiBold"�6?   5      5      5      5      5       .��5!      � ��5"   
   �� 5#       �� 
StartPoint$	 0,0&�6"    �� EndPoint$	 0,1&�63   .��5$      �  � Color$ #016648��6    � Offset$	 0.0Q�6/   � 5%      $ #218668��6   $	 1.0Q�6/   5&       � ColumnDefinitions 5(   
   �� 5)      $
��0.3* �6   �� 5*      $��1* �6   �� 5+      $
��0.3* �6    � RowDefinitions 5-   
   �� 5.      $
��0.3* �6   �� 5/      $��1* �6   �� 50      $
��0.3* �6   .+�52   
   � $��1��6   $��1��6    53      �� 54      $��1* �6    �� 55      $
��0.3* �6    .+�57      � $��0��6   $��Center=�6&   .+�58      �� $��images\sru.png��6   $ HighQuality=�60   � 59      $��30,0,0,0q�6    5:      �� 5;      $��2* �6(   �� 5<      $��1* �6(   .+�5>      �� $��images\sru-words.png��6   $��0��6:   $	��250��6G   $��Bottom=�6S   $ HighQuality=�6n   �� 5?      $��1��6    
�������$��20(�6@   $��Light"�6N   $��81,2,0,0q�6b   Semester Scheduler6�   5@      5A      �� 5B      -   5        $��btn_OpenFile��5B      $��1��6E   $��36��6R   $	��100��6^   $��Bottom=�6j    � OverridesDefaultStyle . #��[  6�   .��6�   Select File ...6�   "  FontAwesome.WPF.ImageAwesome  5C      -   5        $��loadingIcon��5C      $��1��61      Icon$	 Cog=�6>      Spin .    SpinDuration$ 4Q�6U   $��35��6f   $��Bottom=�6r      
Foreground
 �����$y�Hidden=�6�   5D      5E      5F      