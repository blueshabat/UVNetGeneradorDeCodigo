   M S B A M L   `   `   `  ����Q  MUVNetGeneradorDeCodigo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:UVNetGeneradorDeCodigo.APIUVNetGeneradorDeCodigo.API  5      i9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL+UVNetGeneradorDeCodigo.Generador.ScriptsSQL  5      9    3UVNetGeneradorDeCodigo.Dialogos.ModificacionProceso   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      8API(clr-namespace:UVNetGeneradorDeCodigo.API     5      P
ScriptsSQL9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL     5        � WindowStartupLocation$  CenterScreen=�5	       .��5
      � .+�5   
   ��  �� Orientation$ Vertical=�6   .+�5      �� $��Proceso��6   �� 5      -   5         ��Name$ CbxProcesos��5      .@�5      �� $��Seleccione un proceso��6    �� 
IsSelected . 6?   5      �� 5      $��Parámetros��6   $��0,10,0,0q�6)   &   System.Windows.Controls.DataGrid  5      -   5          Name$ dgParametros��5         AutoGenerateColumns .     CanUserAddRows . $��Center=�6�   ��6I   �� *
parametros+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Nombre��6D   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 6)   �� *Nombre+6D     5      $
 Tipo��6F   	 6)   �� *
TipoDato+6F   4  .System.Windows.Controls.DataGridCheckBoxColumn  5      $	 PRE��6-   	 6:   �� *PRESeleccionado+
 �� Mode$
 TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�     5      $ EN��6-   	 69   �� *ENSeleccionado+$
 TwoWay=�$ PropertyChanged=�6�   5      �� 5      $
��Wrap=�6   $����No se deben incluir los parámetros de salida (@O_EXITO, @O_MENSAJE, @O_RESULTADO, etc) ni los parámetros de indentificación (@I_USUARIO AUT y @I_CODIGO_SISTEMA)��6,   $��9(�6�    �� TextAlignment$ Right=�6�   $��
10,10,10,0q�6�   �� 5      $"��Procedimientos de obtención��6   $��0,10,0,0q�6<   .��5      �� $ 
Horizontal=�6   $��Center=�66   .+�5      �� -   5         ��Name$( "ChbCrearProcedimientosObtencionPRE��5      $	��PRE��6K   $��18��6Y    � Cursor$
 Hand��6e   $��Center=�6s   $��
46,10,46,0q�6�   �� 5      -   5        $' !ChbCrearProcedimientosObtencionEN��5      $��EN��6J   $��18��6W   $
 Hand��6c   $��Center=�6q   $��
46,10,46,0q�6�   5      5      �� 5       -   5         ��Name$ button1��5    P   $��	Modificar��6   $��22��6D   $��0,15,0,0q�6a   $��Center=�6s   $��50��6�   5!      5"      5#      