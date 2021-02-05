namespace UVNetGeneradorDeCodigo.Generador.CSharp
{
    using System.Globalization;
    using System.Linq;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CCreacionClaseCSharpDeParametrosDeEntrada : CConstructorDeCodigoCSharp
    {
        public CCreacionClaseCSharpDeParametrosDeEntrada(IRepositorio repositorio) : base(repositorio)
        {
            informacionDeTexto = new CultureInfo("en-us", false).TextInfo;
        }

        private readonly TextInfo informacionDeTexto;

        public void GenerarClaseCSharpDeParametrosDeEntrada(string esquema, string procedimiento, out string mensajeError)
        {
            var parametros = Repositorio.ObtenerParametrosProcedimiento($"{esquema}.{procedimiento}", out mensajeError);
            ConstructorDeCodigo.AppendLine($"public class C{ConvertirATitleCase(procedimiento)} : CPBase");
            ConstructorDeCodigo.AppendLine("{");
            foreach (var parametro in parametros.Where(x => !x.EsParametroSalida && x.Nombre != "@I_USUARIO_AUT" && x.Nombre != "@I_CODIGO_SISTEMA"))
            {
                ConstructorDeCodigo.AppendLine($"    public {parametro.TipoDeDatoCSharp} {ConvertirATitleCase(parametro.Nombre.Substring(2))} {{ get; set; }}");
            }
            ConstructorDeCodigo.AppendLine("}");
        }

        private string ConvertirATitleCase(string cadena) => informacionDeTexto.ToTitleCase(cadena.ToLower()).Replace("_", string.Empty);
    }
}
