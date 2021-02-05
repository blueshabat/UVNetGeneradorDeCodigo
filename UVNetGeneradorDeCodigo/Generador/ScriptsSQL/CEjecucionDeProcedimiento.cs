namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CEjecucionDeProcedimiento : CConstructorDeScriptsSQL
    {
        public CEjecucionDeProcedimiento(IRepositorio repositorio) : base(repositorio, false)
        {
        }

        public void GenerarEjecucionDeProcedimiento(string esquema, string procedimiento, out string mensajeError)
        {
            var parametros = Repositorio.ObtenerParametrosProcedimiento($"{esquema}.{procedimiento}", out mensajeError);
            ConstructorDeCodigo.AppendLine($"EXEC {esquema}.{procedimiento}");
            foreach (var parametro in parametros)
            {
                ConstructorDeCodigo.AppendLine($"    {parametro.Nombre}{(parametro.EsParametroSalida ? " OUTPUT" : string.Empty)},");
            }
            EliminarUltimaComa();
            ConstructorDeCodigo.Append(";");
        }

        private void EliminarUltimaComa() => ConstructorDeCodigo.Length -= 3;
    }
}
