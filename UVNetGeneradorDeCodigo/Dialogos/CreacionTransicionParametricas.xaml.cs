namespace UVNetGeneradorDeCodigo.Dialogos
{
    using Microsoft.VisualStudio.Shell;
    using System.Windows;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Generador.ScriptsSQL;
    using UVNetGeneradorDeCodigo.Repositorio;

    public partial class CreacionTransicionParametricas : CDialogo<CTransicionParametricas>
    {
        public CreacionTransicionParametricas(CRepositorio repositorio, string tabla, string esquema) : base(repositorio)
        {
            Title = "Creación de proceso de transición";
            SizeToContent = SizeToContent.WidthAndHeight;
            MinWidth = 500;
            this.tabla = tabla;
            this.esquema = esquema;
            InitializeComponent();
        }

        private readonly string tabla;
        private readonly string esquema;

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ConstructorDeCodigo.GenerarProcesoDeTransicion(esquema, tabla, TxtProceso.Text, TxtProcesoDescripcion.Text, out MensajeError);
            CrearNuevoDocumento();
        }

        private void TxtProcesoDescripcion_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtProcesoDescripcion.Text))
            {
                if (TxtProcesoDescripcion.Text.Length < 4)
                {
                    TxtProceso.Text = TxtProcesoDescripcion.Text;
                }
                else
                {
                    TxtProceso.Text = TxtProcesoDescripcion.Text.Substring(0, 3);
                }
            }
        }
    }
}
