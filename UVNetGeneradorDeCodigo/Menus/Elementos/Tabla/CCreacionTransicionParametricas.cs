namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;
    using UVNetGeneradorDeCodigo.Dialogos;

    public class CCreacionTransicionParametricas : CElementoMenu
    {
        public CCreacionTransicionParametricas(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Crear proceso de transición";

        protected override void Accion(EventArgs evento)
        {
            var esquema = ElementoSeleccionado.InvariantName.Split('.')[0];
            var tabla = ElementoSeleccionado.InvariantName.Split('.')[1];
            new CreacionTransicionParametricas(Repositorio, tabla, esquema).ShowModal();
        }
    }
}
