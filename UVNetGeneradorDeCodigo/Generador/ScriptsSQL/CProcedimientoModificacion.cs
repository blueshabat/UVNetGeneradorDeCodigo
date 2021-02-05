﻿namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CProcedimientoModificacion : CConstructorDeScriptsSQL
    {
        public CProcedimientoModificacion(IRepositorio repositorio) : base(repositorio)
        {
        }

        public void GenerarProcedimientoModificacion(string esquema, string tabla)
        {
            var entidad = tabla.Substring(2);
            var columnas = Repositorio.ObtenerColumnas($"{ esquema}.{ tabla}", out _).ExcluirAuditoria().ExcluirSecuencial();
            AdicionarCabecera($"Realiza la modificación de un registro en la tabla {esquema}.{tabla}");
            ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_AT_{entidad}_C");
            AdicionarParametrosEntradaUsuarioCodigoSistema();
            ConstructorDeCodigo.AppendLine($"    @I_SECUENCIAL INT,");
            foreach (var columna in columnas)
            {
                ConstructorDeCodigo.AppendLine($"    @I_{columna.Nombre} {columna.TipoDato},");
            }
            AdicionarParametrosSalida("INT", "INT");
            ConstructorDeCodigo.AppendLine($"    BEGIN");
            ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
            ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
            ConstructorDeCodigo.AppendLine($"            BEGIN");
            ConstructorDeCodigo.AppendLine($"                DECLARE @V_FECHA_MODIFICACION DATETIME2= GETDATE();");
            ConstructorDeCodigo.AppendLine($"                DECLARE @V_JSON_AUDITORIA_DATA VARCHAR(MAX)=");
            ConstructorDeCodigo.AppendLine($"                (");
            ConstructorDeCodigo.AppendLine($"                    SELECT *");
            ConstructorDeCodigo.AppendLine($"                    FROM {esquema}.{tabla}");
            ConstructorDeCodigo.AppendLine($"                    WHERE SECUENCIAL = @I_SECUENCIAL FOR JSON PATH, INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER");
            ConstructorDeCodigo.AppendLine($"                );");
            ConstructorDeCodigo.AppendLine($"                UPDATE {esquema}.{tabla}");
            ConstructorDeCodigo.AppendLine($"                  SET ");
            ConstructorDeCodigo.AppendLine($"                      MODIFICADO_POR = @I_USUARIO_AUT,");
            ConstructorDeCodigo.Append($"                      FECHA_MODIFICACION = @V_FECHA_MODIFICACION");
            foreach (var columna in columnas)
            {
                ConstructorDeCodigo.AppendLine($",");
                ConstructorDeCodigo.Append($"                      {columna.Nombre} = @I_{columna.Nombre}");
            }
            ConstructorDeCodigo.AppendLine($"                WHERE SECUENCIAL = @I_SECUENCIAL;");
            ConstructorDeCodigo.AppendLine($"                IF(@@ROWCOUNT > 0)");
            ConstructorDeCodigo.AppendLine($"                    BEGIN");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_MOTIVO VARCHAR(800)= 'Modificación de un registro en la tabla {esquema}.{tabla}';");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_INT INT= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_BIG_INT BIGINT= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_DECIMAL DECIMAL(16, 4)= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_DATETIME DATETIME2= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_VARCHAR_40 VARCHAR(40)= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_VARCHAR_400 VARCHAR(400)= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_DATO_VARCHAR_4000 VARCHAR(4000)= NULL;");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_NOMBRE_BD VARCHAR(100)= DB_NAME();");
            ConstructorDeCodigo.AppendLine($"                        DECLARE @V_NOMBRE_SP VARCHAR(100)= OBJECT_NAME(@@PROCID);");
            ConstructorDeCodigo.AppendLine($"                        EXEC BD_TRANS_AUDITORIA_DATA.AUDITORIA.P_B_TABLAS_VARIACION_A ");
            ConstructorDeCodigo.AppendLine($"                             @I_USUARIO_AUT, ");
            ConstructorDeCodigo.AppendLine($"                             @I_CODIGO_SISTEMA, ");
            ConstructorDeCodigo.AppendLine($"                             'UPDATE', ");
            ConstructorDeCodigo.AppendLine($"                             @V_NOMBRE_SP, ");
            ConstructorDeCodigo.AppendLine($"                             @V_NOMBRE_BD, ");
            ConstructorDeCodigo.AppendLine($"                             '{esquema}', ");
            ConstructorDeCodigo.AppendLine($"                             '{tabla}', ");
            ConstructorDeCodigo.AppendLine($"                             @I_SECUENCIAL, ");
            ConstructorDeCodigo.AppendLine($"                             @V_JSON_AUDITORIA_DATA, ");
            ConstructorDeCodigo.AppendLine($"                             @V_MOTIVO, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_INT, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_BIG_INT, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_DECIMAL, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_DATETIME, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_VARCHAR_40, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_VARCHAR_400, ");
            ConstructorDeCodigo.AppendLine($"                             @V_DATO_VARCHAR_4000;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_INT = @I_SECUENCIAL;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 1;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'El registro fue modificado correctamente.';");
            ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
            ConstructorDeCodigo.AppendLine($"                END;");
            ConstructorDeCodigo.AppendLine($"                    ELSE");
            ConstructorDeCodigo.AppendLine($"                    BEGIN");
            ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_INT = 0;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 0;");
            ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'El registro no pudo ser modificado.';");
            ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
            ConstructorDeCodigo.AppendLine($"                END;");
            ConstructorDeCodigo.AppendLine($"        END;");
            ConstructorDeCodigo.AppendLine($"    END;");
            ConstructorDeCodigo.AppendLine($"GO");
        }
    }
}
