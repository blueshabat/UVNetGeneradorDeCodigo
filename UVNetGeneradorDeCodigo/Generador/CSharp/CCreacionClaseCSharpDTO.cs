namespace UVNetGeneradorDeCodigo.Generador.CSharp
{
    using System.Globalization;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CCreacionClaseCSharpDTO : CConstructorDeCodigoCSharp
    {
        public CCreacionClaseCSharpDTO(IRepositorio repositorio) : base(repositorio)
        {
            informacionDeTexto = new CultureInfo("en-us", false).TextInfo;
        }

        private readonly TextInfo informacionDeTexto;


        private string ConvertirATitleCase(string cadena) => informacionDeTexto.ToTitleCase(cadena.ToLower()).Replace("_", string.Empty);
    }
}
