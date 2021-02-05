namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CProcedimientoAlta : CElementoMenu<Generador.ScriptsSQL.CProcedimientoAlta>
    {
        public CProcedimientoAlta(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Alta";

        protected override void Accion(EventArgs evento)
        {
            var nombreCompleto = ElementoSeleccionado.InvariantName.Split('.');
            var esquema = nombreCompleto[0];
            var tabla = nombreCompleto[1];
            ConstructorDeCodigo.GenerarProcedimientoAlta(esquema, tabla, out MensajeError);
            CrearNuevoDocumento();
        }
    }
}
