namespace UVNetGeneradorDeCodigo.Menus
{
    using Microsoft.VisualStudio.Shell;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Menus.Elementos.Tabla;

    public class CCarpetaTablas : CMenu
    {
        public CCarpetaTablas(AsyncPackage paquete) : base(paquete)
        {
        }

        protected override ToolStripItem[] ElementosMenu => new ToolStripItem[]
        {
            new CCreacionTablaParametrica(ElementoSeleccionado)
        };

        public override object Clone() => new CCarpetaTablas(paquete);
    }
}
