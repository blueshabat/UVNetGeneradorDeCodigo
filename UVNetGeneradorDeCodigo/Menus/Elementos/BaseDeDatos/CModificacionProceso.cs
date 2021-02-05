namespace UVNetGeneradorDeCodigo.Menus.Elementos.BaseDeDatos
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;
    using UVNetGeneradorDeCodigo.Dialogos;

    public class CModificacionProceso : CElementoMenu
    {
        public CModificacionProceso(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override INodeInformation ElementoParaRepositorio => ElementoSeleccionado;

        protected override string Titulo => "Modificar proceso";

        protected override void Accion(EventArgs evento)
        {
            new ModificacionProceso(Repositorio, ElementoSeleccionado.Name).ShowModal();
        }
    }
}
