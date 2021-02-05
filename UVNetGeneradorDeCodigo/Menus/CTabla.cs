namespace UVNetGeneradorDeCodigo.Menus
{
    using Microsoft.VisualStudio.Shell;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Menus.Elementos.Tabla;

    public class CTabla : CMenu
    {
        public CTabla(AsyncPackage paquete) : base(paquete)
        {
        }

        protected override ToolStripItem[] ElementosMenu => new ToolStripItem[]
        {
            new CCreacionProcedimientos(ElementoSeleccionado),
            new ToolStripSeparator(),
            new CCreacionVista(ElementoSeleccionado),
            new CCreacionTransicionParametricas(ElementoSeleccionado),
            new ToolStripSeparator(),
            new CCreacionValidaciones(ElementoSeleccionado),
        };

        public override object Clone() => new CTabla(paquete);
    }
}
