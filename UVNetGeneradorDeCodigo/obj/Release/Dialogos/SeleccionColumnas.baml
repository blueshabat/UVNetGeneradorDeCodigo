   M S B A M L   `   `   `  ���� Q  MUVNetGeneradorDeCodigo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:UVNetGeneradorDeCodigo.APIUVNetGeneradorDeCodigo.API  i9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL+UVNetGeneradorDeCodigo.Generador.ScriptsSQL  7    1UVNetGeneradorDeCodigo.Dialogos.SeleccionColumnas   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    >d2http://schemas.microsoft.com/expression/blend/2008    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     8API(clr-namespace:UVNetGeneradorDeCodigo.API     P
ScriptsSQL9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL       � WindowStartupLocation$  CenterScreen=�.��� .+���  �� Orientation$ Vertical=�.+�&   System.Windows.Controls.DataGrid  -     Name$ 
dgColumnas��   AutoGenerateColumns .     CanUserAddRows .  ���� *
Columnas+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Nombre��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Nombre+  $
 Tipo�� �� *
TipoDato+4  .System.Windows.Controls.DataGridTemplateColumn   �� -      CellTemplate �� .R��� 	 d� 	IsChecked	 �� 
 �� Path$
 Seleccionado� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� $��Tipo de validación��$
��0,10q�.���� $ 
Horizontal=�$��Center=�.+�	� -   $��rbParametro�� 	� 	GroupName$
 tipo��$
��10,0q�.��
Parámetro	� -   $��rbExistencia��$
 tipo��$
	 TrueY�$
��10,0q�.��
Existencia�� -   $��
chbAgrupar��$6��0Agrupar la selección en un procedimiento único��$��Center=�$��0,10,0,0q��� -    ��Name$ btnCrearProcedimiento��$��Crear procedimiento��$��22��$��	0,15,0,10q�$��Center=�$��50��