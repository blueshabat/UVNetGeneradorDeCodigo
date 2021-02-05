namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;

    public class CCreacionProcedimientos : CSubMenu
    {
        public CCreacionProcedimientos(INodeInformation elementoSeleccionado)
        {
            DropDownItems.Add(new CProcedimientoAlta(elementoSeleccionado));
            DropDownItems.Add(new CProcedimientoBaja(elementoSeleccionado));
            DropDownItems.Add(new CProcedimientoModificacion(elementoSeleccionado));
            DropDownItems.Add(new CProcedimientoObtencion(elementoSeleccionado));
            DropDownItems.Add(new CProcedimientoListado(elementoSeleccionado));
        }

        protected override string Titulo => "Creación de procedimientos";
    }
}
