namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CProcedimientoBaja : CElementoMenu<Generador.ScriptsSQL.CProcedimientoBaja>
    {
        public CProcedimientoBaja(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Baja";

        protected override void Accion(EventArgs evento)
        {
            var nombreCompleto = ElementoSeleccionado.InvariantName.Split('.');
            var esquema = nombreCompleto[0];
            var tabla = nombreCompleto[1];
            ConstructorDeCodigo.GenerarProcedimientoBaja(esquema, tabla);
            CrearNuevoDocumento();
        }
    }
}
