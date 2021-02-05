namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CProcedimientoModificacion : CElementoMenu<Generador.ScriptsSQL.CProcedimientoModificacion>
    {
        public CProcedimientoModificacion(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Modificación";

        protected override void Accion(EventArgs evento)
        {
            var nombreCompleto = ElementoSeleccionado.InvariantName.Split('.');
            var esquema = nombreCompleto[0];
            var tabla = nombreCompleto[1];
            ConstructorDeCodigo.GenerarProcedimientoModificacion(esquema, tabla);
            CrearNuevoDocumento();
        }
    }
}
