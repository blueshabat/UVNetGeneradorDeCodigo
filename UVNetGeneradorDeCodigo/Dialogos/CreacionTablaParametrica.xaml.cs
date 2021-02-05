namespace UVNetGeneradorDeCodigo.Dialogos
{
    using Microsoft.VisualStudio.Shell;
    using System.Windows;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Generador.ScriptsSQL;
    using UVNetGeneradorDeCodigo.Repositorio;

    public partial class CreacionTablaParametrica : CDialogo<CCreacionTablaParametrica>
    {
        public CreacionTablaParametrica(CRepositorio repositorio) : base(repositorio)
        {
            InitializeComponent();
        }

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(TxtNombreTabla.Text))
            {
                MessageBox.Show("Debe ingresar un nombre para la tabla.");
            }
            else
            {
                ConstructorDeCodigo.GenerarCreacionTablaParametrica(TxtNombreTabla.Text.ToUpper(), out MensajeError);
                CrearNuevoDocumento();
            }
        }
    }
}
