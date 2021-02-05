namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Globalization;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CProcedimientoObtencion : CConstructorDeScriptsSQL
    {
        public CProcedimientoObtencion(IRepositorio repositorio) : base(repositorio)
        {
            informacionDeTexto = new CultureInfo("en-us", false).TextInfo;
        }

        private readonly TextInfo informacionDeTexto;

        public void GenerarProcedimientoObtencion(string esquema, string tabla, out string mensajeError)
        {
            var entidad = tabla.Substring(2);
            var columnas = Repositorio.ObtenerColumnas($"{esquema}.{ tabla}", out mensajeError).ExcluirAuditoria().ExcluirSecuencial();
            AdicionarCabecera($"Realiza la obtención de un registro de la tabla {esquema}.{tabla}");
            ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_AT_{entidad}_O");
            AdicionarParametrosEntradaUsuarioCodigoSistema();
            ConstructorDeCodigo.AppendLine($"    @I_SECUENCIAL INT,");
            AdicionarParametrosSalida("JSON", "NVARCHAR(MAX)");
            ConstructorDeCodigo.AppendLine($"    BEGIN");
            ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
            ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
            ConstructorDeCodigo.AppendLine($"            BEGIN");
            ConstructorDeCodigo.AppendLine($"                SET @O_RESULTADO_JSON =");
            ConstructorDeCodigo.AppendLine($"                (");
            ConstructorDeCodigo.AppendLine($"                    SELECT SECUENCIAL AS 'Secuencial',");
            ConstructorDeCodigo.AppendLine($"                           ADICIONADO_POR AS 'AdicionadoPor',");
            ConstructorDeCodigo.AppendLine($"                           FECHA_ADICION AS 'FechaAdicion',");
            ConstructorDeCodigo.AppendLine($"                           MODIFICADO_POR AS 'ModificadoPor',");
            ConstructorDeCodigo.Append($"                           FECHA_MODIFICACION AS 'FechaModificacion'");
            foreach (var columna in columnas)
            {
                ConstructorDeCodigo.AppendLine($",");
                ConstructorDeCodigo.Append($"                           {columna.Nombre} AS '{ConvertirATitleCase(columna.Nombre)}'");
            }
            ConstructorDeCodigo.AppendLine();
            ConstructorDeCodigo.AppendLine($"                           FROM {esquema}.{ tabla}");
            ConstructorDeCodigo.AppendLine($"                    WHERE SECUENCIAL = @I_SECUENCIAL FOR JSON PATH, INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER");
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
