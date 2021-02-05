namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Collections.Generic;
    using System.Linq;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Dialogos;
    using UVNetGeneradorDeCodigo.Modelos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CProcedimientoValidacionExistenciasYParametros : CConstructorDeScriptsSQL
    {
        public CProcedimientoValidacionExistenciasYParametros(IRepositorio repositorio) : base(repositorio)
        {

        }

        public void GenerarProcedimientosValidaciones(string tabla, List<Columna> columnas, ETipoProcedimiento tipoProcedimiento, bool agrupar, out string mensajeError)
        {
            mensajeError = "Debe seleccionar por lo menos una columna";
            if (columnas.Any())
            {
                var esquema = Repositorio.ObtenerNombreEsquema(tabla, out mensajeError);
                if (agrupar)
                {
                    GenerarProcedimientoValidacionAgrupado(esquema, tabla, columnas, tipoProcedimiento, out mensajeError);
                }
                else
                {
                    if (tipoProcedimiento == ETipoProcedimiento.Parametros)
                    {
                        GenerarProcedimientoValidacionParametros(tabla, esquema, columnas, out mensajeError);
                    }
                    else
                    {
                        GenerarProcedimientoValidacionExistencias(tabla, esquema, columnas, out mensajeError);
                    }
                }
            }
        }

        private void GenerarProcedimientoValidacionAgrupado(string esquema, string tabla, List<Columna> columnas, ETipoProcedimiento tipoProcedimiento, out string mensajeError)
        {
            var listadoProcedimientos = Repositorio.ObtenerListaProcedimientos(out mensajeError);
            var columnasSeparadasPorGuionBajo = string.Join("_", columnas.Select(x => x.Nombre));
            var nombreProcedimiento = $"{esquema}.P_B_AT_{tabla.Substring(2)}_V_{(tipoProcedimiento == ETipoProcedimiento.Parametros ? "P" : "E")}_{columnasSeparadasPorGuionBajo}";
            if (!listadoProcedimientos.Contains(nombreProcedimiento))
            {
                var columnasSeparadasPorComa = string.Join(", ", columnas.Select(x => x.Nombre));
                var columnasSeparadasPorComaParaMensaje = string.Join(", ', ', ", columnas.Select(x => "@I_" + x.Nombre));
                AdicionarCabecera($"Ejecuta la validación {(tipoProcedimiento == ETipoProcedimiento.Parametros ? string.Empty : "de existencia")} {(columnas.Count == 1 ? "del parámetro" : "de los parámetros")} {columnasSeparadasPorComa}");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {nombreProcedimiento}");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas)
                {
                    ConstructorDeCodigo.AppendLine($"   @I_{columna.Nombre} {columna.TipoDato},");
                }
                AdicionarParametrosSalida("BIT", "BIT");
                ConstructorDeCodigo.AppendLine($"    BEGIN");
                ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                ConstructorDeCodigo.AppendLine($"            BEGIN");
                if (tipoProcedimiento == ETipoProcedimiento.Existencias)
                {
                    ConstructorDeCodigo.AppendLine($"                IF(EXISTS");
                    ConstructorDeCodigo.AppendLine($"                (");
                    ConstructorDeCodigo.AppendLine($"                    SELECT 1");
                    ConstructorDeCodigo.AppendLine($"                    FROM {esquema}.{tabla}");
                    ConstructorDeCodigo.AppendLine($"                    WHERE {columnas[0].Nombre} = @I_{columnas[0].Nombre}");
                    for (int i = 1; i < columnas.Count; i++)
                    {
                        ConstructorDeCodigo.AppendLine($"                          AND {columnas[i].Nombre} = @I_{columnas[i].Nombre}");
                    }

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
                    if (columnas.Count == 1)
                    {
                        ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = 'No se pudo obtener el registro.';");
                    }
                    else
                    {
                        ConstructorDeCodigo.AppendLine($"                        SET @O_MENSAJE = CONCAT('La combinación de parámetros [',{columnasSeparadasPorComaParaMensaje}, '] no existe.');");
                    }
                    ConstructorDeCodigo.AppendLine($"                        SET @O_CODIGO_ERROR = 0;");
                    ConstructorDeCodigo.AppendLine($"                END;");
                }
                else
                {
                    AdicionarCuerpoProcedimientoPorDefecto();
                }
                ConstructorDeCodigo.AppendLine($"        END");
                ConstructorDeCodigo.AppendLine($"    END");
                ConstructorDeCodigo.AppendLine($"GO");
            }
            else
            {
                AdicionarError($"El procedimiento {nombreProcedimiento} no pudo ser generado porque ya existe en la base de datos. {mensajeError}");
            }
        }

        private void GenerarProcedimientoValidacionParametros(string tabla, string esquema, List<Columna> columnas, out string mensajeError)
        {
            var listadoProcedimientos = Repositorio.ObtenerListaProcedimientos(out mensajeError);
            foreach (var columna in columnas)
            {
                var nombreProcedimiento = $"{esquema}.P_B_AT_{tabla.Substring(2)}_V_P_{columna.Nombre}";
                if (!listadoProcedimientos.Contains(nombreProcedimiento))
                {
                    AdicionarCabecera($"Ejecuta la validación del parámetro {columna.Nombre}");
                    ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {nombreProcedimiento}");
                    AdicionarParametrosEntradaUsuarioCodigoSistema();
                    ConstructorDeCodigo.AppendLine($"   @I_{columna.Nombre} {columna.TipoDato},");
                    AdicionarParametrosSalida("BIT", "BIT");
                    ConstructorDeCodigo.AppendLine($"    BEGIN");
                    ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                    ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                    ConstructorDeCodigo.AppendLine($"            BEGIN");
                    AdicionarCuerpoProcedimientoPorDefecto();
                    ConstructorDeCodigo.AppendLine($"        END");
                    ConstructorDeCodigo.AppendLine($"    END");
                    ConstructorDeCodigo.AppendLine($"GO");
                }
                else
                {
                    AdicionarError($"El procedimiento {nombreProcedimiento} no pudo ser generado porque ya existe en la base de datos. {mensajeError}");
                }
            }
        }

        private void GenerarProcedimientoValidacionExistencias(string tabla, string esquema, List<Columna> columnas, out string mensajeError)
        {
            var listadoProcedimientos = Repositorio.ObtenerListaProcedimientos(out mensajeError);
            foreach (var columna in columnas)
            {
                var nombreProcedimiento = $"{esquema}.P_B_AT_{tabla.Substring(2)}_V_E_{columna.Nombre}";
                if (!listadoProcedimientos.Contains(nombreProcedimiento))
                {
                    AdicionarCabecera($"Ejecuta la validación de existencia del parámetro {columna.Nombre}");
                    ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {nombreProcedimiento}");
                    AdicionarParametrosEntradaUsuarioCodigoSistema();
                    ConstructorDeCodigo.AppendLine($"   @I_{columna.Nombre} {columna.TipoDato},");
                    AdicionarParametrosSalida("BIT", "BIT");
                    ConstructorDeCodigo.AppendLine($"    BEGIN");
                    ConstructorDeCodigo.AppendLine($"        IF(@O_EXITO = 1");
                    ConstructorDeCodigo.AppendLine($"           OR @O_EXITO IS NULL)");
                    ConstructorDeCodigo.AppendLine($"            BEGIN");
                    ConstructorDeCodigo.AppendLine($"                IF(EXISTS");
                    ConstructorDeCodigo.AppendLine($"                (");
                    ConstructorDeCodigo.AppendLine($"                    SELECT 1");
                    ConstructorDeCodigo.AppendLine($"                    FROM {esquema}.{tabla}");
                    ConstructorDeCodigo.AppendLine($"                    WHERE {columna.Nombre} = @I_{columna.Nombre}");
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
        }
    }
}
