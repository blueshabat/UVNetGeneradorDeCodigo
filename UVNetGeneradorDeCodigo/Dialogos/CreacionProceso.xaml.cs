namespace UVNetGeneradorDeCodigo.Dialogos
{
    using Microsoft.VisualStudio.Shell;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Generador.ScriptsSQL;
    using UVNetGeneradorDeCodigo.Modelos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public partial class CreacionProceso : CDialogo<CCreacionProceso>
    {
        public CreacionProceso(CRepositorio repositorio, string baseDeDatos) : base(repositorio)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            Title = "Creación de proceso";
            InitializeComponent();
            var esquemas = repositorio.ObtenerEsquemas(baseDeDatos, out string mensajeError);
            CbxEsquemas.Items.Clear();
            CbxEsquemas.Items.Add(new ComboBoxItem { Content = "Seleccione un esquema", IsSelected = true });
            if (string.IsNullOrEmpty(mensajeError))
            {
                foreach (var esquema in esquemas)
                {
                    CbxEsquemas.Items.Add(new ComboBoxItem { Content = esquema });
                }
            }
            else
            {
                MessageBox.Show(mensajeError);
            }
        }

        public ObservableCollection<Parametro> Parametros { get; } = new ObservableCollection<Parametro>();

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var esquema = ((ComboBoxItem)CbxEsquemas.SelectedValue).Content.ToString();
            if (!ChbRequiereSubproceso.IsChecked.Value)
            {
                TxtSubProceso.Text = string.Empty;
            }
            ConstructorDeCodigo.GenerarCreacionDeProceso(esquema, TxtEntidad.Text.ToUpper(), TxtProceso.Text.ToUpper(),
                TxtSubProceso.Text.ToUpper(), Parametros.ToList(), ChbCrearProcedimientosObtencionPRE.IsChecked.Value,
                ChbCrearProcedimientosObtencionEN.IsChecked.Value, out MensajeError);
            CrearNuevoDocumento();
        }

        private IEnumerable<CColumnaTabla> ObtenerParametrosEntrada()
        {
            foreach (var parametro in TxtParametros.Text.Split('\n'))
            {
                var parametroPorPartes = parametro.Split(' ').Where(x => x != "" && x != " " && x != "\r").ToArray();
                switch (parametroPorPartes.Length)
                {
                    case 1:
                        yield return new CColumnaTabla { Nombre = parametroPorPartes[0].Trim() };
                        break;
                    case 2:
                        yield return new CColumnaTabla { Nombre = parametroPorPartes[0].Trim(), TipoDato = parametroPorPartes[1].Trim() };
                        break;
                    default:
                        yield return new CColumnaTabla { };
                        break;
                }
            }
        }

        private void ChbRequiereSubproceso_Checked(object sender, RoutedEventArgs e)
        {
            LblSubproceso.Visibility = TxtSubProceso.Visibility = ChbRequiereSubproceso.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Parametros.Clear();
            foreach (var parametro in ObtenerParametrosEntrada())
            {
                Parametros.Add(new Parametro { Nombre = parametro.Nombre, TipoDato = parametro.TipoDato });
            }
            dgParametros.ItemsSource = Parametros;
        }
    }

    public class Parametro : DependencyObject
    {
        public string Nombre
        {
            get { return (string)GetValue(PropiedadNombre); }
            set { SetValue(PropiedadNombre, value); }
        }
        public static readonly DependencyProperty PropiedadNombre =
            DependencyProperty.Register("Nombre", typeof(string), typeof(Parametro), new UIPropertyMetadata(string.Empty));

        public string TipoDato
        {
            get { return (string)GetValue(PropiedadTipoDato); }
            set { SetValue(PropiedadTipoDato, value); }
        }
        public static readonly DependencyProperty PropiedadTipoDato =
            DependencyProperty.Register("TipoDato", typeof(string), typeof(Parametro), new UIPropertyMetadata(string.Empty));
        public bool PRESeleccionado
        {
            get { return (bool)GetValue(PropiedadPRESeleccionado); }
            set { SetValue(PropiedadPRESeleccionado, value); }
        }
        public static readonly DependencyProperty PropiedadPRESeleccionado =
            DependencyProperty.Register("PRESeleccionado", typeof(bool), typeof(Parametro), new UIPropertyMetadata(false));

        public bool ENSeleccionado
        {
            get { return (bool)GetValue(PropiedadENSeleccionado); }
            set { SetValue(PropiedadENSeleccionado, value); }
        }
        public static readonly DependencyProperty PropiedadENSeleccionado =
            DependencyProperty.Register("ENSeleccionado", typeof(bool), typeof(Parametro), new UIPropertyMetadata(false));
    }
}
