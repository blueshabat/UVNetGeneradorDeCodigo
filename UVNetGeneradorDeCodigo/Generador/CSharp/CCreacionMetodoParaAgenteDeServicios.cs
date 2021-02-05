namespace UVNetGeneradorDeCodigo.Generador.CSharp
{
    using System.Globalization;
    using System.Linq;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CCreacionMetodoParaAgenteDeServicios : CConstructorDeCodigoCSharp
    {
        public CCreacionMetodoParaAgenteDeServicios(IRepositorio repositorio) : base(repositorio)
        {
            informacionDeTexto = new CultureInfo("en-us", false).TextInfo;
        }

        private readonly TextInfo informacionDeTexto;

        public void GenerarMetodoParaAgenteDeServicios(string esquema, string nombreProcedimiento, out string mensajeError)
        {
            var parametros = Repositorio.ObtenerParametrosProcedimiento($"{esquema}.{nombreProcedimiento}", out mensajeError);
            var procedimientoTitleCase = ConvertirATitleCase(nombreProcedimiento).Substring(1);
            ConstructorDeCodigo.AppendLine($"public static CResultado{Repositorio.ObtenerTipoDatoResultado(parametros.Find(x => x.Nombre.Contains("@O_RESULTADO")).Nombre)} {procedimientoTitleCase}(CP{procedimientoTitleCase} oP{procedimientoTitleCase})");
            ConstructorDeCodigo.AppendLine($"{{");
            ConstructorDeCodigo.AppendLine($"    CEParametros oEParametros = new CEParametros(\"{Repositorio.ObtenerNombreBaseDatos(out mensajeError)}.{esquema}.{nombreProcedimiento}\");");
            ConstructorDeCodigo.AppendLine($"    int c = 0;");
            foreach (var parametro in parametros.Where(x => !x.EsParametroSalida))
            {
                var parametroTitleCase = ConvertirATitleCase(parametro.Nombre.Replace("@I_", string.Empty));
                ConstructorDeCodigo.AppendLine($"    oEParametros.lParametro{parametro.TipoDeDatoClaseCSharp}.Add(new CParametro{parametro.TipoDeDatoClaseCSharp} {{ PosicionParametro = ++c, NombreParametro = \"{parametro.Nombre.Replace("@", string.Empty)}\", ValorParametro = oP{procedimientoTitleCase}.{parametroTitleCase}}});");
            }
            ConstructorDeCodigo.AppendLine($"    CEParametros[] lEParametros = {{ oEParametros }};");
            ConstructorDeCodigo.AppendLine($"    ServiciosWebGenericasManagerClient oSW = new ServiciosWebGenericasManagerClient();");
            ConstructorDeCodigo.AppendLine($"    return oSW.EjecutarSP{Repositorio.ObtenerTipoDatoResultado(parametros.Find(x => x.Nombre.Contains("@O_RESULTADO")).Nombre)}(lEParametros);");
            ConstructorDeCodigo.AppendLine($"}}");
        }

        private string ConvertirATitleCase(string cadena) => informacionDeTexto.ToTitleCase(cadena.ToLower()).Replace("_", string.Empty);
    }
}
