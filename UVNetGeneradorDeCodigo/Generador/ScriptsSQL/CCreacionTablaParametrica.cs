namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Collections.Generic;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Modelos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CCreacionTablaParametrica : CConstructorDeScriptsSQL
    {
        public CCreacionTablaParametrica(IRepositorio repositorio) : base(repositorio)
        {
        }

        public void GenerarCreacionTablaParametrica(string nombre, out string mensajeError)
        {
            mensajeError = string.Empty;
            AdicionarCreacionDeEsquema("PARAMETRICAS");
            AdicionarCreacionDeTabla("PARAMETRICAS", $"T_PAR_{nombre}", new List<CColumnaTabla>
            {
                new CColumnaTabla("DESCRIPCION", "NVARCHAR(100)"),
                new CColumnaTabla("ABREVIACION", "NVARCHAR(5)"),
                new CColumnaTabla("VISIBLE","BIT"),
            });
        }
    }
}
