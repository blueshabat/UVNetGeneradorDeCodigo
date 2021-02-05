namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Collections.Generic;
    using System.Linq;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Dialogos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CCreacionProceso : CConstructorDeScriptsSQL
    {
        public CCreacionProceso(IRepositorio repositorio) : base(repositorio)
        {
        }

        public void GenerarCreacionDeProceso(string esquema, string entidad, string proceso, string subproceso, List<Parametro> columnas, bool crearProcedimientosDeObtencionPRE, bool crearProcedimientosDeObtencionEN, out string mensajeError)
        {
            var listadoProcedimientos = Repositorio.ObtenerListaProcedimientos(out mensajeError);
            var procedimientosConValidacionExistencia = new List<(string parametro, string procedimiento, bool PRESeleccionado, bool ENSeleccionado)>();
            foreach (var columna in columnas.Where(x => x.Nombre.Contains("_FK")))
            {
                var tablaAPartirDeColumnaReferenciada = columna.Nombre.Replace("@I_", "").Substring(0, columna.Nombre.Replace("@I_", "").IndexOf("_FK"));
                var esquemaTablaReferenciada = Repositorio.ObtenerNombreEsquema(tablaAPartirDeColumnaReferenciada, out mensajeError);
                if (string.IsNullOrEmpty(mensajeError) && !string.IsNullOrEmpty(esquemaTablaReferenciada))
                {
                    var nombreProcedimiento = $"{esquemaTablaReferenciada}.P_B_AT_{tablaAPartirDeColumnaReferenciada.Substring(2)}_V_E_SECUENCIAL";
                    procedimientosConValidacionExistencia.Add((columna.Nombre, nombreProcedimiento, columna.PRESeleccionado, columna.ENSeleccionado));
                    if (!listadoProcedimientos.Contains(nombreProcedimiento))
                    {
                        AdicionarCabecera("Ejecuta la validación de existencia del parámetro SECUENCIAL");
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {nombreProcedimiento}");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        ConstructorDeCodigo.AppendLine($"   @I_SECUENCIAL {columna.TipoDato},");
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
                        AdicionarError($"El procedimiento {nombreProcedimiento} no pudo ser generado porque ya existe en la base de datos. {mensajeError}");
                    }
                }
                else
                {
                    AdicionarError($"El procedimiento para la tabla {tablaAPartirDeColumnaReferenciada} no se pudo generar. {mensajeError}");
                }
            }

            if (columnas.Any(x => x.PRESeleccionado))
            {
                AdicionarCabecera("Ejecuta la validación de variables.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N1_PARAMETROS");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                AdicionarCuerpoProcedimientoPorDefecto();
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                AdicionarCabecera("Ejecuta la validación de existencias.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N1_EXISTENCIAS");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                if (procedimientosConValidacionExistencia.Any())
                {
                    foreach (var procedimiento in procedimientosConValidacionExistencia.Where(x => x.PRESeleccionado))
                    {
                        AdicionarEjecucionProcedimiento(procedimiento.procedimiento, new List<string> { procedimiento.parametro }, "BIT");
                    }
                }
                else
                {
                    AdicionarCuerpoProcedimientoPorDefecto();
                }
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                AdicionarCabecera("Ejecuta las validaciones en tablas no transaccionales.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N1");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N1_PARAMETROS", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N1_EXISTENCIAS", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                AdicionarCabecera("Ejecuta la validación en tablas transaccionales.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N2_REGISTRABLE");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                AdicionarCuerpoProcedimientoPorDefecto();
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                AdicionarCabecera("Ejecuta las validaciones en tablas transaccionales.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N2");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N2_REGISTRABLE", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                if (crearProcedimientosDeObtencionPRE)
                {
                    AdicionarCabecera("Ejecuta la obtención de información después de las validaciones.");
                    ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_O");
                    AdicionarParametrosEntradaUsuarioCodigoSistema();
                    foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                    {
                        ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                    }
                    AdicionarParametrosSalida("JSON", "NVARCHAR(MAX)");
                    ConstructorDeCodigo.AppendLine($"    BEGIN");
                    ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                    ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                    ConstructorDeCodigo.AppendLine($"            BEGIN");
                    AdicionarCuerpoProcedimientoPorDefecto();
                    ConstructorDeCodigo.AppendLine($"        END;");
                    ConstructorDeCodigo.AppendLine($"    END;");
                    ConstructorDeCodigo.AppendLine($"GO");
                }
                AdicionarCabecera("Ejecuta todas las validaciones previas a la operación del proceso.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                if (crearProcedimientosDeObtencionPRE)
                {
                    AdicionarParametrosSalida("JSON", "NVARCHAR(MAX)");
                }
                else
                {
                    AdicionarParametrosSalida("BIT", "BIT");
                }
                
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                if (crearProcedimientosDeObtencionPRE)
                {
                    ConstructorDeCodigo.AppendLine($"                DECLARE @V_RESULTADO_BIT BIT;");
                }
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N1", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT", crearProcedimientosDeObtencionPRE);
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N2", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT", crearProcedimientosDeObtencionPRE);
                if (crearProcedimientosDeObtencionPRE)
                {
                    AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_O", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "JSON");
                }
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
            }
            if (columnas.Any(x => x.ENSeleccionado))
            {
                AdicionarCabecera("Ejecuta la validación de variables.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N1_PARAMETROS");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                AdicionarCuerpoProcedimientoPorDefecto();
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                AdicionarCabecera("Ejecuta la validación de existencias.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N1_EXISTENCIAS");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                if (procedimientosConValidacionExistencia.Any())
                {
                    foreach (var procedimiento in procedimientosConValidacionExistencia.Where(x => x.ENSeleccionado))
                    {
                        AdicionarEjecucionProcedimiento(procedimiento.procedimiento, new List<string> { procedimiento.parametro }, "BIT");
                    }
                }
                else
                {
                    AdicionarCuerpoProcedimientoPorDefecto();
                }
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                AdicionarCabecera("Ejecuta las validaciones en tablas no transaccionales.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N1");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N1_PARAMETROS", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N1_EXISTENCIAS", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                AdicionarCabecera("Ejecuta la validación en tablas transaccionales.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N2_REGISTRABLE");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                AdicionarCuerpoProcedimientoPorDefecto();
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                AdicionarCabecera("Ejecuta las validaciones en tablas transaccionales.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N2");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N2_REGISTRABLE", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                if (crearProcedimientosDeObtencionEN)
                {
                    AdicionarCabecera("Ejecuta la obtención de información después de las validaciones.");
                    ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_O");
                    AdicionarParametrosEntradaUsuarioCodigoSistema();
                    ConstructorDeCodigo.AppendLine($"   @I_SECUENCIAL INT,");
                    AdicionarParametrosSalida("JSON", "NVARCHAR(MAX)");
                    ConstructorDeCodigo.AppendLine($"    BEGIN");
                    ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                    ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                    ConstructorDeCodigo.AppendLine($"            BEGIN");
                    AdicionarCuerpoProcedimientoPorDefecto();
                    ConstructorDeCodigo.AppendLine($"        END;");
                    ConstructorDeCodigo.AppendLine($"    END;");
                    ConstructorDeCodigo.AppendLine($"GO");
                }
                AdicionarCabecera("Ejecuta la operación del proceso.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}{(!string.IsNullOrEmpty(subproceso) ? "_" + subproceso : string.Empty)}_EN_R");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas)
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("INT", "INT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                AdicionarCuerpoProcedimientoPorDefecto();
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
                AdicionarCabecera("Ejecuta la operación del proceso.");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas)
                {
                    ConstructorDeCodigo.AppendLine($"   {columna.Nombre} {columna.TipoDato},");
                }
                if (crearProcedimientosDeObtencionEN)
                {
                    AdicionarParametrosSalida("JSON", "NVARCHAR(MAX)");
                }
                else
                {
                    AdicionarParametrosSalida("INT", "INT");
                }
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                ConstructorDeCodigo.AppendLine($"                DECLARE @V_RESULTADO_BIT BIT;");
                if (crearProcedimientosDeObtencionEN)
                {
                    ConstructorDeCodigo.AppendLine($"                DECLARE @V_RESULTADO_INT INT;");
                }
                if (columnas.Any(x => x.PRESeleccionado))
                {
                    AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N1", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT", true);
                }
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N1", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT", true);
                if (columnas.Any(x => x.PRESeleccionado))
                {
                    AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}PRE_V_N2", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT", true);
                }
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_V_N2", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT", true);
                AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}{(!string.IsNullOrEmpty(subproceso) ? "_" + subproceso : string.Empty)}_EN_R", columnas.Select(x => x.Nombre), "INT", crearProcedimientosDeObtencionEN);
                if (crearProcedimientosDeObtencionEN)
                {
                    AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{entidad}_{proceso}_{(!string.IsNullOrEmpty(subproceso) ? subproceso + "_" : string.Empty)}EN_O", new List<string> { "@V_RESULTADO_INT" }, "JSON");
                }
                ConstructorDeCodigo.AppendLine($"        END;");
                ConstructorDeCodigo.AppendLine($"    END;");
                ConstructorDeCodigo.AppendLine($"GO");
            }
        }
    }
}
