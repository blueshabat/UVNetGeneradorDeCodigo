   M S B A M L   `   `   `  ����Q  MUVNetGeneradorDeCodigo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:UVNetGeneradorDeCodigo.APIUVNetGeneradorDeCodigo.API  5      i9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL+UVNetGeneradorDeCodigo.Generador.ScriptsSQL  5      5    /UVNetGeneradorDeCodigo.Dialogos.CreacionProceso   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      8API(clr-namespace:UVNetGeneradorDeCodigo.API     5      P
ScriptsSQL9clr-namespace:UVNetGeneradorDeCodigo.Generador.ScriptsSQL     5        � WindowStartupLocation$  CenterScreen=�5	       $��502.5��6E   $��529.375��6T   .��5
      � .+�5   
   ��  �� Orientation$ Vertical=�6   .+�5      �� $��Esquemas��6   �� 5      -   5         ��Name$ CbxEsquemas��5      5      �� 5      $��Entidad��6   �� 5      -   5         ��Name$ 
TxtEntidad��5      $��23��6*    �� TextWrapping$
 Wrap=�66   $�� ��6J    �� MaxLines$ 1��6R    �� 	MaxLength$ 30��6_    �� CharacterCasing$ Upper=�6n   �� 5      $��Proceso��6   �� 5      -   5        $ 
TxtProceso��5      $��23��6*   $
 Wrap=�66   $�� ��6J   $ 4��6R   $ 1��6`   $ Upper=�6m   �� 5      -   5         ��Name$ ChbRequiereSubproceso��5      $��Requiere subproceso��66   $��18��6T   	 � Cursor$
	 Hand��6`   $��Center=�6n   $��
84,10,84,0q�6�   6�   �� 5      -   5        $��LblSubproceso��5   )   $��
Subproceso��6   $y�Hidden=�6>   �� 5      -   5        $ TxtSubProceso��5      $��23��6-   $
 Wrap=�69   $�� ��6M   $ 4��6U   $ 1��6c   $y�Hidden=�6p   $ Upper=�6�   �� 5      $"��Procedimientos de obtención��6   $��0,10,0,0q�6<   .��5      �� $ 
Horizontal=�6   $��Center=�66   .+�5      �� -   5        $( "ChbCrearProcedimientosObtencionPRE��5      $	��PRE��6K   $��18��6Y   $
	 Hand��6e   $��Center=�6s   $��
46,10,46,0q�6�   �� 5      -   5        $' !ChbCrearProcedimientosObtencionEN��5      $��EN��6J   $��18��6W   $
	 Hand��6c   $��Center=�6q   $��
46,10,46,0q�6�   5      5      �� 5      $��Parámetros��6   � 5      
 � ColumnDefinitions
 5      �� 5       $��* �6'   �� 5!      $��55 �6'   �� 5"      $��* �6'   .+�5$      �� $��0��6   $��0q�6+   .��5%      �� -	   5        $ TxtParametros��5%      $
 Wrap=�65   $�� ��6I   $ 50��6Q   " �� VerticalScrollBarVisibility$
 Auto=�6_   $ Upper=�6�    �� AcceptsReturn . 6�   5&      �� 5'      $��1��6   $��0q�6,   $
��-6,0q�6@   .��5(      �� -
   5        $��Dar formato��5(      $��Center=�63   $��Center=�6P   $��10(�6�    � UseLayoutRounding .  $��SemiCondensed&�6�   $��
ExtraLight"�6�   5)      �� 5+      $��2��6   $��0q�6+   .��5,      &   System.Windows.Controls.DataGrid  -   5          Name$ dgParametros��5,         AutoGenerateColumns .     CanUserAddRows . $��Center=�6�   ��6Q   �� *
parametros+   Columns 5-      0  *System.Windows.Controls.DataGridTextColumn  5.      ,  &System.Windows.Controls.DataGridColumn   Header$ Nombre��6L   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Nombre+6L     5/      $
 Tipo��6N    61   �� *
TipoDato+6N   4  .System.Windows.Controls.DataGridCheckBoxColumn  50      $	 PRE��65    6B   �� *PRESeleccionado+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�     51      $ EN��65    6A   �� *ENSeleccionado+$ TwoWay=�$ PropertyChanged=�6�   53      54      55      �� 56      $
��Wrap=�6   $����No se deben incluir los parámetros de salida (@O_EXITO, @O_MENSAJE, @O_RESULTADO, etc) ni los parámetros de indentificación (@I_USUARIO AUT y @I_CODIGO_SISTEMA)��6,   $��9(�6�    �� TextAlignment$ Right=�6�   $��	10,10,0,0q�6�   �� 57      -   5         ��Name$ button1��57   H   $��Crear��6   $��22��6<   $��0,15,0,0q�6Y   $��Center=�6k   $��50��6�   58      59      5:      