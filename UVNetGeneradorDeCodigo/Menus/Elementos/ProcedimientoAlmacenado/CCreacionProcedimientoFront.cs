namespace UVNetGeneradorDeCodigo.Menus.Elementos.ProcedimientoAlmacenado
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CCreacionProcedimientoFront : CElementoMenu
    {
        public CCreacionProcedimientoFront(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Crear procedimiento front";

        protected override void Accion(EventArgs evento)
        {
        }
    }
}
