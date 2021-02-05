using System.Collections.Generic;

namespace UVNetGeneradorDeCodigo.Modelos
{
    public class CDependenciaProcedimiento
    {
        public string Objeto { get; set; }

        public string ObjetoDependiente { get; set; }

        public int Nivel { get; set; }
    }
}
