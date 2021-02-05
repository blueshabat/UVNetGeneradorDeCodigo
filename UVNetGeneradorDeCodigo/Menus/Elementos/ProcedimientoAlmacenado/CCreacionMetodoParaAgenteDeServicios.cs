namespace UVNetGeneradorDeCodigo.Menus.Elementos.ProcedimientoAlmacenado
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CCreacionMetodoParaAgenteDeServicios : CElementoMenu<Generador.CSharp.CCreacionMetodoParaAgenteDeServicios>
    {
        public CCreacionMetodoParaAgenteDeServicios(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Crear método para agente de servicios";

        protected override void Accion(EventArgs evento)
        {
            var nombreCompleto = ElementoSeleccionado.InvariantName.Split('.');
            var esquema = nombreCompleto[0];
            var procedimiento = nombreCompleto[1];
            ConstructorDeCodigo.GenerarMetodoParaAgenteDeServicios(esquema, procedimiento, out MensajeError);
            CopiarAlPortapapeles();
        }
    }
}
