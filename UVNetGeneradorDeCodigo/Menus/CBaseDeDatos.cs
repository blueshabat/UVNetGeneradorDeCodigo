namespace UVNetGeneradorDeCodigo.Menus
{
    using Microsoft.VisualStudio.Shell;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Menus.Elementos.BaseDeDatos;
    using UVNetGeneradorDeCodigo.Menus.Elementos.Tabla;

    public class CBaseDeDatos : CMenu
    {
        public CBaseDeDatos(AsyncPackage paquete) : base(paquete)
        {
        }

        protected override ToolStripItem[] ElementosMenu => new ToolStripItem[] 
        { 
            new CCreacionProceso(ElementoSeleccionado), 
            new CModificacionProceso(ElementoSeleccionado),
            new ToolStripSeparator(),
            new CCreacionTablaParametrica(ElementoSeleccionado)
        };

        public override object Clone() => new CBaseDeDatos(paquete);
    }
}
