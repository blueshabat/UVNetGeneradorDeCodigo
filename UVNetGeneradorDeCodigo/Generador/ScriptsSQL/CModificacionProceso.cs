namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Collections.Generic;
    using System.Linq;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Dialogos;
    using UVNetGeneradorDeCodigo.Modelos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CModificacionProceso : CConstructorDeScriptsSQL
    {
        public CModificacionProceso(IRepositorio repositorio) : base(repositorio)
        {
        }

        public void GenerarModificacionDeProceso(Modelos.CModificacionProceso modificacionProceso, out string mensajeError)
        {
            List<string> definicionProcedimiento;
            var columnas = modificacionProceso.Columnas;
            var esquema = modificacionProceso.Esquema;
            var proceso = modificacionProceso.Proceso;
            var baseDeDatos = modificacionProceso.BaseDeDatos;
            var listadoProcedimientos = Repositorio.ObtenerListaProcedimientos(out mensajeError);
            var procedimientosConValidacionExistencia = new List<(string parametro, string procedimiento, bool PRESeleccionado, bool ENSeleccionado, bool creado)>();
            foreach (var columna in columnas.Where(x => x.Nombre.Contains("_FK")))
            {
                var tablaAPartirDeColumnaReferenciada = columna.Nombre.Replace("@I_", "").Substring(0, columna.Nombre.Replace("@I_", "").IndexOf("_FK"));
                var esquemaTablaReferenciada = Repositorio.ObtenerNombreEsquema(tablaAPartirDeColumnaReferenciada, out mensajeError);
                if (string.IsNullOrEmpty(mensajeError) && !string.IsNullOrEmpty(esquemaTablaReferenciada))
                {
                    var nombreProcedimiento = $"{esquemaTablaReferenciada}.P_B_AT_{tablaAPartirDeColumnaReferenciada.Substring(2)}_V_E_SECUENCIAL";
                    bool creado;
                    if (creado = !listadoProcedimientos.Contains(nombreProcedimiento))
                    {
                        AdicionarCabecera("Ejecuta la validación de existencia del parámetro SECUENCIAL");
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {nombreProcedimiento}");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        ConstructorDeCodigo.AppendLine($"    @I_SECUENCIAL {columna.TipoDato},");
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
                    procedimientosConValidacionExistencia.Add((columna.Nombre, nombreProcedimiento, columna.PRESeleccionado, columna.ENSeleccionado, creado));
                }
                else
                {
                    AdicionarError($"El procedimiento para la tabla {tablaAPartirDeColumnaReferenciada} no se pudo generar. {mensajeError}");
                }
            }
            if (columnas.Any(x => x.PRESeleccionado))
            {
                switch (modificacionProceso.OperacionPRE)
                {
                    case OperacionModificacion.Crear:
                        AdicionarCabecera("Ejecuta la validación de variables.");
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N1_PARAMETROS");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
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
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N1_EXISTENCIAS");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
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
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N1");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        ConstructorDeCodigo.AppendLine($"    BEGIN");
                        ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                        ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                        ConstructorDeCodigo.AppendLine($"            BEGIN");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1_PARAMETROS", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1_EXISTENCIAS", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta la validación en tablas transaccionales.");
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N2_REGISTRABLE");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
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
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N2");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        ConstructorDeCodigo.AppendLine($"    BEGIN");
                        ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                        ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                        ConstructorDeCodigo.AppendLine($"            BEGIN");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N2_REGISTRABLE", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        if (modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear)
                        {
                            AdicionarCabecera("Ejecuta la obtención de información después de las validaciones.");
                            ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_O");
                            AdicionarParametrosEntradaUsuarioCodigoSistema();
                            foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                            {
                                ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
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
                        AdicionarCabecera("Ejecuta todas las validaciones previas a la operación del proceso");
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_PRE");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        if (modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear)
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
                        if (modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear)
                        {
                            ConstructorDeCodigo.AppendLine($"                DECLARE @V_RESULTADO_BIT BIT;");
                        }
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre),
                            "BIT", modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear);
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N2", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre),
                            "BIT", modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear);
                        if (modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear)
                        {
                            AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_O", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "JSON");
                        }
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        break;
                    case OperacionModificacion.Modificar:
                        AdicionarCabecera("Ejecuta la validación de variables.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N1_PARAMETROS");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        definicionProcedimiento = Repositorio.ObtenerDefinicionProcedimiento(baseDeDatos, esquema, $"P_B_PR_{proceso}_PRE_V_N1_PARAMETROS", out mensajeError);
                        foreach (var linea in definicionProcedimiento)
                        {
                            ConstructorDeCodigo.AppendLine(linea);
                        }
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta la validación de existencias.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N1_EXISTENCIAS");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        definicionProcedimiento = Repositorio.ObtenerDefinicionProcedimiento(baseDeDatos, esquema, $"P_B_PR_{proceso}_PRE_V_N1_EXISTENCIAS", out mensajeError);
                        foreach (var linea in definicionProcedimiento)
                        {
                            ConstructorDeCodigo.AppendLine(linea);
                        }
                        foreach (var procedimiento in procedimientosConValidacionExistencia.Where(x => x.PRESeleccionado && x.creado))
                        {
                            AdicionarEjecucionProcedimiento(procedimiento.procedimiento, new List<string> { procedimiento.parametro }, "BIT");
                        }
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta las validaciones en tablas no transaccionales.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N1");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        ConstructorDeCodigo.AppendLine($"    BEGIN");
                        ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                        ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                        ConstructorDeCodigo.AppendLine($"            BEGIN");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1_PARAMETROS", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1_EXISTENCIAS", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta la validación en tablas transaccionales.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N2_REGISTRABLE");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        definicionProcedimiento = Repositorio.ObtenerDefinicionProcedimiento(baseDeDatos, esquema, $"P_B_PR_{proceso}_PRE_V_N2_REGISTRABLE", out mensajeError);
                        foreach (var linea in definicionProcedimiento)
                        {
                            ConstructorDeCodigo.AppendLine(linea);
                        }
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta las validaciones en tablas transaccionales.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_V_N2");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        ConstructorDeCodigo.AppendLine($"    BEGIN");
                        ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                        ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                        ConstructorDeCodigo.AppendLine($"            BEGIN");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N2_REGISTRABLE", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarProcedimientoObtencionPRE(modificacionProceso.ProcedimientoObtencionPRE, baseDeDatos, esquema, columnas, proceso, out mensajeError);
                        AdicionarCabecera("Ejecuta todas las validaciones previas a la operación del proceso");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_PRE");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        if (modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear)
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
                        if (modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear || modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Modificar)
                        {
                            ConstructorDeCodigo.AppendLine($"                DECLARE @V_RESULTADO_BIT BIT;");
                        }
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre),
                            "BIT", modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear || modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Modificar);
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N2", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre),
                            "BIT", modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear || modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Modificar);
                        if (modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Crear || modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Modificar)
                        {
                            AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_O", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "JSON");
                        }
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        break;
                    case OperacionModificacion.Eliminar:
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1_PARAMETROS");
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1_EXISTENCIAS");
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1");
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N2_REGISTRABLE");
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N2");
                        if (modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Eliminar)
                        {
                            AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_O");
                        }
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE");
                        break;
                }
            }
            if (columnas.Any(x => x.ENSeleccionado))
            {
                switch (modificacionProceso.OperacionEN)
                {
                    case OperacionModificacion.Crear:
                        AdicionarCabecera("Ejecuta la validación de variables.");
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N1_PARAMETROS");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
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
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N1_EXISTENCIAS");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
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
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N1");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        ConstructorDeCodigo.AppendLine($"    BEGIN");
                        ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                        ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                        ConstructorDeCodigo.AppendLine($"            BEGIN");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N1_PARAMETROS", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N1_EXISTENCIAS", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                        ConstructorDeCodigo.AppendLine($"                EXEC {esquema}.P_B_PR_{proceso}_EN_V_N1_EXISTENCIAS ");
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta la validación en tablas transaccionales.");
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N2_REGISTRABLE");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
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
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N2");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        ConstructorDeCodigo.AppendLine($"    BEGIN");
                        ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                        ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                        ConstructorDeCodigo.AppendLine($"            BEGIN");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N2_REGISTRABLE", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        if (modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Crear)
                        {
                            AdicionarCabecera("Ejecuta la obtención de información después de las validaciones.");
                            ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_EN_O");
                            AdicionarParametrosEntradaUsuarioCodigoSistema();
                            ConstructorDeCodigo.AppendLine($"    @I_SECUENCIAL INT,");
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
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_EN_R");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas)
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
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
                        AdicionarCabecera("Ejecuta la operación de un proceso.");
                        ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_EN");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas)
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        if (modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Crear)
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
                        if (modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Crear)
                        {

                            ConstructorDeCodigo.AppendLine($"                DECLARE @V_RESULTADO_INT INT;");
                        }
                        if (columnas.Any(x => x.PRESeleccionado))
                        {
                            AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                        }
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N1", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                        if (columnas.Any(x => x.PRESeleccionado))
                        {
                            AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N2", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT");
                        }
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N2", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_R", columnas.Select(x => x.Nombre), "INT", modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Crear);
                        if (modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Crear)
                        {
                            AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_O", new List<string> { "@V_RESULTADO_INT" }, "JSON");
                        }
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        break;
                    case OperacionModificacion.Modificar:
                        AdicionarCabecera("Ejecuta la validación de variables.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N1_PARAMETROS");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        definicionProcedimiento = Repositorio.ObtenerDefinicionProcedimiento(baseDeDatos, esquema, $"P_B_PR_{proceso}_EN_V_N1_PARAMETROS", out mensajeError);
                        foreach (var linea in definicionProcedimiento)
                        {
                            ConstructorDeCodigo.AppendLine(linea);
                        }
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta la validación de existencias");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N1_EXISTENCIAS");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        definicionProcedimiento = Repositorio.ObtenerDefinicionProcedimiento(baseDeDatos, esquema, $"P_B_PR_{proceso}_EN_V_N1_EXISTENCIAS", out mensajeError);
                        foreach (var linea in definicionProcedimiento)
                        {
                            ConstructorDeCodigo.AppendLine(linea);
                        }
                        foreach (var procedimiento in procedimientosConValidacionExistencia.Where(x => x.ENSeleccionado && x.creado))
                        {
                            AdicionarEjecucionProcedimiento(procedimiento.procedimiento, new List<string> { procedimiento.parametro }, "BIT");
                        }
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta las validaciones en tablas no transaccionales.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N1");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        ConstructorDeCodigo.AppendLine($"    BEGIN");
                        ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                        ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                        ConstructorDeCodigo.AppendLine($"            BEGIN");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N1_PARAMETROS", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N1_EXISTENCIAS", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta la validación en tablas transaccionales.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N2_REGISTRABLE");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        definicionProcedimiento = Repositorio.ObtenerDefinicionProcedimiento(baseDeDatos, esquema, $"P_B_PR_{proceso}_EN_V_N2_REGISTRABLE", out mensajeError);
                        foreach (var linea in definicionProcedimiento)
                        {
                            ConstructorDeCodigo.AppendLine(linea);
                        }
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta las validaciones en tablas transaccionales.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_EN_V_N2");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas.Where(x => x.ENSeleccionado))
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("BIT", "BIT");
                        ConstructorDeCodigo.AppendLine($"    BEGIN");
                        ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                        ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                        ConstructorDeCodigo.AppendLine($"            BEGIN");
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N2_REGISTRABLE", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT");
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarProcedimientoObtencionEN(modificacionProceso.ProcedimientoObtencionEN, baseDeDatos, esquema, proceso, out mensajeError);
                        AdicionarCabecera("Ejecuta la operación del proceso.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_EN_R");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas)
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        AdicionarParametrosSalida("INT", "INT");
                        definicionProcedimiento = Repositorio.ObtenerDefinicionProcedimiento(baseDeDatos, esquema, $"P_B_PR_{proceso}_EN_R", out mensajeError);
                        foreach (var linea in definicionProcedimiento)
                        {
                            ConstructorDeCodigo.AppendLine(linea);
                        }
                        ConstructorDeCodigo.AppendLine($"GO");
                        AdicionarCabecera("Ejecuta la operación de un proceso.");
                        ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_EN");
                        AdicionarParametrosEntradaUsuarioCodigoSistema();
                        foreach (var columna in columnas)
                        {
                            ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                        }
                        if (modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Crear || modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Modificar)
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
                        if (modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Crear || modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Modificar)
                        {
                            ConstructorDeCodigo.AppendLine($"                DECLARE @V_RESULTADO_INT INT;");
                        }
                        if (columnas.Any(x => x.PRESeleccionado))
                        {
                            AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N1", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT", true);
                        }
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N1", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT", true);
                        if (columnas.Any(x => x.PRESeleccionado))
                        {
                            AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_V_N2", columnas.Where(x => x.PRESeleccionado).Select(x => x.Nombre), "BIT", true);
                        }
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N2", columnas.Where(x => x.ENSeleccionado).Select(x => x.Nombre), "BIT", true);
                        AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_R", columnas.Select(x => x.Nombre), "INT", modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Crear || modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Modificar);
                        if (modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Crear || modificacionProceso.ProcedimientoObtencionEN == OperacionModificacion.Modificar)
                        {
                            AdicionarEjecucionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_O", new List<string> { "@V_RESULTADO_INT" }, "JSON");
                        }
                        ConstructorDeCodigo.AppendLine($"        END;");
                        ConstructorDeCodigo.AppendLine($"    END;");
                        ConstructorDeCodigo.AppendLine($"GO");
                        break;
                    case OperacionModificacion.Eliminar:
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N1_PARAMETROS");
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N1_EXISTENCIAS");
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N1");
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N2_REGISTRABLE");
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_V_N2");
                        if (modificacionProceso.ProcedimientoObtencionPRE == OperacionModificacion.Eliminar)
                        {
                            AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_O");
                        }
                        AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN");
                        break;
                }
            }
        }

        private void AdicionarProcedimientoObtencionPRE(OperacionModificacion operacionModificacion, string baseDeDatos, string esquema, List<Parametro> columnas, string proceso, out string mensajeError)
        {
            mensajeError = "La operación no es correcta.";
            switch (operacionModificacion)
            {
                case OperacionModificacion.Crear:
                    AdicionarCabecera("Ejecuta la obtención de información después de las validaciones.");
                    ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_O");
                    AdicionarParametrosEntradaUsuarioCodigoSistema();
                    foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                    {
                        ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
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
                    break;
                case OperacionModificacion.Modificar:
                    AdicionarCabecera("Ejecuta la obtención de información después de las validaciones.");
                    ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_PRE_O");
                    AdicionarParametrosEntradaUsuarioCodigoSistema();
                    foreach (var columna in columnas.Where(x => x.PRESeleccionado))
                    {
                        ConstructorDeCodigo.AppendLine($"    {columna.Nombre} {columna.TipoDato},");
                    }
                    AdicionarParametrosSalida("JSON", "NVARCHAR(MAX)");
                    foreach (var linea in Repositorio.ObtenerDefinicionProcedimiento(baseDeDatos, esquema, $"P_B_PR_{proceso}_PRE_O", out mensajeError))
                    {
                        ConstructorDeCodigo.AppendLine(linea);
                    }
                    ConstructorDeCodigo.AppendLine($"GO");
                    break;
                case OperacionModificacion.Eliminar:
                    AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_PRE_O");
                    break;
            }
        }

        private void AdicionarProcedimientoObtencionEN(OperacionModificacion operacionModificacion, string baseDeDatos, string esquema, string proceso, out string mensajeError)
        {
            mensajeError = "La operación no es correcta.";
            switch (operacionModificacion)
            {
                case OperacionModificacion.Crear:
                    AdicionarCabecera("Ejecuta la obtención de información después de las validaciones.");
                    ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_PR_{proceso}_EN_O");
                    AdicionarParametrosEntradaUsuarioCodigoSistema();
                    ConstructorDeCodigo.AppendLine($"    @I_SECUENCIAL INT,");
                    AdicionarParametrosSalida("JSON", "NVARCHAR(MAX)");
                    ConstructorDeCodigo.AppendLine($"    BEGIN");
                    ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                    ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                    ConstructorDeCodigo.AppendLine($"            BEGIN");
                    AdicionarCuerpoProcedimientoPorDefecto();
                    ConstructorDeCodigo.AppendLine($"        END;");
                    ConstructorDeCodigo.AppendLine($"    END;");
                    ConstructorDeCodigo.AppendLine($"GO");
                    break;
                case OperacionModificacion.Modificar:
                    AdicionarCabecera("Ejecuta la obtención de información después de las validaciones.");
                    ConstructorDeCodigo.AppendLine($"ALTER PROCEDURE {esquema}.P_B_PR_{proceso}_EN_O");
                    AdicionarParametrosEntradaUsuarioCodigoSistema();
                    ConstructorDeCodigo.AppendLine($"    @I_SECUENCIAL INT,");
                    AdicionarParametrosSalida("JSON", "NVARCHAR(MAX)");
                    foreach (var linea in Repositorio.ObtenerDefinicionProcedimiento(baseDeDatos, esquema, $"P_B_PR_{proceso}_EN_O", out mensajeError))
                    {
                        ConstructorDeCodigo.AppendLine(linea);
                    }
                    ConstructorDeCodigo.AppendLine($"GO");
                    break;
                case OperacionModificacion.Eliminar:
                    AdicionarEliminacionProcedimiento($"{esquema}.P_B_PR_{proceso}_EN_O");
                    break;
            }
        }

    }
}
