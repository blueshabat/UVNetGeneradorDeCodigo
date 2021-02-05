namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Globalization;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CProcedimientoListado : CConstructorDeScriptsSQL
    {
        public CProcedimientoListado(IRepositorio repositorio) : base(repositorio)
        {
            informacionDeTexto = new CultureInfo("en-us", false).TextInfo;
        }

        private readonly TextInfo informacionDeTexto;

        public void GenerarProcedimientoListado(string esquema, string tabla, out string mensajeError)
        {
            var entidad = tabla.Substring(2);
            var columnas = Repositorio.ObtenerColumnas($"{esquema}.{tabla}", out mensajeError);
            AdicionarCabecera($"Realiza la obtención del listado de los registros de la tabla {esquema}.{tabla}");
            ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_AT_{entidad}_L");
            AdicionarParametrosEntradaUsuarioCodigoSistema();
            AdicionarParametrosSalida("JSON", "NVARCHAR(MAX)");
            ConstructorDeCodigo.AppendLine($"    BEGIN");
            ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
            ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
            ConstructorDeCodigo.AppendLine($"            BEGIN");
            ConstructorDeCodigo.AppendLine($"                SET @O_RESULTADO_JSON =");
            ConstructorDeCodigo.AppendLine($"                (");
            ConstructorDeCodigo.AppendLine($"                    SELECT ");
            foreach (var columna in columnas)
            {
                ConstructorDeCodigo.AppendLine($"                           {columna.Nombre} AS '{ConvertirATitleCase(columna.Nombre)}',");
            }
            ConstructorDeCodigo.Length -= 3;
            ConstructorDeCodigo.AppendLine();
            ConstructorDeCodigo.AppendLine($"                    FROM {esquema}.{tabla} FOR JSON PATH, INCLUDE_NULL_VALUES");
            ConstructorDeCodigo.AppendLine($"                );");
            ConstructorDeCodigo.AppendLine($"                IF(@O_RESULTADO_JSON IS NOT NULL)");
            ConstructorDeCodigo.AppendLine($"                    BEGIN");
            ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 1;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'La información fue obtenida correctamente.';");
            ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
            ConstructorDeCodigo.AppendLine($"                END;");
            ConstructorDeCodigo.AppendLine($"                    ELSE");
            ConstructorDeCodigo.AppendLine($"                    BEGIN;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_JSON = '';");
            ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 0;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'La información no pudo ser obtenida.';");
            ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
            ConstructorDeCodigo.AppendLine($"                END;");
            ConstructorDeCodigo.AppendLine($"        END");
            ConstructorDeCodigo.AppendLine($"    END");
            ConstructorDeCodigo.AppendLine($"GO");
        }

        private string ConvertirATitleCase(string cadena) => informacionDeTexto.ToTitleCase(cadena.ToLower()).Replace("_", string.Empty);
    }
}
