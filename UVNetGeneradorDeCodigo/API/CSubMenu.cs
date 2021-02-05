namespace UVNetGeneradorDeCodigo.Menus
{
    using System.Windows.Forms;

    public abstract class CSubMenu : ToolStripMenuItem
    {
        public CSubMenu() : base()
        {
            Text = Titulo;
        }

        protected abstract string Titulo { get; }
    }
}
