   M S B A M L   `   `   `  ����Q  MUVNetGeneradorDeCodigo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:UVNetGeneradorDeCodigo.APIUVNetGeneradorDeCodigo.API  5      i9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL+UVNetGeneradorDeCodigo.Generador.ScriptsSQL  5      7    1UVNetGeneradorDeCodigo.Dialogos.SeleccionColumnas   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      8API(clr-namespace:UVNetGeneradorDeCodigo.API     5      P
ScriptsSQL9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL     5        � WindowStartupLocation$  CenterScreen=�5      .��5      � .+�5   
   ��  �� Orientation$ Vertical=�6   .+�5      &   System.Windows.Controls.DataGrid  -   5          Name$ 
dgColumnas��5         AutoGenerateColumns .     CanUserAddRows .  ��6G   �� *
Columnas+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Nombre��6D   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6)   �� *Nombre+6D     5      $
 Tipo��6F    6)   �� *
TipoDato+6F   4  .System.Windows.Controls.DataGridTemplateColumn  5       5      �� 5      -   5        5   F      CellTemplate 5      �� 5      .R�5   "   �� 	 d� 	IsChecked	 6+   �� 
 �� Path$
 Seleccionado� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5      5      5      �� 5      $��Tipo de validación��6   $
��0,10q�63   .��5       �� $ 
Horizontal=�6   $��Center=�66   .+�5!      	� -   5        $��rbParametro��5!   "    	� 	GroupName$
 tipo��65   $
��10,0q�6F   .��6T   
Parámetro6_   	� 5"      -   5        $��rbExistencia��5"   "   $
 tipo��66   $
	 TrueY�6G   $
��10,0q�6X   .��6f   
Existencia6r   5#      5$      �� 5%      -   5        $��
chbAgrupar��5%   P   $6��0Agrupar la selección en un procedimiento único��6   $��Center=�6b   $��0,10,0,0q�6   �� 5&      -   5         ��Name$ btnCrearProcedimiento��5&   g   $��Crear procedimiento��6   $��22��6[   $��	0,15,0,10q�6�   $��Center=�6�   $��50��6�   5'      5(      5)      