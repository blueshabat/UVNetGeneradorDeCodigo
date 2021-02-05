namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CProcedimientoValidacionRegistro : CElementoMenu<Generador.ScriptsSQL.CProcedimientoValidacionRegistro>
    {
        public CProcedimientoValidacionRegistro(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Cabecera registrable";

        protected override void Accion(EventArgs evento)
        {
            var nombreEnPartes = ElementoSeleccionado.InvariantName.Split('.');
            var esquema = nombreEnPartes[0];
            var tabla = nombreEnPartes[1];
            ConstructorDeCodigo.GenerarProcedimientoDeValidacionDeRegistro(esquema, tabla, out MensajeError);
            CrearNuevoDocumento();
        }
    }
}
