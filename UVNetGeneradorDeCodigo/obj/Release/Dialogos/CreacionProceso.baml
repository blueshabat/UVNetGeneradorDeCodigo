   M S B A M L   `   `   `  ���� Q  MUVNetGeneradorDeCodigo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:UVNetGeneradorDeCodigo.APIUVNetGeneradorDeCodigo.API  i9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL+UVNetGeneradorDeCodigo.Generador.ScriptsSQL  5    /UVNetGeneradorDeCodigo.Dialogos.CreacionProceso   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    >d2http://schemas.microsoft.com/expression/blend/2008    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     8API(clr-namespace:UVNetGeneradorDeCodigo.API     P
ScriptsSQL9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL       � WindowStartupLocation$  CenterScreen=�$��502.5��$��529.375��.��� .+���  �� Orientation$ Vertical=�.+��� $��Esquemas���� -    ��Name$ CbxEsquemas���� $��Entidad���� -    ��Name$ 
TxtEntidad��$��23�� �� TextWrapping$
 Wrap=�$�� �� �� MaxLines$ 1�� �� 	MaxLength$ 30�� �� CharacterCasing$ Upper=��� $��Proceso���� -   $ 
TxtProceso��$��23��$
 Wrap=�$�� ��$ 4��$ 1��$ Upper=��� -    ��Name$ ChbRequiereSubproceso��$��Requiere subproceso��$��18��	 � Cursor$
	 Hand��$��Center=�$��
84,10,84,0q��� -   $��LblSubproceso��$��
Subproceso��$y�Hidden=��� -   $ TxtSubProceso��$��23��$
 Wrap=�$�� ��$ 4��$ 1��$y�Hidden=�$ Upper=��� $"��Procedimientos de obtención��$��0,10,0,0q�.���� $ 
Horizontal=�$��Center=�.+��� -   $( "ChbCrearProcedimientosObtencionPRE��$	��PRE��$��18��$
	 Hand��$��Center=�$��
46,10,46,0q��� -   $' !ChbCrearProcedimientosObtencionEN��$��EN��$��18��$
	 Hand��$��Center=�$��
46,10,46,0q��� $��Parámetros��� 
 � ColumnDefinitions
 �� $��* ��� $��55 ��� $��* �.+��� $��0��$��0q�.���� -	   $ TxtParametros��$
 Wrap=�$�� ��$ 50��" �� VerticalScrollBarVisibility$
 Auto=�$ Upper=� �� AcceptsReturn . �� $��1��$��0q�$
��-6,0q�.���� -
   $��Dar formato��$��Center=�$��Center=�$��10(� � UseLayoutRounding .  $��SemiCondensed&�$��
ExtraLight"��� $��2��$��0q�.��&   System.Windows.Controls.DataGrid  -     Name$ dgParametros��   AutoGenerateColumns .     CanUserAddRows . $��Center=����� *
parametros+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Nombre��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Nombre+  $
 Tipo�� �� *
TipoDato+4  .System.Windows.Controls.DataGridCheckBoxColumn  $	 PRE�� �� *PRESeleccionado+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�  $ EN�� �� *ENSeleccionado+$ TwoWay=�$ PropertyChanged=��� $
��Wrap=�$����No se deben incluir los parámetros de salida (@O_EXITO, @O_MENSAJE, @O_RESULTADO, etc) ni los parámetros de indentificación (@I_USUARIO AUT y @I_CODIGO_SISTEMA)��$��9(� �� TextAlignment$ Right=�$��	10,10,0,0q��� -    ��Name$ button1��$��Crear��$��22��$��0,15,0,0q�$��Center=�$��50��