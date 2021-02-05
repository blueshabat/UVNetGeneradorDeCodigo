namespace UVNetGeneradorDeCodigo.Menus.Elementos.Tabla
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CProcedimientoValidacionExistenciaDependenciaTablas : CElementoMenu<Generador.ScriptsSQL.CProcedimientoValidacionExistenciaDependenciaTablas>
    {
        public CProcedimientoValidacionExistenciaDependenciaTablas(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Existencia dependencia tablas";

        protected override void Accion(EventArgs evento)
        {
            var nombreCompleto = ElementoSeleccionado.InvariantName.Split('.');
            var esquema = nombreCompleto[0];
            var tabla = nombreCompleto[1];
            ConstructorDeCodigo.GenerarProcedimientoDeValidacionDeExistenciaDependenciaTablas(esquema, tabla, out MensajeError);
            CrearNuevoDocumento();
        }
    }
}
