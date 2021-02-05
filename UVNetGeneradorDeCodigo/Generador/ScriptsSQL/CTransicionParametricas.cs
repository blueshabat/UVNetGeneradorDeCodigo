namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Collections.Generic;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Modelos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CTransicionParametricas : CConstructorDeScriptsSQL
    {
        public CTransicionParametricas(IRepositorio repositorio) : base(repositorio)
        {
        }

        public void GenerarProcesoDeTransicion(string esquema, string tabla, string proceso, string descripcion, out string mensajeError)
        {
            var prefijoEsquema = esquema.Substring(0, 3);
            var entidad = tabla.Substring(2);
            var nombreCompuesto = $"{prefijoEsquema}_{entidad}_{proceso}";
            var listadoProcedimientos = Repositorio.ObtenerListaProcedimientos(out mensajeError);
            AdicionarCreacionDeEsquema("PARAMETRICAS");
            AdicionarCreacionDeTabla("PARAMETRICAS", $"T_PAR_{nombreCompuesto}", new List<CColumnaTabla>
            {
                new CColumnaTabla("DESCRIPCION", "NVARCHAR(100)"),
                new CColumnaTabla("ABREVIACION", "NVARCHAR(5)"),
                new CColumnaTabla("VISIBLE","BIT"),
            });
            AdicionarCreacionDeTabla(esquema, $"T_{entidad}_HIST_{proceso}", new List<CColumnaTabla>
            {
                new CColumnaTabla($"T_{entidad}_FK", "INT", admiteValoresNulos: false, esLlaveForanea: true, referencia: new CReferencia(esquema, $"T_{entidad}")),
                new CColumnaTabla($"T_PAR_{nombreCompuesto}_FK", "INT", admiteValoresNulos: false, esLlaveForanea: true, referencia: new CReferencia("PARAMETRICAS",$"T_PAR_{nombreCompuesto}")),
                new CColumnaTabla("T_USUARIO_DATOS_FK", "INT", admiteValoresNulos: false),
                new CColumnaTabla("MOTIVO", "NVARCHAR(500)"),
                new CColumnaTabla($"DATOS_{entidad}_JSON", "NVARCHAR(MAX)")
            }, true);
            ConstructorDeCodigo.AppendLine($"CREATE NONCLUSTERED INDEX T_{entidad}_HIST_{proceso}_NUI_T_{entidad}_FK ON {esquema}.T_{entidad}_HIST_{proceso}(T_{entidad}_FK ASC) WITH(FILLFACTOR = 80, ONLINE=ON);");
            ConstructorDeCodigo.AppendLine($"GO");
            AdicionarCreacionDeEsquema("PARAM_TRANSICION");
            AdicionarCreacionDeTabla("PARAM_TRANSICION", $"T_PAR_{nombreCompuesto}_TP", new List<CColumnaTabla>
            {
                new CColumnaTabla($"T_PAR_{nombreCompuesto}_FK_INICIAL", "INT", esLlaveForanea:true, referencia: new CReferencia("PARAMETRICAS", $"T_PAR_{nombreCompuesto}", "INICIAL")),
                new CColumnaTabla($"T_PAR_{nombreCompuesto}_FK_FINAL", "INT", admiteValoresNulos:false, esLlaveForanea:true, referencia: new CReferencia("PARAMETRICAS", $"T_PAR_{nombreCompuesto}", "FINAL")),
                new CColumnaTabla("HABILITADO", "BIT", admiteValoresNulos:false, tieneValorPorDefecto:true, valorPorDefecto:"1")
            });
            if (!listadoProcedimientos.Contains($"{esquema}.P_B_AT_{entidad}_V_E_SECUENCIAL"))
            {
                AdicionarCabecera($"Valida la existencia de parámetro SECUENCIAL en {esquema}.T_{entidad}.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_AT_{entidad}_V_E_SECUENCIAL");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                ConstructorDeCodigo.AppendLine($"    @I_SECUENCIAL INT,");
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                ConstructorDeCodigo.AppendLine($"                IF(EXISTS");
                ConstructorDeCodigo.AppendLine($"                (");
                ConstructorDeCodigo.AppendLine($"                    SELECT 1");
                ConstructorDeCodigo.AppendLine($"                    FROM {esquema}.T_{entidad}");
                ConstructorDeCodigo.AppendLine($"                    WHERE SECUENCIAL = @I_SECUENCIAL");
                ConstructorDeCodigo.AppendLine($"                ))");
                ConstructorDeCodigo.AppendLine($"                    BEGIN");
                ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_BIT = 1;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 1;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = CONCAT('Si existe {entidad.Replace('_', ' ')} con el código número ', @I_SECUENCIAL, '.');");
                ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                ConstructorDeCodigo.AppendLine($"                END;");
                ConstructorDeCodigo.AppendLine($"                    ELSE");
                ConstructorDeCodigo.AppendLine($"                    BEGIN");
                ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_BIT = 0;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 0;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = CONCAT('No existe {entidad.Replace('_', ' ')} con el código número ', @I_SECUENCIAL ,'.');");
                ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                ConstructorDeCodigo.AppendLine($"                END;");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
            }
            else
            {
                AdicionarError($"El procedimiento {esquema}.P_B_AT_{entidad}_V_E_SECUENCIAL no pudo ser generado porque ya existe en la base de datos. {mensajeError}");
            }
            if (!listadoProcedimientos.Contains($"PARAMETRICAS.P_B_AT_PAR_{nombreCompuesto}_V_E_SECUENCIAL"))
            {
                AdicionarCabecera($"Valida la existencia del parámetro SECUENCIAL en PARAMETRICAS.T_PAR_{nombreCompuesto}.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE PARAMETRICAS.P_B_AT_PAR_{nombreCompuesto}_V_E_SECUENCIAL");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                ConstructorDeCodigo.AppendLine($"    @I_SECUENCIAL INT,");
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                ConstructorDeCodigo.AppendLine($"                IF(EXISTS");
                ConstructorDeCodigo.AppendLine($"                (");
                ConstructorDeCodigo.AppendLine($"                    SELECT 1");
                ConstructorDeCodigo.AppendLine($"                    FROM PARAMETRICAS.T_PAR_{nombreCompuesto}");
                ConstructorDeCodigo.AppendLine($"                    WHERE SECUENCIAL = @I_SECUENCIAL");
                ConstructorDeCodigo.AppendLine($"                ))");
                ConstructorDeCodigo.AppendLine($"                    BEGIN");
                ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_BIT = 1;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 1;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = CONCAT('Si existe {descripcion} con el código número ', @I_SECUENCIAL ,'.')");
                ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                ConstructorDeCodigo.AppendLine($"                END;");
                ConstructorDeCodigo.AppendLine($"                    ELSE");
                ConstructorDeCodigo.AppendLine($"                    BEGIN");
                ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_BIT = 0;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 0;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = CONCAT('No existe {descripcion} con el código número ', @I_SECUENCIAL ,'.')");
                ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                ConstructorDeCodigo.AppendLine($"                END;");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
            }
            else
            {
                AdicionarError($"El procedimiento PARAMETRICAS.P_B_AT_PAR_{nombreCompuesto}_V_E_SECUENCIAL no pudo ser generado porque ya existe en la base de datos. {mensajeError}");
            }
            if (!listadoProcedimientos.Contains($"PARAM_TRANSICION.P_B_AT_PAR_{prefijoEsquema}_{entidad}_TP_V_E_T_PAR_{nombreCompuesto}_FK_INICIAL_T_PAR_{nombreCompuesto}_FK_FINAL"))
            {
                AdicionarCabecera($"Valida la existencia de los parámetros [T_PAR_{nombreCompuesto}_FK_INICIAL, T_PAR_{nombreCompuesto}_FK_FINAL] en PARAM_TRANSICION.T_PAR_{prefijoEsquema}_{entidad}_TP.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE PARAM_TRANSICION.P_B_AT_PAR_{prefijoEsquema}_{entidad}_TP_V_E_T_PAR_{nombreCompuesto}_FK_INICIAL_T_PAR_{nombreCompuesto}_FK_FINAL");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                ConstructorDeCodigo.AppendLine($"    @I_T_PAR_{nombreCompuesto}_FK_INICIAL INT,");
                ConstructorDeCodigo.AppendLine($"    @I_T_PAR_{nombreCompuesto}_FK_FINAL INT,");
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                ConstructorDeCodigo.AppendLine($"                IF(EXISTS");
                ConstructorDeCodigo.AppendLine($"                (");
                ConstructorDeCodigo.AppendLine($"                    SELECT 1");
                ConstructorDeCodigo.AppendLine($"                    FROM PARAM_TRANSICION.T_PAR_{nombreCompuesto}_TP");
                ConstructorDeCodigo.AppendLine($"                    WHERE T_PAR_{nombreCompuesto}_FK_FINAL = @I_T_PAR_{nombreCompuesto}_FK_FINAL");
                ConstructorDeCodigo.AppendLine($"                          AND(T_PAR_{nombreCompuesto}_FK_INICIAL = @I_T_PAR_{nombreCompuesto}_FK_INICIAL");
                ConstructorDeCodigo.AppendLine($"                              OR(T_PAR_{nombreCompuesto}_FK_INICIAL IS NULL");
                ConstructorDeCodigo.AppendLine($"                                 AND @I_T_PAR_{nombreCompuesto}_FK_INICIAL IS NULL))");
                ConstructorDeCodigo.AppendLine($"                ))");
                ConstructorDeCodigo.AppendLine($"                    BEGIN");
                ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_BIT = 1;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 1;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'La transición es válida.'");
                ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                ConstructorDeCodigo.AppendLine($"                END;");
                ConstructorDeCodigo.AppendLine($"                    ELSE");
                ConstructorDeCodigo.AppendLine($"                    BEGIN");
                ConstructorDeCodigo.AppendLine($"                        DECLARE @V_PAR_{nombreCompuesto}_DESCRIPCION_INICIAL NVARCHAR(100) = (SELECT CONCAT(' de ', DESCRIPCION) FROM PARAMETRICAS.T_PAR_{nombreCompuesto} WHERE SECUENCIAL = @I_T_PAR_{nombreCompuesto}_FK_INICIAL);");
                ConstructorDeCodigo.AppendLine($"                        DECLARE @V_PAR_{nombreCompuesto}_DESCRIPCION_FINAL NVARCHAR(100) = (SELECT DESCRIPCION FROM PARAMETRICAS.T_PAR_{nombreCompuesto} WHERE SECUENCIAL = @I_T_PAR_{nombreCompuesto}_FK_FINAL);");
                ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_BIT = 0;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 0;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = CONCAT('NO es posible realizar una transición ', @V_PAR_{nombreCompuesto}_DESCRIPCION_INICIAL , ' a ', @V_PAR_{nombreCompuesto}_DESCRIPCION_FINAL ,' en {entidad.Replace('_', ' ')}.')");
                ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                ConstructorDeCodigo.AppendLine($"                END;");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
            }
            else
            {
                AdicionarError($"El procedimiento PARAM_TRANSICION.P_B_AT_PAR_{prefijoEsquema}_{entidad}_TP_V_E_T_PAR_{nombreCompuesto}_FK_INICIAL_T_PAR_{nombreCompuesto}_FK_FINAL no pudo ser generado porque ya existe en la base de datos. {mensajeError}");
            }
            if (!listadoProcedimientos.Contains($"{esquema}.P_B_TP_{entidad}_{proceso}_VALIDA_TRANSICION"))
            {
                AdicionarCabecera("Verifica si es posible realizar la transición de paramétrica.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_TP_{entidad}_{proceso}_VALIDA_TRANSICION");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                ConstructorDeCodigo.AppendLine($"@I_T_{entidad}_FK INT,");
                ConstructorDeCodigo.AppendLine($"@I_T_PAR_{nombreCompuesto}_FK_FINAL INT,");
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                ConstructorDeCodigo.AppendLine($"                DECLARE @V_T_PAR_{nombreCompuesto}_FK_INICIAL INT;");
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_AT_{entidad}_V_E_SECUENCIAL", new List<string> { $"@I_T_{entidad}_FK" }, "BIT");
                AdicionarEjecucionProcedimiento($"PARAMETRICAS.P_B_AT_PAR_{nombreCompuesto}_V_E_SECUENCIAL", new List<string> { $"@I_T_PAR_{nombreCompuesto}_FK_FINAL" }, "BIT");
                ConstructorDeCodigo.AppendLine($"                IF(@O_EXITO = 1)");
                ConstructorDeCodigo.AppendLine($"                BEGIN");
                ConstructorDeCodigo.AppendLine($"                    SET @V_T_PAR_{nombreCompuesto}_FK_INICIAL=");
                ConstructorDeCodigo.AppendLine($"                    (");
                ConstructorDeCodigo.AppendLine($"                        SELECT TOP 1 T_PAR_{nombreCompuesto}_FK");
                ConstructorDeCodigo.AppendLine($"                        FROM {esquema}.T_{entidad}_HIST_{proceso}");
                ConstructorDeCodigo.AppendLine($"                        WHERE T_{entidad}_FK = @I_T_{entidad}_FK");
                ConstructorDeCodigo.AppendLine($"                        ORDER BY SECUENCIAL DESC");
                ConstructorDeCodigo.AppendLine($"                    );");
                ConstructorDeCodigo.AppendLine($"                END;");
                AdicionarEjecucionProcedimiento($"PARAM_TRANSICION.P_B_AT_PAR_{prefijoEsquema}_{entidad}_TP_V_E_T_PAR_{nombreCompuesto}_FK_INICIAL_T_PAR_{nombreCompuesto}_FK_FINAL", new List<string> { $"@V_T_PAR_{nombreCompuesto}_FK_INICIAL", $"@I_T_PAR_{nombreCompuesto}_FK_FINAL" }, "BIT");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
            }
            else
            {
                AdicionarError($"El procedimiento {esquema}.P_B_TP_{entidad}_{proceso}_VALIDA_TRANSICION no pudo ser generado porque ya existe en la base de datos. {mensajeError}");
            }
            if (!listadoProcedimientos.Contains($"{esquema}.P_B_AT_{entidad}_HIST_{proceso}_A"))
            {
                AdicionarCabecera($"Realiza el alta en la tabla {esquema}.T_{entidad}_HIST_{proceso}");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_AT_{entidad}_HIST_{proceso}_A");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                ConstructorDeCodigo.AppendLine($"    @I_T_{entidad}_FK INT,");
                ConstructorDeCodigo.AppendLine($"    @I_T_USUARIO_DATOS_FK INT,");
                ConstructorDeCodigo.AppendLine($"    @I_T_PAR_{nombreCompuesto}_FK_FINAL INT,");
                ConstructorDeCodigo.AppendLine($"    @I_MOTIVO NVARCHAR(500),");
                AdicionarParametrosSalida("INT", "INT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                ConstructorDeCodigo.AppendLine($"                DECLARE @V_FECHA_ADICION DATETIME2= GETDATE();");
                ConstructorDeCodigo.AppendLine($"                DECLARE @V_DATOS_{entidad}_JSON NVARCHAR(MAX)= ");
                ConstructorDeCodigo.AppendLine($"                (");
                ConstructorDeCodigo.AppendLine($"                    SELECT *");
                ConstructorDeCodigo.AppendLine($"                    FROM {esquema}.V_{entidad}");
                ConstructorDeCodigo.AppendLine($"                    WHERE {entidad.Substring(0, 1)}{entidad.Substring(1, 2).ToLower()}Secuencial = @I_T_{entidad}_FK FOR JSON PATH, INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER");
                ConstructorDeCodigo.AppendLine($"                ); ");
                ConstructorDeCodigo.AppendLine($"                INSERT INTO {esquema}.T_{entidad}_HIST_{proceso}");
                ConstructorDeCodigo.AppendLine($"                (ADICIONADO_POR, ");
                ConstructorDeCodigo.AppendLine($"                 FECHA_ADICION, ");
                ConstructorDeCodigo.AppendLine($"                 T_{entidad}_FK, ");
                ConstructorDeCodigo.AppendLine($"                 DATOS_{entidad}_JSON, ");
                ConstructorDeCodigo.AppendLine($"                 T_USUARIO_DATOS_FK, ");
                ConstructorDeCodigo.AppendLine($"                 T_PAR_{nombreCompuesto}_FK,");
                ConstructorDeCodigo.AppendLine($"                 MOTIVO");
                ConstructorDeCodigo.AppendLine($"                )");
                ConstructorDeCodigo.AppendLine($"                VALUES");
                ConstructorDeCodigo.AppendLine($"                (@I_USUARIO_AUT, ");
                ConstructorDeCodigo.AppendLine($"                 @V_FECHA_ADICION, ");
                ConstructorDeCodigo.AppendLine($"                 @I_T_{entidad}_FK, ");
                ConstructorDeCodigo.AppendLine($"                 @V_DATOS_{entidad}_JSON, ");
                ConstructorDeCodigo.AppendLine($"                 @I_T_USUARIO_DATOS_FK, ");
                ConstructorDeCodigo.AppendLine($"                 @I_T_PAR_{nombreCompuesto}_FK_FINAL,");
                ConstructorDeCodigo.AppendLine($"                 @I_MOTIVO");
                ConstructorDeCodigo.AppendLine($"                );");
                ConstructorDeCodigo.AppendLine($"                IF(@@ROWCOUNT > 0)");
                ConstructorDeCodigo.AppendLine($"                    BEGIN");
                ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_INT = SCOPE_IDENTITY();");
                ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 1;");
                ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'El registro de {descripcion} se realizó correctamente para {entidad.Replace('_', ' ')}.';");
                ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                ConstructorDeCodigo.AppendLine($"                        DECLARE @V_MOTIVO VARCHAR(800)= '';");
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
                ConstructorDeCodigo.AppendLine($"                            SELECT @O_RESULTADO_INT AS SECUENCIAL, ");
                ConstructorDeCodigo.AppendLine($"                                   @I_USUARIO_AUT AS ADICIONADO_POR, ");
                ConstructorDeCodigo.AppendLine($"                                   @V_FECHA_ADICION AS FECHA_ADICION, ");
                ConstructorDeCodigo.AppendLine($"                                   @I_CODIGO_SISTEMA AS I_CODIGO_SISTEMA, ");
                ConstructorDeCodigo.AppendLine($"                                   @I_T_{entidad}_FK AS I_T_{entidad}_FK, ");
                ConstructorDeCodigo.AppendLine($"                                   @I_T_USUARIO_DATOS_FK AS I_T_USUARIO_DATOS_FK, ");
                ConstructorDeCodigo.AppendLine($"                                   @I_MOTIVO AS I_MOTIVO_FK, ");
                ConstructorDeCodigo.AppendLine($"                                   @I_T_PAR_{nombreCompuesto}_FK_FINAL AS I_T_PAR_{nombreCompuesto}_FK_FINAL FOR JSON PATH, INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER");
                ConstructorDeCodigo.AppendLine($"                        );");
                ConstructorDeCodigo.AppendLine($"                        EXEC BD_TRANS_AUDITORIA_DATA.AUDITORIA.P_B_TABLAS_VARIACION_A ");
                ConstructorDeCodigo.AppendLine($"                             @I_USUARIO_AUT, ");
                ConstructorDeCodigo.AppendLine($"                             @I_CODIGO_SISTEMA, ");
                ConstructorDeCodigo.AppendLine($"                             'INSERT', ");
                ConstructorDeCodigo.AppendLine($"                             @V_NOMBRE_SP, ");
                ConstructorDeCodigo.AppendLine($"                             @V_NOMBRE_BD, ");
                ConstructorDeCodigo.AppendLine($"                             '{esquema}', ");
                ConstructorDeCodigo.AppendLine($"                             'T_{entidad}_HIST_{proceso}', ");
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
                ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'No se pudo realizar el registro de {descripcion} para {entidad.Replace('_', ' ')}.';");
                ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                ConstructorDeCodigo.AppendLine($"                END;");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
            }
            else
            {
                AdicionarError($"El procedimiento {esquema}.P_B_AT_{entidad}_HIST_{proceso}_A no pudo ser generado porque ya existe en la base de datos. {mensajeError}");
            }
            if (!listadoProcedimientos.Contains($"{esquema}.P_B_TP_{entidad}_{proceso}_REGISTRA_TRANSICION"))
            {
                AdicionarCabecera("Ejecuta la transición de paramétrica.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_TP_{entidad}_{proceso}_REGISTRA_TRANSICION");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                ConstructorDeCodigo.AppendLine($"    @I_T_{entidad}_FK INT,");
                ConstructorDeCodigo.AppendLine($"    @I_T_PAR_{nombreCompuesto}_FK_FINAL INT,");
                ConstructorDeCodigo.AppendLine($"    @I_MOTIVO NVARCHAR(500),");
                AdicionarParametrosSalida("INT", "INT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                ConstructorDeCodigo.AppendLine($"                DECLARE @V_RESULTADO_BIT BIT;");
                ConstructorDeCodigo.AppendLine($"                DECLARE @V_SECUENCIAL_USUARIO INT;");
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_TP_{entidad}_{proceso}_VALIDA_TRANSICION", new List<string> { $"@I_T_{entidad}_FK", $"@I_T_PAR_{nombreCompuesto}_FK_FINAL" }, "BIT", true);
                AdicionarEjecucionProcedimiento($"BD_ADMIN.RRHH_AFILIACION.P_B_EX_USUARIO_DATOS_OBTENER_SECUENCIAL", new List<string> { $"@I_USUARIO_AUT" }, null, true, "SECUENCIAL_USUARIO");
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_AT_{entidad}_HIST_{proceso}_A", new List<string> { $"@I_T_{entidad}_FK", "@V_SECUENCIAL_USUARIO", $"@I_T_PAR_{nombreCompuesto}_FK_FINAL", "@I_MOTIVO" }, "INT");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
            }
            else
            {
                AdicionarError($"El procedimiento {esquema}.P_B_TP_{entidad}_{proceso}_REGISTRA_TRANSICION no pudo ser generado porque ya existe en la base de datos. {mensajeError}");
            }
        }
    }
}
