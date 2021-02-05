namespace UVNetGeneradorDeCodigo.Menus.Elementos.ProcedimientoAlmacenado
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using UVNetGeneradorDeCodigo.Dialogos;
    using UVNetGeneradorDeCodigo.Modelos;

    public class CVisorDeDependencias : CElementoMenu
    {
        public CVisorDeDependencias(INodeInformation elementoSeleccionado) : base(elementoSeleccionado)
        {
        }

        protected override string Titulo => "Ver dependencias";

        protected override void Accion(EventArgs evento)
        {
            var esquema = Repositorio.ObtenerNombreEsquema(ElementoSeleccionado.Name, out string mensajeError);
            var dependencias = Repositorio.ObtenerProdimientosDependientes($"{esquema}.{ElementoSeleccionado.Name}", out mensajeError);
            dependencias.Insert(0, new CDependenciaProcedimiento { Objeto = null, ObjetoDependiente = $"{esquema}.{ElementoSeleccionado.Name}", Nivel = 0 });
            var allNodes = dependencias.Select(x => new Dependencia { Procedimiento = x }).ToList();
            var lookup = allNodes.ToLookup(x => x.Procedimiento.Objeto);
            foreach (var node in allNodes)
                node.Dependencias = lookup[node.Procedimiento.ObjetoDependiente];
            var a = lookup[null];
            VisorDependencias dialogo = new VisorDependencias(dependencias);
            if (string.IsNullOrEmpty(mensajeError))
            {
                dialogo.ShowModal();
            }
            else
            {
                MessageBox.Show(mensajeError);
            }
        }
    }

    public class Dependencia
    {
        public CDependenciaProcedimiento Procedimiento { get; set; }

        public IEnumerable<Dependencia> Dependencias { get; set; }
    }
}
