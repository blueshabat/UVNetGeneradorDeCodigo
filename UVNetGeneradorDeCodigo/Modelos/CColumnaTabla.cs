namespace UVNetGeneradorDeCodigo.Modelos
{
    public class CColumnaTabla
    {
        public CColumnaTabla()
        {

        }

        public CColumnaTabla(string nombre, string tipoDato, bool tieneValorPorDefecto = false, string valorPorDefecto = "", bool admiteValoresNulos = true, bool esLlaveForanea = false, CReferencia referencia = null)
        {
            Nombre = nombre;
            TipoDato = tipoDato;
            TieneValorPorDefecto = tieneValorPorDefecto;
            ValorPorDefecto = valorPorDefecto;
            AdmiteValoresNulos = admiteValoresNulos;
            EsLlaveForanea = esLlaveForanea;
            Referencia = referencia;
        }

        public string Nombre { get; set; }

        public string TipoDato { get; set; }

        public int TamañoMaximo { get; set; }

        public int Presicion { get; set; }

        public int Escala { get; set; }

        public bool EsIdentidad { get; set; }

        public bool TieneValorPorDefecto { get; set; }

        public string ValorPorDefecto { get; set; }

        public bool AdmiteValoresNulos { get; set; } = true;

        public bool EsLlaveForanea { get; set; }

        public CReferencia Referencia { get; set; }
    }

    public class CReferencia
    {
        public CReferencia(string esquema, string tabla, string sufijo = null, string campo = "SECUENCIAL")
        {
            Esquema = esquema;
            Tabla = tabla;
            Campo = campo;
            Sufijo = sufijo != null ? "_" + sufijo : sufijo;
        }

        public string Esquema { get; set; }

        public string Tabla { get; set; }

        public string Campo { get; set; }

        public string Sufijo { get; set; }

        public override string ToString()
        {
            return $"{Esquema}.{Tabla}({Campo})";
        }
    }
}
