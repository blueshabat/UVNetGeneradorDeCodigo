namespace UVNetGeneradorDeCodigo.Menus
{
    using Microsoft.VisualStudio.Shell;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Menus.Elementos.Vista;

    public class CCarpetaVistas : CMenu
    {
        public CCarpetaVistas(AsyncPackage paquete) : base(paquete)
        {
        }

        protected override ToolStripItem[] ElementosMenu => new ToolStripItem[]
        {
            new CRecompilarVistas(ElementoSeleccionado)
        };

        public override object Clone() => new CCarpetaVistas(paquete);
    }
}
