namespace UVNetGeneradorDeCodigo.Repositorio
{
    using System;
    using System.Collections.Generic;
    using UVNetGeneradorDeCodigo.Modelos;

    public interface IRepositorio : IDisposable
    {
        List<CColumnaTabla> ObtenerColumnas(string tabla, out string mensajeError);

        string ObtenerNombreBaseDatos(out string mensajeError);

        string ObtenerNombreUsuario(out string mensajeError);

        string ObtenerNombreEsquema(string objeto, out string mensajeError);

        List<string> ObtenerBasesDeDatos(out string mensajeError);

        List<string> ObtenerListaProcedimientos(out string mensajeError);

        List<string> ObtenerEntidades(string baseDeDatos, out string mensajeError);

        List<string> ObtenerOperaciones(string baseDeDatos, string esquema, string entidad, out string mensajeError);

        List<CParametroProceso> ObtenerParametrosProceso(string baseDeDatos, string esquema, string proceso, out string mensajeError);

        List<string> ObtenerEsquemas(string baseDeDatos, out string mensajeError);

        List<string> ObtenerDefinicionProcedimiento(string baseDeDatos, string esquema, string procedimiento, out string mensajeError);

        List<string> ObtenerProcedimientosDeUnProceso(string baseDeDatos, string esquema, string proceso, out string mensajeError);

        bool ExisteEsquema(string esquema, out string mensajeError);

        void RecompilarVistas(out string mensajeError);

        List<CParametroProcedimiento> ObtenerParametrosProcedimiento(string procedimiento, out string mensajeError);

        string ObtenerTipoDatoResultado(string tipoDatoSql);
    }
}
