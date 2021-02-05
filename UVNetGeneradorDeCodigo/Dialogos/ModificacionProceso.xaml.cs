namespace UVNetGeneradorDeCodigo.Dialogos
{
    using Microsoft.VisualStudio.Shell;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Modelos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public partial class ModificacionProceso : CDialogo<Generador.ScriptsSQL.CModificacionProceso>
    {
        public ModificacionProceso(CRepositorio repositorio, string baseDeDatos) : base(repositorio)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            this.repositorio = repositorio;
            this.baseDeDatos = baseDeDatos;
            Title = "Modificación de proceso";
            InitializeComponent();
            var entidades = repositorio.ObtenerEntidades(baseDeDatos, out string mensajeError);
            CbxProcesos.Items.Clear();
            CbxProcesos.Items.Add(new ComboBoxItem { Content = "Seleccione un proceso", IsSelected = true });
            if (string.IsNullOrEmpty(mensajeError))
            {
                foreach (var entidad in entidades)
                {
                    CbxProcesos.Items.Add(new ComboBoxItem { Content = entidad });
                }
            }
            else
            {
                MessageBox.Show(mensajeError);
            }
        }

        public ObservableCollection<Parametro> Parametros { get; } = new ObservableCollection<Parametro>();
        private readonly CRepositorio repositorio;
        private readonly string baseDeDatos;
        private bool existeProcedimientoObtencionPRE;
        private bool existeProcedimientoObtencionEN;
        private bool existeProcesoPRE;
        private bool existeProcesoEN;

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var modificacionProceso = new CModificacionProceso
            {
                BaseDeDatos = baseDeDatos,
                Esquema = ((ComboBoxItem)CbxProcesos.SelectedValue).Content.ToString().Split('.')[0],
                Proceso = ((ComboBoxItem)CbxProcesos.SelectedValue).Content.ToString().Split('.')[1],
                Columnas = Parametros.ToList()
            };
            if (!existeProcedimientoObtencionPRE && ChbCrearProcedimientosObtencionPRE.IsChecked.Value)
            {
                modificacionProceso.ProcedimientoObtencionPRE = OperacionModificacion.Crear;
            }
            if (existeProcedimientoObtencionPRE && ChbCrearProcedimientosObtencionPRE.IsChecked.Value)
            {
                modificacionProceso.ProcedimientoObtencionPRE = OperacionModificacion.Modificar;
            }
            if (existeProcedimientoObtencionPRE && !ChbCrearProcedimientosObtencionPRE.IsChecked.Value)
            {
                modificacionProceso.ProcedimientoObtencionPRE = OperacionModificacion.Eliminar;
            }
            if (!existeProcedimientoObtencionEN && ChbCrearProcedimientosObtencionEN.IsChecked.Value)
            {
                modificacionProceso.ProcedimientoObtencionEN = OperacionModificacion.Crear;
            }
            if (existeProcedimientoObtencionEN && ChbCrearProcedimientosObtencionEN.IsChecked.Value)
            {
                modificacionProceso.ProcedimientoObtencionEN = OperacionModificacion.Modificar;
            }
            if (existeProcedimientoObtencionEN && !ChbCrearProcedimientosObtencionEN.IsChecked.Value)
            {
                modificacionProceso.ProcedimientoObtencionEN = OperacionModificacion.Eliminar;
            }
            if (existeProcesoPRE && Parametros.Any(x => x.PRESeleccionado))
            {
                modificacionProceso.OperacionPRE = OperacionModificacion.Modificar;
            }
            if (!existeProcesoPRE && Parametros.Any(x => x.PRESeleccionado))
            {
                modificacionProceso.OperacionPRE = OperacionModificacion.Crear;
            }
            if (existeProcesoPRE && !Parametros.Any(x => x.PRESeleccionado))
            {
                modificacionProceso.OperacionPRE = OperacionModificacion.Eliminar;
            }
            if (existeProcesoEN && Parametros.Any(x => x.ENSeleccionado))
            {
                modificacionProceso.OperacionEN = OperacionModificacion.Modificar;
            }
            if (!existeProcesoEN && Parametros.Any(x => x.ENSeleccionado))
            {
                modificacionProceso.OperacionEN = OperacionModificacion.Crear;
            }
            if (existeProcesoEN && !Parametros.Any(x => x.ENSeleccionado))
            {
                modificacionProceso.OperacionEN = OperacionModificacion.Eliminar;
            }
            ConstructorDeCodigo.GenerarModificacionDeProceso(modificacionProceso, out MensajeError);
            CrearNuevoDocumento();
        }

        private void SeHaSeleccionadoUnProceso(object sender, SelectionChangedEventArgs e)
        {
            if (CbxProcesos.SelectedIndex == 0 || CbxProcesos.SelectedIndex == -1)
            {
                return;
            }
            var esquema = ((ComboBoxItem)CbxProcesos.SelectedValue).Content.ToString().Split('.')[0];
            var proceso = ((ComboBoxItem)CbxProcesos.SelectedValue).Content.ToString().Split('.')[1];
            var parametrosProceso = repositorio.ObtenerParametrosProceso(baseDeDatos, esquema, proceso, out string mensajeError);
            var procedimientosDeUnProceso = repositorio.ObtenerProcedimientosDeUnProceso(baseDeDatos, esquema, proceso, out mensajeError);
            existeProcedimientoObtencionPRE = procedimientosDeUnProceso.Any(x => x == $"P_B_PR_{proceso}_PRE_O");
            existeProcedimientoObtencionEN = procedimientosDeUnProceso.Any(x => x == $"P_B_PR_{proceso}_EN_O");
            ChbCrearProcedimientosObtencionPRE.IsChecked = existeProcedimientoObtencionPRE;
            ChbCrearProcedimientosObtencionEN.IsChecked = existeProcedimientoObtencionEN;
            Parametros.Clear();
            if (string.IsNullOrEmpty(mensajeError))
            {
                if (parametrosProceso.Any())
                {
                    var todosLosParametrosSonEN = !parametrosProceso.Any(x => !x.PRESeleccionado);
                    foreach (var parametro in parametrosProceso)
                    {
                        if (!string.IsNullOrEmpty(parametro.Nombre))
                        {
                            Parametros.Add(new Parametro
                            {
                                Nombre = parametro.Nombre,
                                TipoDato = parametro.TipoDeDato,
                                PRESeleccionado = parametro.PRESeleccionado,
                                ENSeleccionado = todosLosParametrosSonEN || !parametro.PRESeleccionado
                            });
                        }
                    }
                    existeProcesoPRE = Parametros.Any(x => x.PRESeleccionado);
                    existeProcesoEN = Parametros.Any(x => x.ENSeleccionado);
                    dgParametros.ItemsSource = Parametros;
                }
            }
            else
            {
                MessageBox.Show(mensajeError);
            }
        }

        private void SeHaSeleccionadoUnaOperacion(object sender, SelectionChangedEventArgs e)
        {
        }

        private void VerificarPosibilidadCreacionProcedimientosObtencion(object sender, SelectionChangedEventArgs e)
        {
            ChbCrearProcedimientosObtencionPRE.IsEnabled = Parametros.Any(x => x.PRESeleccionado);
            ChbCrearProcedimientosObtencionEN.IsEnabled = Parametros.Any(x => x.ENSeleccionado);
            if (!ChbCrearProcedimientosObtencionPRE.IsEnabled)
            {
                ChbCrearProcedimientosObtencionPRE.IsChecked = false;
            }
            if (!ChbCrearProcedimientosObtencionEN.IsEnabled)
            {
                ChbCrearProcedimientosObtencionEN.IsChecked = false;
            }
        }
    }
}
