namespace UVNetGeneradorDeCodigo.Generador.ScriptsSQL
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using UVNetGeneradorDeCodigo.API;
    using UVNetGeneradorDeCodigo.Modelos;
    using UVNetGeneradorDeCodigo.Repositorio;

    public class CCreacionVista : CConstructorDeScriptsSQL
    {
        public CCreacionVista(IRepositorio repositorio) : base(repositorio)
        {
            informacionDeTexto = new CultureInfo("en-us", false).TextInfo;
            tablasForaneas = new List<(string, string, string, string)>();
            aliasUtilizados = new List<(string, string)>();
            errores = new List<string>();
        }

        private readonly TextInfo informacionDeTexto;
        private readonly List<(string esquema, string nombre, string alias, string columna)> tablasForaneas;
        private readonly List<(string real, string utilizable)> aliasUtilizados;
        private readonly List<string> errores;
        private string prefijoEntidad;

        public void GenerarCreacionVista(string esquema, string tabla, out string mensajeError)
        {
            var entidad = tabla.Substring(2);
            var alias = ObtenerAlias(tabla);
            tablasForaneas.Add((esquema, tabla, alias, "SECUENCIAL"));
            prefijoEntidad = string.Join(string.Empty, entidad.Split('_').Select(x => ConvertirATitleCase(x.Substring(0, 3))));
            var columnas = Repositorio.ObtenerColumnas($"{esquema}.{tabla}", out mensajeError).ExcluirAuditoria().ExcluirControlBD();
            AdicionarCabecera($"Vista de la tabla {esquema}.{tabla}");
            ConstructorDeCodigo.AppendLine($"CREATE VIEW {esquema}.V_{entidad}");
            ConstructorDeCodigo.AppendLine($"AS");
            ConstructorDeCodigo.AppendLine($"   SELECT");
            ConstruirTablasForaneas(columnas);
            foreach (var columna in columnas.Where(x => !x.Nombre.Contains("_FK")))
            {
                ConstructorDeCodigo.AppendLine($"       {alias}.{columna.Nombre} AS '{prefijoEntidad}{ConvertirATitleCase(columna.Nombre)}',");
            }
            foreach (var columna in columnas.Where(x => x.Nombre.Contains("_FK")))
            {
                ConstructorDeCodigo.AppendLine("       ---");
                ConstructorDeCodigo.AppendLine($"       {alias}.{columna.Nombre} AS '{prefijoEntidad}{ConvertirATitleCase(columna.Nombre)}',");
                var tablaReferenciada = ObtenerNombreTablaAPartirDeNombreColumna(columna.Nombre);
                if (tablaReferenciada != tabla)
                {
                    AdicionarColumnas(tablaReferenciada, columna.Nombre);
                }
            }
            ConstructorDeCodigo.Length -= 3;
            ConstructorDeCodigo.AppendLine();
            ConstructorDeCodigo.AppendLine($"   FROM {esquema}.{tabla} AS {alias}");
            foreach (var tablaForanea in tablasForaneas.Where(x => x.nombre != tabla))
            {
                ConstructorDeCodigo.AppendLine($"   JOIN {tablaForanea.esquema}.{tablaForanea.nombre} AS {tablaForanea.alias} ON {alias}.{tablaForanea.columna} = {tablaForanea.alias}.SECUENCIAL");
            }
            ConstructorDeCodigo.AppendLine("GO");
            foreach (var error in errores)
            {
                ConstructorDeCodigo.AppendLine($"-- {error}");
            }
        }

        private string ConvertirATitleCase(string cadena) => informacionDeTexto.ToTitleCase(cadena.ToLower()).Replace("_", string.Empty);

        private void ConstruirTablasForaneas(List<CColumnaTabla> columnas)
        {
            foreach (var columna in columnas.Where(x => x.Nombre.Contains("_FK")))
            {
                var tabla = ObtenerNombreTablaAPartirDeNombreColumna(columna.Nombre);
                var esquema = Repositorio.ObtenerNombreEsquema(tabla, out string mensajeError);
                if (!string.IsNullOrEmpty(esquema))
                {
                    if (esquema == "PARAMETRICAS")
                    {
                        tablasForaneas.Add((esquema, tabla, ObtenerAlias(tabla), columna.Nombre));
                    }
                }
                else
                {
                    errores.Add($"La tabla {tabla} no existe.");
                }
            }
        }

        private string ObtenerNombreTablaAPartirDeNombreColumna(string nombreColumna) => nombreColumna.Substring(0, nombreColumna.IndexOf("_FK", StringComparison.Ordinal));

        private string ObtenerAlias(string tabla)
        {
            var aliasReal = string.Join(string.Empty, tabla.Split('_').Select(x => x.Substring(0, 1)));
            var aliasUtilizable = aliasReal + (aliasUtilizados.Any(x => x.real == aliasReal) ? aliasUtilizados.Count(x => x.real == aliasReal).ToString() : string.Empty);
            aliasUtilizados.Add((aliasReal, aliasUtilizable));
            return aliasUtilizable;
        }

        private void AdicionarColumnas(string tabla, string tablaColumna)
        {
            if (tablasForaneas.Any(x => x.nombre == tabla))
            {
                var esquema = tablasForaneas.Where(x => x.nombre == tabla).FirstOrDefault().esquema;
                var columnas = Repositorio.ObtenerColumnas($"{esquema}.{tabla}", out string mensajeError).ExcluirAuditoria().ExcluirControlBD().ExcluirSecuencial().Where(x => x.Nombre != "VISIBLE");
                if (string.IsNullOrEmpty(mensajeError))
                {
                    foreach (var columna in columnas)
                    {
                        var (alias, prefijoTablaPadre) = tablasForaneas.Where(x => x.columna == tablaColumna).Select(x => (x.alias, ObtenerPrefijoTablaPadre(x.columna))).FirstOrDefault();
                        ConstructorDeCodigo.AppendLine($"       {alias}.{columna.Nombre} AS '{prefijoEntidad}{prefijoTablaPadre}{ConvertirATitleCase(columna.Nombre)}',");
                    }
                }
                else
                {
                    errores.Add(mensajeError);
                }
            }
        }

        private string ObtenerPrefijoTablaPadre(string nombreColumna)
        {
            return ConvertirATitleCase(nombreColumna.Replace("_FK", string.Empty));
        }
    }
}
