namespace UVNetGeneradorDeCodigo.Dialogos
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Generador.ScriptsSQL;
    using UVNetGeneradorDeCodigo.Modelos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public partial class SeleccionColumnas : CDialogo<CProcedimientoValidacionExistenciasYParametros>
    {
        public ObservableCollection<Columna> Columnas { get; } = new ObservableCollection<Columna>();
        private readonly string nombreTabla;

        public SeleccionColumnas(List<CColumnaTabla> columnas, CRepositorio repositorio, string nombreTabla) : base(repositorio)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            DataContext = this;
            Title = "Selección de columnas";
            foreach (var columna in columnas)
            {
                Columnas.Add(new Columna { Nombre = columna.Nombre, TipoDato = columna.TipoDato });
            }
            this.nombreTabla = nombreTabla;
            InitializeComponent();
        }

        private void SeHaSeleccionadoTodo(object sender, RoutedEventArgs e)
        {
            foreach (var columna in Columnas)
            {
                columna.Seleccionado = true;
            }
        }

        private void SeHaDeseleccionadoTodo(object sender, RoutedEventArgs e)
        {
            foreach (var columna in Columnas)
            {
                columna.Seleccionado = false;
            }
        }

        private void CrearProcedimientosValidaciones(object sender, RoutedEventArgs e)
        {
            var tipoProcedimiento = rbExistencia.IsChecked.Value ? ETipoProcedimiento.Existencias : ETipoProcedimiento.Parametros;
            ConstructorDeCodigo.GenerarProcedimientosValidaciones(nombreTabla, Columnas.Where(x => x.Seleccionado).ToList(),
                tipoProcedimiento, chbAgrupar.IsChecked.Value, out MensajeError);
            CrearNuevoDocumento();
        }
    }

    public class Columna : DependencyObject
    {
        public string Nombre
        {
            get { return (string)GetValue(PropiedadNombre); }
            set { SetValue(PropiedadNombre, value); }
        }
        public static readonly DependencyProperty PropiedadNombre =
            DependencyProperty.Register("Nombre", typeof(string), typeof(Columna), new UIPropertyMetadata(string.Empty));

        public string TipoDato
        {
            get { return (string)GetValue(PropiedadTipoDato); }
            set { SetValue(PropiedadTipoDato, value); }
        }
        public static readonly DependencyProperty PropiedadTipoDato =
            DependencyProperty.Register("TipoDato", typeof(string), typeof(Columna), new UIPropertyMetadata(string.Empty));
        public bool Seleccionado
        {
            get { return (bool)GetValue(PropiedadSeleccionado); }
            set { SetValue(PropiedadSeleccionado, value); }
        }
        public static readonly DependencyProperty PropiedadSeleccionado =
            DependencyProperty.Register("Seleccionado", typeof(bool), typeof(Columna), new UIPropertyMetadata(false));
    }
}
