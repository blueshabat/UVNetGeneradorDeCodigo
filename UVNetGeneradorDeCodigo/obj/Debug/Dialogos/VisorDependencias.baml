   M S B A M L   `   `   `  ����Q  MUVNetGeneradorDeCodigo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:UVNetGeneradorDeCodigo.DialogosUVNetGeneradorDeCodigo.Dialogos  5      i eMicrosoft.VisualStudio.Shell.15.0, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�Zclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0!Microsoft.VisualStudio.PlatformUI 5      7    1UVNetGeneradorDeCodigo.Dialogos.VisorDependencias   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:UVNetGeneradorDeCodigo.Dialogos     5      q
platformUiZclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0     5        � WindowStartupLocation$  CenterScreen=�5   3   $��478.398��6X    � 	Resources 5	       �� DataType3   -UVNetGeneradorDeCodigo.Dialogos.Procedimiento �� ItemsSource � ColumnDefinitions ��Name �� CornerRadius J� SnapsToDevicePixels �� 
StartPoint	 �� EndPoint
 �� Opacity � Color � Offset�� 5
   
   %X  (��       5   '   *, +)�� 6   "  6'    6O   �� *Procedimientos+.R�5      �  5      �� 5      $
 Line��6,   �� 5      $ 	Rectangle��6,   .+�5      �� $	��bdr��6.   $��3��6   $ 3��69   $��1q�6J   $
��Auto��6^   $
��Auto��6k   $��50��6y   $��
0, 5, 0, 5q�6�    . ��5      �� 5      $ 0.5, 0&�62   $	 0.5, 1&�6F   $	
 0.3Q�6X   .��5   "   � $ 	#EEFFFFFF��6/   $ 0Q�6A   � 5   "   $ 	#22000000��6/   $ 1Q�6A   5      ��5      �� 5      $ 0.5, 0&�62   $	 0.5, 1&�6F   .��5   "   � $ 	LightGray��6/   $ 0Q�6A   � 5   "   $
 Gray��6/   $ 1Q�6<   5      ._�5      9� $��RightToLeft=�6#   ��5      �� 5    "   $��LeftToRight=�6D   $��Center=�6`   $��Center=�6}   $��10, 0, 10, 0q�6�   ��6,   �� *Nombre+6�   .��5"      �� $
��Wrap=�6(   $��18(�6<   $	��180��6J   $��LeftToRight=�6Y   �Aquí se cargarán más detalles del procedimiento, tales como los parámetros de entrada y un boton para editar su definición5#      5$      5%      5&      5'      5(      .��5*      R� -   5        $��tvDependencias��5*       5+   
      SelectedItemAreaBrush  SelectedItemBorderBrush  DropShadowEffect��%w  &	        &	 �     &	 F    �� 5,      $ 0.5, 0&�6@   $	 0.5, 1&�6T   $	
 0.4Q�6f   .��5-      � $ 	#22000000��6   $ 0Q�61   � 5.      $ 	#EEFFFFFF��6   $ 1Q�61   5/      �� 50      $ 0.5, 0&�6B   $	 0.5, 1&�6V   .��51      � $
 Gray��6   $ 0Q�6,   � 52      $ 	LightGray��6   $ 1Q�61   53      F� 54      6%   55      ��56   
   �� 57       �� 
TargetType" Q�6   .�58      ��  �� Property �@Q�
IsExpanded �� Value . 6/   �� 59       � |� .  6.    �� Triggers 5:      P� 5;       P� Property �@Q�
IsSelected P� Value . . �5<      ��  � ��# [  6=   �� 5=       � ��# [ 6?   5>      5@      5B      5C      