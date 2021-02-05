namespace UVNetGeneradorDeCodigo.Modelos
{
    public class CParametroProcedimiento : CParametro
    {
        public bool EsParametroSalida { get; set; }

        public string TipoDeDatoCSharp { get; set; }

        public string TipoDeDatoClaseCSharp { get; set; }
    }
}
