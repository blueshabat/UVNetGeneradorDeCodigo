namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CProcedimientoListado : CElementoMenu<Generador.ScriptsSQL.CProcedimientoListado>
    {
        public CProcedimientoListado(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Listado";

        protected override void Accion(EventArgs evento)
        {
            var nombreCompleto = ElementoSeleccionado.InvariantName.Split('.');
            var esquema = nombreCompleto[0];
            var tabla = nombreCompleto[1];
            ConstructorDeCodigo.GenerarProcedimientoListado(esquema, tabla, out MensajeError);
            CrearNuevoDocumento();
        }
    }
}
