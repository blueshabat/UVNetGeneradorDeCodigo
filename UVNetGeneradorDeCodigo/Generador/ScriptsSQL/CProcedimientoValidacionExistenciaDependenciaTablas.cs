namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Linq;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CProcedimientoValidacionExistenciaDependenciaTablas : CConstructorDeScriptsSQL
    {
        public CProcedimientoValidacionExistenciaDependenciaTablas(IRepositorio repositorio) : base(repositorio)
        {
        }

        public void GenerarProcedimientoDeValidacionDeExistenciaDependenciaTablas(string esquema, string tabla, out string mensajeError)
        {
            var columnas = Repositorio.ObtenerColumnas($"{esquema}.{tabla}", out mensajeError).Where(x => x.Nombre.Contains("_FK"));
            var listadoProcedimientos = Repositorio.ObtenerListaProcedimientos(out mensajeError);
            foreach (var columna in columnas)
            {
                var tablaAPartirDeColumnaReferenciada = columna.Nombre.Substring(0, columna.Nombre.IndexOf("_FK"));
                var esquemaTablaReferenciada = Repositorio.ObtenerNombreEsquema(tablaAPartirDeColumnaReferenciada, out mensajeError);
                if (string.IsNullOrEmpty(mensajeError) && !string.IsNullOrEmpty(esquemaTablaReferenciada))
                {
                    var nombreProcedimiento = $"{esquemaTablaReferenciada}.P_B_AT_{tablaAPartirDeColumnaReferenciada.Substring(2)}_V_E_SECUENCIAL";
                    if (!listadoProcedimientos.Contains(nombreProcedimiento))
                    {
                        AdicionarCabecera($"Ejecuta la validación de existencia del parámetro SECUENCIAL en la tabla {esquemaTablaReferenciada}.{tablaAPartirDeColumnaReferenciada}.");
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {nombreProcedimiento}");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        ConstructorDeCodigo.AppendLine($"  @I_SECUENCIAL {columna.TipoDato},");
                        AdicionarParametrosSalida("BIT", "BIT");
                        ConstructorDeCodigo.AppendLine($"    BEGIN");
                        ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                        ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                        ConstructorDeCodigo.AppendLine($"            BEGIN");
                        ConstructorDeCodigo.AppendLine($"                IF(EXISTS");
                        ConstructorDeCodigo.AppendLine($"                (");
                        ConstructorDeCodigo.AppendLine($"                    SELECT 1");
                        ConstructorDeCodigo.AppendLine($"                    FROM {esquemaTablaReferenciada}.{tablaAPartirDeColumnaReferenciada}");
                        ConstructorDeCodigo.AppendLine($"                    WHERE SECUENCIAL = @I_SECUENCIAL");
                        ConstructorDeCodigo.AppendLine($"                ))");
                        ConstructorDeCodigo.AppendLine($"                    BEGIN");
                        ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_BIT = 1;");
                        ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 1;");
                        ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'Registro obtenido correctamente.';");
                        ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                        ConstructorDeCodigo.AppendLine($"                END;");
                        ConstructorDeCodigo.AppendLine($"                    ELSE");
                        ConstructorDeCodigo.AppendLine($"                    BEGIN");
                        ConstructorDeCodigo.AppendLine($"                        SET @O_RESULTADO_BIT = 0;");
                        ConstructorDeCodigo.AppendLine($"                        SET @O_EXITO = 0;");
                        ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'No se pudo obtener el registro.';");
                        ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                        ConstructorDeCodigo.AppendLine($"                END;");
                        ConstructorDeCodigo.AppendLine($"        END");
                        ConstructorDeCodigo.AppendLine($"    END");
                        ConstructorDeCodigo.AppendLine($"GO");
                    }
                    else
                    {
                        AdicionarError($"El procedimiento {nombreProcedimiento} ya existe en la base de datos.");
                    }
                }
                else
                {
                    AdicionarError($"El procedimiento P_B_AT_{tablaAPartirDeColumnaReferenciada.Substring(2)}_V_E_SECUENCIAL no ha sido generado. La tabla {tablaAPartirDeColumnaReferenciada} no existe.");
                }
            }
        }
    }
}
