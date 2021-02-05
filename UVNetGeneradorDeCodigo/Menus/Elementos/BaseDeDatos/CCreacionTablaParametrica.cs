namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;
    using UVNetGeneradorDeCodigo.Dialogos;

    public class CCreacionTablaParametrica : CElementoMenu
    {
        public CCreacionTablaParametrica(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override INodeInformation ElementoParaRepositorio => ElementoSeleccionado;

        protected override string Titulo => "Creación tabla paramétrica";

        protected override void Accion(EventArgs evento)
        {
            new CreacionTablaParametrica(Repositorio).ShowDialog();
        }
    }
}
