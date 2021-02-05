namespace UVNetGeneradorDeCodigo.Menus
{
    using Microsoft.VisualStudio.Shell;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Menus.Elementos;

    public class CVista : CMenu
    {
        public CVista(AsyncPackage paquete) : base(paquete)
        {
        }

        protected override ToolStripItem[] ElementosMenu => new CElementoMenu[] { 
        };

        public override object Clone() => new CVista(paquete);
    }
}
