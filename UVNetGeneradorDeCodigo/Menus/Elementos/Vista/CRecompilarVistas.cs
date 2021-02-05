namespace UVNetGeneradorDeCodigo.Menus.Elementos.Vista
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;
    using System.Windows;

    public class CRecompilarVistas : CElementoMenu
    {
        public CRecompilarVistas(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Recompilar todas las vistas";

        protected override void Accion(EventArgs evento)
        {
            Repositorio.RecompilarVistas(out string mensajeError);
            MessageBox.Show(string.IsNullOrEmpty(mensajeError) ? "El comando fue ejecutado correctamente." : mensajeError);
        }
    }
}
