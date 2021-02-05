namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Linq;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CProcedimientoAlta : CConstructorDeScriptsSQL
    {
        public CProcedimientoAlta(IRepositorio repositorio) : base(repositorio)
        {
        }

        public void GenerarProcedimientoAlta(string esquema, string tabla, out string mensajeError)
        {
            var entidad = tabla.Substring(2);
            var columnas = Repositorio.ObtenerColumnas($"{esquema}.{ tabla}", out mensajeError).ExcluirAuditoria().ExcluirSecuencial();
            AdicionarCabecera($"Realiza el alta de un registro en la tabla {esquema}.{tabla}");
            ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_AT_{entidad}_A");
            AdicionarParametrosEntradaUsuarioCodigoSistema();
            foreach (var columna in columnas)
            {
                ConstructorDeCodigo.AppendLine($"    @I_{columna.Nombre} {columna.TipoDato},");
            }
            AdicionarParametrosSalida("INT", "INT");
            ConstructorDeCodigo.AppendLine($"    BEGIN");
            ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
            ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
            ConstructorDeCodigo.AppendLine($"            BEGIN");
            ConstructorDeCodigo.AppendLine($"                DECLARE @V_FECHA_ADICION DATETIME2= GETDATE();");
            ConstructorDeCodigo.AppendLine($"                INSERT INTO {esquema}.{tabla}");
            ConstructorDeCodigo.AppendLine($"                (ADICIONADO_POR,");
            ConstructorDeCodigo.Append($"                 FECHA_ADICION");
            foreach (var columna in columnas)
            {
                ConstructorDeCodigo.AppendLine($",");
                ConstructorDeCodigo.Append($"                 {columna.Nombre}");
            }
            ConstructorDeCodigo.AppendLine($" )");
            ConstructorDeCodigo.AppendLine($"                VALUES");
            ConstructorDeCodigo.AppendLine($"                (@I_USUARIO_AUT,");
            ConstructorDeCodigo.Append($"                 @V_FECHA_ADICION");
            foreach (var columna in columnas)
            {
                ConstructorDeCodigo.AppendLine($",");
                ConstructorDeCodigo.Append($"                 @I_{columna.Nombre}");
            }
            ConstructorDeCodigo.AppendLine($" );");
            ConstructorDeCodigo.AppendLine($"                IF(@@ROWCOUNT > 0)");
            ConstructorDeCodigo.AppendLine($"                    BEGIN");
            ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_INT = SCOPE_IDENTITY();");
            ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 1;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'La información fue registrada correctamente.'");
            ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_MOTIVO VARCHAR(800)= 'Alta de un registro en la tabla {esquema}.{tabla}';");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_INT INT= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_BIG_INT BIGINT= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_DECIMAL DECIMAL(16, 4)= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_DATETIME DATETIME2= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_VARCHAR_40 VARCHAR(40)= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_VARCHAR_400 VARCHAR(400)= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_VARCHAR_4000 VARCHAR(4000)= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_NOMBRE_BD VARCHAR(100)= DB_NAME();");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_NOMBRE_SP VARCHAR(100)= OBJECT_NAME(@@PROCID);");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_JSON_AUDITORIA_DATA VARCHAR(MAX)=");
            ConstructorDeCodigo.AppendLine($"                        (");
            ConstructorDeCodigo.AppendLine($"                            SELECT @O_RESULTADO_INT AS SECUENCIAL,");
            ConstructorDeCodigo.AppendLine($"                                   @I_USUARIO_AUT AS ADICIONADO_POR,");
            ConstructorDeCodigo.AppendLine($"                                   @V_FECHA_ADICION AS FECHA_ADICION,");
            ConstructorDeCodigo.Append($"                                   @I_CODIGO_SISTEMA AS I_CODIGO_SISTEMA");
            foreach (var columna in columnas)
            {
                ConstructorDeCodigo.AppendLine($",");
                ConstructorDeCodigo.Append($"                                   @I_{columna.Nombre} AS I_{columna.Nombre}");
            }
            ConstructorDeCodigo.AppendLine($" FOR JSON PATH, INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER");
            ConstructorDeCodigo.AppendLine($"                        );");
            ConstructorDeCodigo.AppendLine($"                        EXEC BD_TRANS_AUDITORIA_DATA.AUDITORIA.P_B_TABLAS_VARIACION_A ");
            ConstructorDeCodigo.AppendLine($"                             @I_USUARIO_AUT, ");
            ConstructorDeCodigo.AppendLine($"                             @I_CODIGO_SISTEMA, ");
            ConstructorDeCodigo.AppendLine($"                             'INSERT', ");
            ConstructorDeCodigo.AppendLine($"                             @V_NOMBRE_SP, ");
            ConstructorDeCodigo.AppendLine($"                             @V_NOMBRE_BD, ");
            ConstructorDeCodigo.AppendLine($"                             '{esquema}', ");
            ConstructorDeCodigo.AppendLine($"                             '{tabla}', ");
            ConstructorDeCodigo.AppendLine($"                             @O_RESULTADO_INT, ");
            ConstructorDeCodigo.AppendLine($"                             @V_JSON_AUDITORIA_DATA, ");
            ConstructorDeCodigo.AppendLine($"                             @V_MOTIVO, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_INT, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_BIG_INT, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_DECIMAL, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_DATETIME, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_VARCHAR_40, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_VARCHAR_400, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_VARCHAR_4000;");
            ConstructorDeCodigo.AppendLine($"                END;");
            ConstructorDeCodigo.AppendLine($"                    ELSE");
            ConstructorDeCodigo.AppendLine($"                    BEGIN");
            ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_INT = 0;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 0;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'La información no pudo ser registrada.';");
            ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
            ConstructorDeCodigo.AppendLine($"                END;");
            ConstructorDeCodigo.AppendLine($"        END;");
            ConstructorDeCodigo.AppendLine($"    END;");
            ConstructorDeCodigo.AppendLine($"GO");
        }
    }
}
