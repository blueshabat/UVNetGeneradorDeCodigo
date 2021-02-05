namespace UVNetGeneradorDeCodigo.API
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using Microsoft.VisualStudio.Shell;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.Menus.Elementos;
    using UVNetGeneradorDeCodigo.Properties;

    public abstract class CMenu : ToolsMenuItemBase, IWinformsMenuHandler
    {
        public CMenu(AsyncPackage paquete)
        {
            this.paquete = paquete;
        }

        protected abstract ToolStripItem[] ElementosMenu { get; }
        protected readonly AsyncPackage paquete;
        protected INodeInformation ElementoSeleccionado;
        private ToolStripMenuItem Menu;

        public ToolStripItem[] GetMenuItems()
        {
            ElementoSeleccionado = Parent;
            Menu = new ToolStripMenuItem("UnividaNet", Resources.logo_univida);
            Menu.DropDownItems.AddRange(ElementosMenu);
            return new ToolStripItem[] { Menu };
        }

        public abstract override object Clone();

        protected override void Invoke() { }
    }
}
