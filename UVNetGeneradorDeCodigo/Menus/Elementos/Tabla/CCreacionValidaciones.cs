namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;

    public class CCreacionValidaciones : CSubMenu
    {
        public CCreacionValidaciones(INodeInformation elementoSeleccionado)
        {
            DropDownItems.Add(new CProcedimientoValidacionParametrosYExistencias(elementoSeleccionado));
            DropDownItems.Add(new CProcedimientoValidacionExistenciaDependenciaTablas(elementoSeleccionado));
            DropDownItems.Add(new CProcedimientoValidacionRegistro(elementoSeleccionado));
        }

        protected override string Titulo => "Creación de validaciones";
    }
}
