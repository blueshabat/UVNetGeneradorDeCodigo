   M S B A M L   `   `   `  ���� Q  MUVNetGeneradorDeCodigo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:UVNetGeneradorDeCodigo.APIUVNetGeneradorDeCodigo.API  i9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL+UVNetGeneradorDeCodigo.Generador.ScriptsSQL  9    3UVNetGeneradorDeCodigo.Dialogos.ModificacionProceso   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    >d2http://schemas.microsoft.com/expression/blend/2008    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     8API(clr-namespace:UVNetGeneradorDeCodigo.API     P
ScriptsSQL9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL       � WindowStartupLocation$  CenterScreen=�.��� .+���  �� Orientation$ Vertical=�.+��� $��Proceso���� -    ��Name$ CbxProcesos��.@��� $��Seleccione un proceso�� �� 
IsSelected . �� $��Parámetros��$��0,10,0,0q�&   System.Windows.Controls.DataGrid  -     Name$ dgParametros��   AutoGenerateColumns .     CanUserAddRows . $��Center=����� *
parametros+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Nombre��1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 �� *Nombre+  $
 Tipo��	 �� *
TipoDato+4  .System.Windows.Controls.DataGridCheckBoxColumn  $	 PRE��	 �� *PRESeleccionado+
 �� Mode$
 TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�  $ EN��	 �� *ENSeleccionado+$
 TwoWay=�$ PropertyChanged=��� $
��Wrap=�$����No se deben incluir los parámetros de salida (@O_EXITO, @O_MENSAJE, @O_RESULTADO, etc) ni los parámetros de indentificación (@I_USUARIO AUT y @I_CODIGO_SISTEMA)��$��9(� �� TextAlignment$ Right=�$��
10,10,10,0q��� $"��Procedimientos de obtención��$��0,10,0,0q�.���� $ 
Horizontal=�$��Center=�.+��� -    ��Name$( "ChbCrearProcedimientosObtencionPRE��$	��PRE��$��18�� � Cursor$
 Hand��$��Center=�$��
46,10,46,0q��� -   $' !ChbCrearProcedimientosObtencionEN��$��EN��$��18��$
 Hand��$��Center=�$��
46,10,46,0q��� -    ��Name$ button1��$��	Modificar��$��22��$��0,15,0,0q�$��Center=�$��50��