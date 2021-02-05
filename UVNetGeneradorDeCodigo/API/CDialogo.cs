namespace UVNetGeneradorDeCodigo.API
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.Editors;
    using Microsoft.VisualStudio.PlatformUI;
    using System;
    using System.Windows.Forms;
    using UVNetGeneradorDeCodigo.Repositorio;

    public abstract class CDialogo<T> : DialogWindow where T : CConstructorDeCodigo
    {
        protected T ConstructorDeCodigo;
        protected CRepositorio Repositorio;
        protected string MensajeError;

        public CDialogo(CRepositorio repositorio)
        {
            Repositorio = repositorio;
            ConstructorDeCodigo = Activator.CreateInstance(typeof(T), new object[] { repositorio }) as T;
        }

        protected void CrearNuevoDocumento()
        {
            if (string.IsNullOrEmpty(MensajeError))
            {
                ((SqlScriptEditorControl)ScriptFactory.Instance.CreateNewBlankScript(ScriptType.Sql)).EditorText = ConstructorDeCodigo.CodigoGenerado;
                Close();
            }
            else
            {
                MessageBox.Show(MensajeError);
            }
        }
    }
}
