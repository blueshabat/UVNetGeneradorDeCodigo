namespace UVNetGeneradorDeCodigo.Menus.Elementos.ProcedimientoAlmacenado
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;

    public class CCreacionClaseCSharpDeParametrosDeEntrada : CElementoMenu<Generador.CSharp.CCreacionClaseCSharpDeParametrosDeEntrada>
    {
        public CCreacionClaseCSharpDeParametrosDeEntrada(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Crear clase C# de parámetros de entrada";

        protected override void Accion(EventArgs evento)
        {
            var nombreCompleto = ElementoSeleccionado.InvariantName.Split('.');
            var esquema = nombreCompleto[0];
            var procedimiento = nombreCompleto[1];
            ConstructorDeCodigo.GenerarClaseCSharpDeParametrosDeEntrada(esquema, procedimiento, out MensajeError);
            CopiarAlPortapapeles();
        }
    }
}
