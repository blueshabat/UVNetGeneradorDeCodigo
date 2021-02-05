namespace UVNetGeneradorDeCodigo.Menus.Elementos
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.Editors;
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public abstract class CElementoMenu : CElementoMenu<CConstructorDeScriptsSQL>
    {
        public CElementoMenu(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }
    }

    public abstract class CElementoMenu<T> : ToolStripMenuItem where T : CConstructorDeCodigo
    {
        public CElementoMenu(INodeInformation elementoSeleccionado)
        {
            ElementoSeleccionado = elementoSeleccionado;
            Text = Titulo;
        }

        protected abstract string Titulo { get; }
        protected INodeInformation ElementoSeleccionado;
        protected CRepositorio Repositorio;
        protected string MensajeError;
        protected T ConstructorDeCodigo;
        
        protected override void OnClick(EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            using (Repositorio = new CRepositorio(ElementoParaRepositorio))
            {
                ConstructorDeCodigo = Activator.CreateInstance(typeof(T), new object[] { Repositorio }) as T;
                Accion(e);
            }
        }

        protected abstract void Accion(EventArgs evento);

        protected virtual INodeInformation ElementoParaRepositorio { get => ElementoSeleccionado?.Parent; }

        protected void CrearNuevoDocumento()
        {
            if (string.IsNullOrEmpty(MensajeError))
            {
                ((SqlScriptEditorControl)ScriptFactory.Instance.CreateNewBlankScript(ScriptType.Sql)).EditorText = ConstructorDeCodigo.CodigoGenerado;
            }
            else
            {
                MessageBox.Show(MensajeError);
            }
        }

        protected void CopiarAlPortapapeles(string mensajeUnaVezCopiado = "El texto ha sido copiado correctamente.")
        {
            Clipboard.SetText(ConstructorDeCodigo.CodigoGenerado);
            MessageBox.Show(mensajeUnaVezCopiado);
        }
    }
}
