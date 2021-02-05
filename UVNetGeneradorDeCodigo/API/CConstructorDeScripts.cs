namespace UVNetGeneradorDeCodigo.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UVNetGeneradorDeCodigo.Modelos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public abstract class CConstructorDeCodigo
    {
        public CConstructorDeCodigo(IRepositorio repositorio)
        {
            Repositorio = repositorio;
            ConstructorDeCodigo = new StringBuilder(CapacidadConstructorCadenas);
        }

        protected readonly IRepositorio Repositorio;
        protected StringBuilder ConstructorDeCodigo;
        private const int CapacidadConstructorCadenas = 10485760;

        public string CodigoGenerado
        {
            get
            {
                var codigoGenerado = ConstructorDeCodigo.ToString();
                ConstructorDeCodigo.Clear();
                return codigoGenerado;
            }
        }
    }

    public class CConstructorDeCodigoCSharp : CConstructorDeCodigo
    {
        public CConstructorDeCodigoCSharp(IRepositorio repositorio) : base(repositorio)
        {
        }
    }

    public class CConstructorDeScriptsSQL : CConstructorDeCodigo
    {
        public CConstructorDeScriptsSQL(IRepositorio repositorio, bool adicionarInicioDeScript = true) : base(repositorio)
        {
            Usuario = repositorio.ObtenerNombreUsuario(out _);
            BaseDeDatos = repositorio.ObtenerNombreBaseDatos(out _);
            if (adicionarInicioDeScript)
            {
                AdicionarInicioScript();
            }
        }

        public CConstructorDeScriptsSQL(IRepositorio repositorio) : base(repositorio)
        {
            Usuario = repositorio.ObtenerNombreUsuario(out _);
            BaseDeDatos = repositorio.ObtenerNombreBaseDatos(out _);
            AdicionarInicioScript();
        }

        protected readonly string Usuario;
        protected readonly string BaseDeDatos;

        protected void AdicionarInicioScript()
        {
            ConstructorDeCodigo.AppendLine($"USE {BaseDeDatos};");
            ConstructorDeCodigo.AppendLine($"GO");
            ConstructorDeCodigo.AppendLine();
        }

        protected void AdicionarCabecera(string descripcion)
        {
            AdicionarSeparadorDeSeccion();
            ConstructorDeCodigo.AppendLine($"-- Datos creación:");
            ConstructorDeCodigo.AppendLine($"-- Autor: {Usuario}");
            ConstructorDeCodigo.AppendLine($"-- Descripción:	{descripcion}");
            ConstructorDeCodigo.AppendLine($"-- Nro. Pase:	");
            AdicionarSeparadorDeSeccion();
            ConstructorDeCodigo.AppendLine();
        }

        protected void AdicionarCabeceraModificacion(string descripcion, string numeroPase, string numeroTicketMesaDeAyuda)
        {

        }

        protected void AdicionarParametrosEntradaUsuarioCodigoSistema()
        {
            ConstructorDeCodigo.AppendLine($"    @I_USUARIO_AUT NVARCHAR(100),");
            ConstructorDeCodigo.AppendLine($"    @I_CODIGO_SISTEMA NVARCHAR(20),");
            AdicionarSeparadorSimple();
        }

        protected void AdicionarSeparadorSimple() => ConstructorDeCodigo.AppendLine($"    ---");
        protected void AdicionarSeparadorDeSeccion() => ConstructorDeCodigo.AppendLine($"-- =============================================");

        protected void AdicionarParametrosSalida(string tipoDatoNombre, string tipoDatoTipo)
        {
            AdicionarSeparadorSimple();
            ConstructorDeCodigo.AppendLine($"    @O_RESULTADO_{tipoDatoNombre}  {tipoDatoTipo} OUTPUT, ");
            AdicionarParametrosSalidaSinResultado();
        }

        protected void AdicionarParametrosSalidaSinResultado()
        {
            ConstructorDeCodigo.AppendLine($"    @O_EXITO          BIT OUTPUT, ");
            ConstructorDeCodigo.AppendLine($"    @O_MENSAJE        NVARCHAR(4000) OUTPUT, ");
            ConstructorDeCodigo.AppendLine($"    @O_CODIGO_ERROR   INT OUTPUT");
            ConstructorDeCodigo.AppendLine($"AS");
        }

        protected void AdicionarEjecucionProcedimiento(string nombreProcedimiento, IEnumerable<string> parametrosEntrada, string tipoDatoResultado, bool resultadoEnVariable = false, string nombreVariableResultado = "RESULTADO")
        {
            ConstructorDeCodigo.AppendLine($"                EXEC {nombreProcedimiento}");
            ConstructorDeCodigo.AppendLine($"                     @I_USUARIO_AUT, ");
            ConstructorDeCodigo.AppendLine($"                     @I_CODIGO_SISTEMA, ");
            foreach (var parametro in parametrosEntrada)
            {
                ConstructorDeCodigo.AppendLine($"                     {parametro},");
            }
            ConstructorDeCodigo.AppendLine($"                     @{(resultadoEnVariable ? "V" : "O")}_{nombreVariableResultado}{(string.IsNullOrEmpty(tipoDatoResultado) ? "" : "_" + tipoDatoResultado)} OUTPUT, ");
            ConstructorDeCodigo.AppendLine($"                     @O_EXITO OUTPUT, ");
            ConstructorDeCodigo.AppendLine($"                     @O_MENSAJE OUTPUT, ");
            ConstructorDeCodigo.AppendLine($"                     @O_CODIGO_ERROR OUTPUT;");
        }

        protected void AdicionarCuerpoProcedimientoPorDefecto()
        {
            ConstructorDeCodigo.AppendLine($"                SET @O_EXITO = @O_EXITO;");
            ConstructorDeCodigo.AppendLine($"                --Definición del procedimiento (Eliminar la línea precedente, una vez definido el procedimiento)");
        }

        protected void AdicionarEliminacionProcedimiento(string procedimiento)
        {
            ConstructorDeCodigo.AppendLine();
            AdicionarSeparadorDeSeccion();
            ConstructorDeCodigo.AppendLine($"-- Eliminación del procedimiento {procedimiento}");
            AdicionarSeparadorDeSeccion();
            ConstructorDeCodigo.AppendLine();
            ConstructorDeCodigo.AppendLine($"DROP PROCEDURE {procedimiento}");
            ConstructorDeCodigo.AppendLine();
            ConstructorDeCodigo.AppendLine($"GO");
        }

        protected void AdicionarError(string mensajeError)
        {
            ConstructorDeCodigo.AppendLine($"-- {mensajeError}");
            ConstructorDeCodigo.AppendLine();
        }

        protected void AdicionarCreacionDeEsquema(string esquema)
        {
            if (!Repositorio.ExisteEsquema(esquema, out _))
            {
                ConstructorDeCodigo.AppendLine($"CREATE SCHEMA {esquema};");
                ConstructorDeCodigo.AppendLine("GO");
            }
        }

        protected void AdicionarCreacionDeTabla(string esquema, string tabla, List<CColumnaTabla> columnas, bool secuencialIdentidad = false)
        {
            ConstructorDeCodigo.AppendLine($"CREATE TABLE {esquema}.{tabla}");
            ConstructorDeCodigo.AppendLine($"(SECUENCIAL INT {(secuencialIdentidad ? "IDENTITY(1, 1) " : " ")}NOT NULL,");
            ConstructorDeCodigo.AppendLine($" ADICIONADO_POR NVARCHAR(50) NOT NULL,");
            ConstructorDeCodigo.AppendLine($" FECHA_ADICION DATETIME2(7) NOT NULL,");
            ConstructorDeCodigo.AppendLine($" MODIFICADO_POR NVARCHAR(50) NULL,");
            ConstructorDeCodigo.AppendLine($" FECHA_MODIFICACION DATETIME2(7) NULL,");
            foreach (var columna in columnas)
            {
                ConstructorDeCodigo.AppendLine($" {columna.Nombre} {columna.TipoDato} {(columna.AdmiteValoresNulos ? "NULL" : "NOT NULL")} {(columna.TieneValorPorDefecto ? "DEFAULT " + columna.ValorPorDefecto : "")},");
            }
            ConstructorDeCodigo.Append($" CONSTRAINT PK_{tabla} PRIMARY KEY(SECUENCIAL)");
            var columnasConLlaveForanea = columnas.Where(x => x.EsLlaveForanea);
            if (columnasConLlaveForanea.Any())
            {
                foreach (var columna in columnas.Where(x => x.EsLlaveForanea))
                {
                    ConstructorDeCodigo.AppendLine(",");
                    ConstructorDeCodigo.Append($" CONSTRAINT FK_{tabla}_{columna.Referencia.Tabla}{columna.Referencia.Sufijo} FOREIGN KEY({columna.Nombre}) REFERENCES {columna.Referencia}");
                }
            }
            else
            {
                ConstructorDeCodigo.AppendLine();
            }
            ConstructorDeCodigo.AppendLine($");");
            ConstructorDeCodigo.AppendLine($"GO");
        }
    }
}
