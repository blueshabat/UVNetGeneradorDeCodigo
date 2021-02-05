namespace UVNetGeneradorDeCodigo.Menus.Elementos.ProcedimientoAlmacenado
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CCopiaEjecucionDeProcedimiento : CElementoMenu<Generador.ScriptsSQL.CEjecucionDeProcedimiento>
    {
        public CCopiaEjecucionDeProcedimiento(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Copiar ejecución";

        protected override void Accion(EventArgs evento)
        {
            var nombreCompleto = ElementoSeleccionado.InvariantName.Split('.');
            var esquema = nombreCompleto[0];
            var procedimiento = nombreCompleto[1];
            ConstructorDeCodigo.GenerarEjecucionDeProcedimiento(esquema, procedimiento, out MensajeError);
            CopiarAlPortapapeles("La ejecución del procedimiento ha sido copiada correctamente.");
        }
    }
}
