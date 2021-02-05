namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System.Collections.Generic;
    using System.Linq;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CProcedimientoValidacionRegistro : CConstructorDeScriptsSQL
    {
        public CProcedimientoValidacionRegistro(IRepositorio repositorio) : base(repositorio)
        {
        }

        public void GenerarProcedimientoDeValidacionDeRegistro(string esquema, string tabla, out string mensajeError)
        {
            var columnas = Repositorio.ObtenerColumnas($"{esquema}.{tabla}", out mensajeError).ExcluirAuditoria().ExcluirSecuencial()
                .ExcluirColumnas(new List<string> { "ID_PARTICION", "OBSERVACION", "SISTEMA_ORIGEN" });
            if (columnas.Any())
            {
                AdicionarCabecera("Valida que la información pueda ser registrada");
                ConstructorDeCodigo.AppendLine($"CREATE PROCEDURE {esquema}.P_B_AT_{tabla.Substring(2)}_V_R ");
                AdicionarParametrosEntradaUsuarioCodigoSistema();
                foreach (var columna in columnas)
                {
                    ConstructorDeCodigo.AppendLine($"  @I_{columna.Nombre} {columna.TipoDato},");
                }
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
        }
    }
}
