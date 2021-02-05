namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.Dialogos;

    public class CProcedimientoValidacionParametrosYExistencias : CElementoMenu
    {
        public CProcedimientoValidacionParametrosYExistencias(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Parámetros y existencias";

        protected override void Accion(EventArgs evento)
        {
            var dialogo = new SeleccionColumnas(Repositorio.ObtenerColumnas(ElementoSeleccionado.InvariantName, out string mensajeError), Repositorio, ElementoSeleccionado.Name);
            if (string.IsNullOrEmpty(mensajeError))
            {
                dialogo.ShowModal();
            }
            else
            {
                MessageBox.Show(mensajeError);
            }
        }
    }
}
