namespace UVNetGeneradorDeCodigo.Repositorio
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using UVNetGeneradorDeCodigo.Modelos;

    public class CRepositorio : IRepositorio
    {
        public CRepositorio(string cadenaConexion)
        {
            AbrirConexionABaseDeDatos(cadenaConexion);
        }

        private readonly IDictionary<string, string> TiposDeDato = new Dictionary<string, string>
        {
            { "nvarchar", "string" },
            { "varchar", "string" },
            { "bigint", "long" },
            { "int", "int" },
            { "smallint", "int" },
            { "tinyint", "int" },
            { "bit", "bool" },
            { "date", "DateTime" },
            { "decimal", "decimal" },
            { "numeric", "decimal" },
            { "varbinary", "byte[]" },
            { "datetime2", "DateTime" },
            { "datetime", "DateTime" }
        };

        private readonly IDictionary<string, string> TiposDeDatoResultado = new Dictionary<string, string>
        {
            { "nvarchar-1", "JsonDocument" },
            { "nvarchar", "String" },
            { "varchar", "String" },
            { "bigint", "BigInt" },
            { "int", "Int" },
            { "smallint", "Int" },
            { "tinyint", "Int" },
            { "bit", "Bool" },
            { "date", "DateTime" },
            { "decimal", "Decimal" },
            { "numeric", "Decimal" },
            { "varbinary", "ArrayByte" },
            { "datetime2", "DateTime" },
            { "datetime", "DateTime" }
        };

        private IDictionary<string, string> TiposDeDatoParaClase = new Dictionary<string, string>
        {
            { "@O_RESULTADO_STRING", "String" },
            { "@O_RESULTADO_JSON", "JsonDocument" },
            { "@O_RESULTADO_BIGINT", "BigInt" },
            { "@O_RESULTADO_INT", "Int" },
            { "@O_RESULTADO_DATETIME", "DateTime" },
            { "@O_RESULTADO_DATE", "DateTime" },
            { "@O_RESULTADO_BIT", "Bool" },
            { "@O_RESULTADO_DECIMAL", "Decimal" },
            { "@O_RESULTADO_VARBINARY", "ArrayByte" },
            { "@O_RESULTADO_XML", "XmlDocument" },
        };

        public CRepositorio(INodeInformation elementoMenu)
        {
            string cadenaDeConexion = (elementoMenu?.Connection == null ? null :
                (new SqlConnectionStringBuilder(elementoMenu.Connection.ConnectionString) { InitialCatalog = elementoMenu.Name }).ConnectionString);
            AbrirConexionABaseDeDatos(cadenaDeConexion);
        }

        private SqlConnection conexionSql;

        private void AbrirConexionABaseDeDatos(string cadenaDeConexion)
        {
            if (!string.IsNullOrEmpty(cadenaDeConexion))
            {
                conexionSql = new SqlConnection(cadenaDeConexion);
                conexionSql.Open();
            }
        }

        private delegate void AdicionarALista<T>(ref List<T> listado, SqlDataReader dr);

        public List<CColumnaTabla> ObtenerColumnas(string tabla, out string mensajeError)
        {
            return EjecutarConsulta("SELECT COLUMN_NAME, data_type + case when data_type like '%text' or data_type in ('image', 'sql_variant' ,'xml') then '' when data_type in ('float') then '(' + cast(coalesce(numeric_precision, 18) as varchar(11)) + ')' when data_type in ('datetime2', 'datetimeoffset', 'time') then '(' + cast(coalesce(datetime_precision, 7) as varchar(11)) + ')' when data_type in ('decimal', 'numeric') then '(' + cast(coalesce(numeric_precision, 18) as varchar(11)) + ',' + cast(coalesce(numeric_scale, 0) as varchar(11)) + ')' when (data_type like '%binary' or data_type like '%char') and character_maximum_length = -1 then '(max)' when character_maximum_length is not null then '(' + cast(character_maximum_length as varchar(11)) + ')' else '' end as CONDENSED_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE,	COLUMNPROPERTY(object_id(TABLE_SCHEMA+'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') IS_IDENTITY, IIF(COLUMN_DEFAULT IS NULL, 0, 1) HAS_DEFAULT_VALUE,	COLUMN_DEFAULT" +
                " FROM INFORMATION_SCHEMA.COLUMNS C WHERE TABLE_SCHEMA+'.'+TABLE_NAME = '" + tabla + "' ORDER BY ORDINAL_POSITION", new AdicionarALista<CColumnaTabla>(AdicionarColumnasAlListado), out mensajeError);
        }

        private List<T> EjecutarConsulta<T>(string consulta, AdicionarALista<T> adicionarAlListado, out string mensajeError)
        {
            List<T> resultado = new List<T>();
            mensajeError = string.Empty;
            using (var comando = new SqlCommand(consulta, conexionSql))
            {
                try
                {
                    using (var dr = comando.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            adicionarAlListado(ref resultado, dr);
                        }
                    }
                }
                catch (Exception excepcion)
                {
                    mensajeError = excepcion.Message;
                    return null;
                }
            }
            return resultado;
        }

        private void AdicionarColumnasAlListado(ref List<CColumnaTabla> listado, SqlDataReader dr)
        {
            listado.Add(new CColumnaTabla
            {
                Nombre = dr.GetString(0),
                TipoDato = dr.GetString(1).ToUpper(),
                TamañoMaximo = dr.IsDBNull(2) ? 0 : dr.GetInt32(2),
                Presicion = dr.IsDBNull(3) ? 0 : Convert.ToInt32(dr[3], System.Globalization.CultureInfo.InvariantCulture),
                Escala = dr.IsDBNull(4) ? 0 : Convert.ToInt32(dr[4], System.Globalization.CultureInfo.InvariantCulture),
                EsIdentidad = dr.GetInt32(5) == 1,
                TieneValorPorDefecto = dr.GetInt32(6) == 1,
                ValorPorDefecto = dr.IsDBNull(7) ? string.Empty : dr.GetString(7)
            });
        }

        private void ListarDependencias(ref List<CDependenciaProcedimiento> listado, SqlDataReader dr)
        {
            listado.Add(new CDependenciaProcedimiento
            {
                Objeto = dr.GetString(0),
                ObjetoDependiente = dr.GetString(1),
                Nivel = dr.GetInt32(2)
            });

        }

        private void AdicionarParametrosProcesoAlListado(ref List<CParametroProceso> listado, SqlDataReader dr)
        {
            listado.Add(new CParametroProceso
            {
                Nombre = dr.GetString(0),
                TipoDeDato = dr.GetString(1),
                PRESeleccionado = dr.GetInt32(2) == 1
            });
        }

        private void AdicionarParametrosProcedimientoAlListado(ref List<CParametroProcedimiento> listado, SqlDataReader dr)
        {
            listado.Add(new CParametroProcedimiento
            {
                Nombre = dr.GetString(0),
                EsParametroSalida = dr.GetString(0).Contains("@O"),
                TipoDeDatoCSharp = TiposDeDato[dr.GetString(1)],
                TipoDeDato = dr.GetString(1),
                TipoDeDatoClaseCSharp = TiposDeDatoResultado[$"{dr.GetString(1)}{(dr.GetInt16(2) == -1 ? dr.GetInt16(2).ToString() : string.Empty)}"]
            });
        }

        private void AdicionarColumnasVistaAlListado(ref List<CColumnaVista> listado, SqlDataReader dr)
        {
            listado.Add(new CColumnaVista
            {
                Nombre = dr.GetString(0),
                TipoDeDatoClaseCSharp = TiposDeDatoResultado[$"{dr.GetString(1)}{(dr.GetInt16(2) == -1 ? dr.GetInt16(2).ToString() : string.Empty)}"]
            }); 
        }

        public void Dispose()
        {
            if (conexionSql != null)
            {
                conexionSql.Close();
                conexionSql = null;
            }
        }

        public string ObtenerNombreBaseDatos(out string mensajeError)
        {
            return EjecutarConsulta("SELECT DB_NAME()", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError).FirstOrDefault();
        }

        private void AdicionarResultadoUnico(ref List<string> listado, SqlDataReader dr)
        {
            listado.Add(dr.GetString(0));
        }

        private void AdicionarResultadoUnico(ref List<bool> listado, SqlDataReader dr)
        {
            listado.Add(dr.GetInt32(0) == 1);
        }

        public string ObtenerNombreUsuario(out string mensajeError)
        {
            return EjecutarConsulta("select upper(user)", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError).FirstOrDefault();
        }

        public string ObtenerNombreEsquema(string objeto, out string mensajeError)
        {
            return EjecutarConsulta($"SELECT DISTINCT OBJECT_SCHEMA_NAME(object_id) FROM sys.objects O WHERE O.name = '{objeto}'", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError).FirstOrDefault();
        }

        public List<string> ObtenerBasesDeDatos(out string mensajeError)
        {
            return EjecutarConsulta("SELECT name FROM sys.databases WHERE database_id > 5 ORDER BY name ASC", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError);
        }

        public List<string> ObtenerListaProcedimientos(out string mensajeError)
        {
            return EjecutarConsulta("SELECT concat(ROUTINE_SCHEMA,'.',ROUTINE_NAME) FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' order by ROUTINE_SCHEMA", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError);
        }

        public List<string> ObtenerEsquemas(string baseDeDatos, out string mensajeError)
        {
            return EjecutarConsulta($"SELECT s.name FROM {baseDeDatos}.sys.schemas s INNER JOIN {baseDeDatos}.sys.sysusers u ON u.uid = s.principal_id WHERE  s.name NOT IN('sys', 'guest', 'INFORMATION_SCHEMA', 'dbo') and LEFT(u.name, 3) <> 'db_' ORDER BY s.name ASC", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError);
        }

        public List<string> ObtenerEntidades(string baseDeDatos, out string mensajeError)
        {
            var entidades = EjecutarConsulta($"SELECT CONCAT(SCHEMA_NAME(schema_id),'.', REPLACE(REPLACE(name, '_EN', ''), 'P_B_PR_', '')) FROM {baseDeDatos}.sys.procedures WHERE name like 'P_B_PR_%_%[_][a-z][a-z]'", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError);

            return entidades;
        }

        public List<string> ObtenerOperaciones(string baseDeDatos, string esquema, string entidad, out string mensajeError)
        {
            var operaciones = EjecutarConsulta($"SELECT P.name FROM {baseDeDatos}.sys.procedures P JOIN {baseDeDatos}.sys.schemas S ON P.schema_id = S.schema_id WHERE P.name like CONCAT('P_B_PR_', '{entidad}', '_%[_][a-z][a-z]') AND S.name = '{esquema}'", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError);
            return operaciones.Select(x => x.Split('_').Length == 7 ? x.Split('_')[4] + "_" + x.Split('_')[5] : x.Split('_')[4]).ToList();
        }

        public List<CParametroProceso> ObtenerParametrosProceso(string baseDeDatos, string esquema, string proceso, out string mensajeError)
        {
            return EjecutarConsulta($"SELECT B.name, B.TIPO, IIF(A.name IS NOT NULL,1,0) FROM(SELECT name,UPPER(concat(TYPE_NAME(user_type_id),CASE WHEN TYPE_NAME(user_type_id) IN('nvarchar', 'varchar', 'nchar', 'char') THEN concat('(', IIF(max_length = -1, 'MAX', CAST(OdbcPrec(system_type_id, max_length, precision) AS NVARCHAR(200))), ')') WHEN TYPE_NAME(user_type_id) IN('decimal') THEN concat('(', OdbcPrec(system_type_id, max_length, precision), ',', OdbcScale(system_type_id, scale), ')') ELSE '' END)) TIPO FROM {baseDeDatos}.sys.parameters WHERE object_id = object_id('{esquema}.P_B_PR_{proceso}_PRE') AND is_output = 0 AND name NOT IN('@I_CODIGO_SISTEMA','@I_USUARIO_AUT')) AS A right JOIN(SELECT name,UPPER(concat(TYPE_NAME(user_type_id),CASE WHEN TYPE_NAME(user_type_id) IN('nvarchar', 'varchar', 'nchar', 'char') THEN concat('(', IIF(max_length = -1, 'MAX', CAST(OdbcPrec(system_type_id, max_length, precision) AS NVARCHAR(200))), ')') WHEN TYPE_NAME(user_type_id) IN('decimal') THEN concat('(', OdbcPrec(system_type_id, max_length, precision), ',', OdbcScale(system_type_id, scale), ')') ELSE '' END)) TIPO FROM {baseDeDatos}.sys.parameters WHERE object_id = object_id('{esquema}.P_B_PR_{proceso}_EN') AND is_output = 0 AND name NOT IN('@I_CODIGO_SISTEMA','@I_USUARIO_AUT')) AS B ON A.name = B.name", new AdicionarALista<CParametroProceso>(AdicionarParametrosProcesoAlListado), out mensajeError);
        }

        public List<string> ObtenerDefinicionProcedimiento(string baseDeDatos, string esquema, string procedimiento, out string mensajeError)
        {
            return EjecutarConsulta($"DECLARE @V_TABLA TABLE(linea VARCHAR(8000));INSERT INTO @V_TABLA EXEC {baseDeDatos}.dbo.sp_helptext '{esquema}.{procedimiento}';SELECT REPLACE(T.linea,CHAR(13)+CHAR(10),' ') FROM(SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS numero_linea, linea FROM @V_TABLA) T WHERE numero_linea > (SELECT TOP 1 n FROM(SELECT ROW_NUMBER() OVER(ORDER BY(SELECT 1)) AS n, linea FROM @V_TABLA) T WHERE linea LIKE concat('AS',char(13),CHAR(10)));", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError);
        }

        public List<string> ObtenerProcedimientosDeUnProceso(string baseDeDatos, string esquema, string proceso, out string mensajeError)
        {
            return EjecutarConsulta($"SELECT P.name FROM {baseDeDatos}.sys.procedures P JOIN {baseDeDatos}.sys.schemas S ON S.schema_id = P.schema_id WHERE P.name like concat('P_B_PR_','{proceso}','_%') AND S.name = '{esquema}';", new AdicionarALista<string>(AdicionarResultadoUnico), out mensajeError);
        }

        public List<CDependenciaProcedimiento> ObtenerProdimientosDependientes(string objeto, out string mensajeError)
        {
            return EjecutarConsulta($"WITH Objetos AS(SELECT DISTINCT CONCAT(s.name, '.', b.name) AS Objeto,CONCAT(sd.name,'.',c.name) AS ObjetoDependiente FROM sys.sysdepends a INNER JOIN sys.objects b ON a.id = b.object_id INNER JOIN sys.objects c ON a.depid = c.object_id INNER JOIN sys.schemas AS s ON s.schema_id = b.schema_id INNER JOIN sys.schemas AS sd ON sd.schema_id = c.schema_id WHERE b.type IN ('P') AND c.type IN ('P')), ObjetosDependientes AS(SELECT Objeto, ObjetoDependiente, 1 AS Nivel FROM Objetos a WHERE a.Objeto = '{objeto}' UNION ALL SELECT a.Objeto, a.ObjetoDependiente, (b.Nivel + 1) AS Level FROM Objetos a INNER JOIN  ObjetosDependientes b ON a.Objeto = b.ObjetoDependiente) SELECT DISTINCT * FROM ObjetosDependientes ORDER BY Nivel, ObjetoDependiente", new AdicionarALista<CDependenciaProcedimiento>(ListarDependencias), out mensajeError);
        }

        public bool ExisteEsquema(string esquema, out string mensajeError)
        {
            return EjecutarConsulta($"IF(EXISTS(SELECT 1 FROM sys.schemas WHERE name = '{esquema}'))SELECT 1;ELSE SELECT 0;", new AdicionarALista<bool>(AdicionarResultadoUnico), out mensajeError).FirstOrDefault();
        }

        public void RecompilarVistas(out string mensajeError)
        {
            EjecutarConsulta("DECLARE Recompilar CURSOR FOR SELECT OBJECT_SCHEMA_NAME([id]) AS esquema, [name] FROM sysobjects WHERE xtype IN('v'); OPEN Recompilar; DECLARE @V_ESQUEMA VARCHAR(250); DECLARE @V_NOMBRE VARCHAR(250); FETCH NEXT FROM Recompilar INTO @V_ESQUEMA, @V_NOMBRE; WHILE @@fetch_status = 0 BEGIN DECLARE @V_OBJETO NVARCHAR(500)= concat(@V_ESQUEMA, '.', @V_NOMBRE); EXEC sp_recompile @V_OBJETO; EXEC sp_refreshview @V_OBJETO; FETCH NEXT FROM Recompilar INTO @V_ESQUEMA, @V_NOMBRE; END; CLOSE Recompilar; DEALLOCATE Recompilar;", new AdicionarALista<bool>(AdicionarResultadoUnico), out mensajeError);
        }

        public List<CParametroProcedimiento> ObtenerParametrosProcedimiento(string procedimiento, out string mensajeError)
        {
            return EjecutarConsulta($"SELECT name, type_name(user_type_id), max_length FROM sys.parameters WHERE object_id = OBJECT_ID('{procedimiento}')", new AdicionarALista<CParametroProcedimiento>(AdicionarParametrosProcedimientoAlListado), out mensajeError);
        }

        public List<CParametroProcedimiento> ObtenerParametrosVista(string vista, out string mensajeError)
        {
            return EjecutarConsulta($"SELECTC.COLUMN_NAME,C.DATA_TYPE,data_type+CASEWHENdata_typeLIKE'%text'ORdata_typeIN('image','sql_variant','xml')THEN''WHENdata_typeIN('float')THEN'('+CAST(COALESCE(numeric_precision,18)ASVARCHAR(11))+')'WHENdata_typeIN('datetime2','datetimeoffset','time')THEN'('+CAST(COALESCE(datetime_precision,7)ASVARCHAR(11))+')'WHENdata_typeIN('decimal','numeric')THEN'('+CAST(COALESCE(numeric_precision,18)ASVARCHAR(11))+','+CAST(COALESCE(numeric_scale,0)ASVARCHAR(11))+')'WHEN(data_typeLIKE'%binary'ORdata_typeLIKE'%char')ANDcharacter_maximum_length=-1THEN'(max)'WHENcharacter_maximum_lengthISNOTNULLTHEN'('+CAST(character_maximum_lengthASVARCHAR(11))+')'ELSE''ENDASCONDENSED_TYPEFROMINFORMATION_SCHEMA.VIEWSVJOININFORMATION_SCHEMA.COLUMNSCONC.TABLE_SCHEMA=V.TABLE_SCHEMAANDC.TABLE_NAME=V.TABLE_NAMEWHEREC.TABLE_SCHEMA+'.'+C.TABLE_NAME='{vista}';", new AdicionarALista<CParametroProcedimiento>(AdicionarParametrosProcedimientoAlListado), out mensajeError);
        }

        public string ObtenerTipoDatoResultado(string tipoDatoSql) => TiposDeDatoParaClase[tipoDatoSql];
    }

    public static class CExtensionesRepositorio
    {
        public static List<CColumnaTabla> ExcluirAuditoria(this List<CColumnaTabla> columnas)
        {
            var columnasAuditoria = new List<string> { "FECHA_ADICION", "FECHA_MODIFICACION", "ADICIONADO_POR", "MODIFICADO_POR" };
            return columnas.Where(x => !columnasAuditoria.Contains(x.Nombre)).ToList();
        }

        public static List<CColumnaTabla> ExcluirSecuencial(this List<CColumnaTabla> columnas)
        {
            var columnaSecuencial = new List<string> { "SECUENCIAL"};
            return columnas.Where(x => !columnaSecuencial.Contains(x.Nombre)).ToList();
        }

        public static List<CColumnaTabla> ExcluirColumnas(this List<CColumnaTabla> columnas, List<string> columnasAExcluir)
        {
            return columnas.Where(x => !columnasAExcluir.Contains(x.Nombre)).ToList();
        }

        public static List<CColumnaTabla> ExcluirControlBD(this List<CColumnaTabla> columnas)
        {
            var columnasControlBD = new List<string> { "ID_PARTICION", "OBSERVACION" };
            return columnas.Where(x => !columnasControlBD.Contains(x.Nombre)).ToList();
        }
    }
}
