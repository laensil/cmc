   M S B A M L   `   `   `  ����  
CMCrepairs    CMCrepairs.Menu.SearchID   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        � 	Resources  5	       �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� RadiusX �� RadiusY ��Name	 �� Triggers
 P� Property P� Value��%  '��       '�� q     �� 5   
   " ��6   .�5      ��  � ��$ Right=�64   �� 5       � ��$
 Bold"�6+   5      �� 5   
   " ��6   .�5      ��  � ��
 � ���6+   �� 5       � ��$ 	10,0,10,0q�6(   �� 5       � �� 5      �� 5      " ��6&   .R�5      � #��z��6   #��z��6?   }�. .+�5      �� $ outerRectangle��6(   $��Stretch=�5   !   $��Stretch=�5   !   #��z��5   !   $ 20��5   !   $ 20��6.   $��5��6;   
�������5   !   �� 5"      $ innerRectangle��6(   $��Stretch=�5#   !   $��Stretch=�5$   !   
������ $��20��5%   !   $ 10��5&   !   $ 20��6.   
�������5'   (   ]� 5*      $!��myContentPresenterDockPanel��6(   .+�5+   "   �� $ myContentPresenter��63   $��1q�6O   #��z��5,   %   $��DemiBold"�5-   %   $��20(�5.   %   $��Center=�5/   %   $��Center=�50   %   51      52      53      55      	 56      P� 57      
 � {� . . �58      ��  � ��
 �   �63   59      P� 5:      
 � {� .  . �5;      ��  � ��
 �����63   5<      P� 5=      
 �@J�IsMouseOver . . �5>      ��  � ��
 � E��63   5?      P� 5@      
 �@J�IsMouseOver .  . �5A      ��  � ��
 � ���63   5B      5D      5F      .��5H      �� .+�5I   
   �� $��Search for Item��6   $��Center=�6*   $��24(�6G   $
��Bold"�6U   $��	0,20,0,40q�6g   �� 5J   
    �� Orientation$ 
Horizontal=�6   $��Center=�6.   
�������$	��590��6b   .+�5K      �� -   5        $��txtOne��5K      $��80��6$   $��18,5,8,5q�6/   6A   �� 5N      -   5        $��chbCompleted��5N      $
��Bold"�6+   $
��Left=�6=   $��Center=�6X   $��Completed Repairs��6s   6�   �� 5O      -   5        $��chbItemWithCustomer��5O      $
��Bold"�62   $
��Left=�6D   $��Center=�6_   $��Item with Customer��6z   $��	20,0,20,0q�6�   �� 5P      -   5        $��chbRWPA��5P      $
��Bold"�6&   $
��Left=�68   $��Center=�6S   $
��RWPA��6n   6�   �� 5S      -   5        $��
chbNowSold��5S      $
��Bold"�6)   $
��Left=�6;   $��Center=�6V   $��Now Sold��6q   6�   5Z      &   System.Windows.Controls.DataGrid  5[   
   -   5        $��dgdIDs��5[      $	��400��6!      CanUserAddRows .     
IsReadOnly .    CanUserResizeColumns .    ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * 6�      AlternatingRowBackground
 �������6.   ��  �� Path$ LoadDataBinding�6�   �� 5\   
   $ 
Horizontal=�6   $��Center=�6.   .+�5]      �� -   5        $��	btnSearch��5]      $	��100��6&   $��40��62   $��Search��6>   $��20,20,20,20q�6O   $��20(�6d   $��Center=�6r   $3��-pack://Application:,,,/Images/#Tranquila DEMO)�6�   �� 5^      -   5        $��btnLoad��5^      $	��100��6$   $��40��60   $
��Load��6<   $��20,20,20,20q�6K   $��20(�6`   $��Center=�6n   $3��-pack://Application:,,,/Images/#Tranquila DEMO)�6�   �� 5_      -	   5        $��	btnCancel��5_      $	��100��6&   $��40��62   $��Cancel��6>   $��20,20,20,20q�6O   $��20(�6d   $3��-pack://Application:,,,/Images/#Tranquila DEMO)�6�   5`      5a      5b      