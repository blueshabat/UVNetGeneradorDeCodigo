namespace UVNetGeneradorDeCodigo.Menus
{
    using Microsoft.VisualStudio.Shell;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Menus.Elementos.ProcedimientoAlmacenado;

    public class CProcedimiento : CMenu
    {
        public CProcedimiento(AsyncPackage paquete) : base(paquete)
        {
        }

        protected override ToolStripItem[] ElementosMenu => new ToolStripItem[]
        {
            new CCreacionClaseCSharpDeParametrosDeEntrada(ElementoSeleccionado),
            new CCreacionMetodoParaAgenteDeServicios(ElementoSeleccionado),
            new CCopiaEjecucionDeProcedimiento(ElementoSeleccionado),
            new CVisorDeDependencias(ElementoSeleccionado)
        };

        public override object Clone() => new CProcedimiento(paquete);
    }
}
