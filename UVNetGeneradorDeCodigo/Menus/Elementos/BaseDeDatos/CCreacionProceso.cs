namespace UVNetGeneradorDeCodigo.Menus.Elementos.BaseDeDatos
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;
    using UVNetGeneradorDeCodigo.Dialogos;

    public class CCreacionProceso : CElementoMenu
    {
        public CCreacionProceso(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override INodeInformation ElementoParaRepositorio => ElementoSeleccionado;

        protected override string Titulo => "Crear proceso";

        protected override void Accion(EventArgs evento)
        {
            new CreacionProceso(Repositorio, ElementoSeleccionado.Name).ShowModal();
        }
    }
}
