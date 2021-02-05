namespace UVNetGeneradorDeCodigo.API
{
    using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using UVNetGeneradorDeCodigo.Menus;

    public class CExploradorObjetos
    {
        public CExploradorObjetos(AsyncPackage paquete)
        {
            this.paquete = paquete;
        }

        private readonly AsyncPackage paquete;
        private HierarchyObject menuTabla;
        private HierarchyObject menuProcedimiento;
        private HierarchyObject menuVista;
        private HierarchyObject menuCarpetaVistas;
        private HierarchyObject menuCarpetaTablas;
        private HierarchyObject menuBaseDeDatos;
        private const string RutaUrnTabla = "Server/Database/Table";
        private const string RutaUrnCarpetaTablas = "Server/Database/TablesFolder";
        private const string RutaUrnProcedimiento = "Server/Database/StoredProcedure";
        private const string RutaUrnVista = "Server/Database/View";
        private const string RutaUrnCarpetaVistas = "Server/Database/ViewsFolder";
        private const string RutaUrnBaseDeDatos = "Server/Database";

        public async System.Threading.Tasks.Task AsignarProveedorEventosAsync()
        {
            var informacionMetodo = GetType().GetMethod("VerificarCambioSeleccion", BindingFlags.NonPublic | BindingFlags.Instance);
            if (!(await paquete.GetServiceAsync(typeof(IObjectExplorerService)) is IObjectExplorerService exploradorObjetos))
            {
                return;
            }
            var dllExploradorObjetos = Assembly.Load("Microsoft.SqlServer.Management.SqlStudio.Explorer").GetType("Microsoft.SqlServer.Management.SqlStudio.Explorer.ObjectExplorerService");
            exploradorObjetos.GetSelectedNodes(out _, out _);
            var contendorExploradorObjetos = dllExploradorObjetos.GetProperty("Container", BindingFlags.Public | BindingFlags.Instance).GetValue(exploradorObjetos, null);
            object contextoServicio = null;
            if (contendorExploradorObjetos.GetType().GetProperty("Components", BindingFlags.Public | BindingFlags.Instance).GetValue(contendorExploradorObjetos, null) is ComponentCollection componentesExploradorObjetos)
            {
                foreach (Component componente in componentesExploradorObjetos)
                {
                    if (componente.GetType().FullName.Contains("ContextService"))
                    {
                        contextoServicio = componente;
                        break;
                    }
                }
            }
            if (contextoServicio == null)
            {
                throw new NullReferenceException("No se puede encontrar el explorador de objetos en el contexto del servicio.");
            }
            var contextoExploradorObjetos = contextoServicio.GetType().GetProperty("ObjectExplorerContext", BindingFlags.Public | BindingFlags.Instance).GetValue(contextoServicio, null);
            var instanciaEvento = contextoExploradorObjetos.GetType().GetEvent("CurrentContextChanged", BindingFlags.Public | BindingFlags.Instance);
            if (instanciaEvento == null)
            {
                return;
            }
            instanciaEvento.AddEventHandler(contextoExploradorObjetos, Delegate.CreateDelegate(instanciaEvento.EventHandlerType, this, informacionMetodo));
        }

        private void VerificarCambioSeleccion(object sender, NodesChangedEventArgs evento)
        {
            if (evento.ChangedNodes.Count <= 0)
            {
                return;
            }
            var nodo = evento.ChangedNodes[0];
            if (nodo == null)
            {
                return;
            }
            AdicionarMenuContextualATablas(nodo);
            AdicionarMenuContextualAProcedimientoAlmacenado(nodo);
            AdicionarMenuContextualAVista(nodo);
            AdicionarMenuContextualACarpetaVistas(nodo);
            AdicionarMenuContextualACarpetaTablas(nodo);
            AdicionarMenuContextualABaseDeDatos(nodo);
        }

        private void AdicionarMenuContextualATablas(INavigationContext nodo)
        {
            if (menuTabla == null && nodo.UrnPath == RutaUrnTabla)
            {
                menuTabla = (HierarchyObject)nodo.GetService(typeof(IMenuHandler));
                menuTabla.AddChild(string.Empty, new CTabla(paquete));
            }
        }

        private void AdicionarMenuContextualAProcedimientoAlmacenado(INavigationContext nodo)
        {
            if (menuProcedimiento == null && nodo.UrnPath == RutaUrnProcedimiento)
            {
                menuProcedimiento = (HierarchyObject)nodo.GetService(typeof(IMenuHandler));
                menuProcedimiento.AddChild(string.Empty, new CProcedimiento(paquete));
            }
        }

        private void AdicionarMenuContextualAVista(INavigationContext nodo)
        {
            if (menuVista == null && nodo.UrnPath == RutaUrnVista)
            {
                menuVista = (HierarchyObject)nodo.GetService(typeof(IMenuHandler));
                menuVista.AddChild(string.Empty, new CVista(paquete));
            }
        }

        private void AdicionarMenuContextualACarpetaVistas(INavigationContext nodo)
        {
            if (menuCarpetaVistas == null && nodo.UrnPath == RutaUrnCarpetaVistas)
            {
                menuCarpetaVistas = (HierarchyObject)nodo.GetService(typeof(IMenuHandler));
                menuCarpetaVistas.AddChild(string.Empty, new CCarpetaVistas(paquete));
            }
        }

        private void AdicionarMenuContextualACarpetaTablas(INavigationContext nodo)
        {
            if (menuCarpetaTablas == null && nodo.UrnPath == RutaUrnCarpetaTablas)
            {
                menuCarpetaTablas = (HierarchyObject)nodo.GetService(typeof(IMenuHandler));
                menuCarpetaTablas.AddChild(string.Empty, new CCarpetaTablas(paquete));
            }
        }

        private void AdicionarMenuContextualABaseDeDatos(INavigationContext nodo)
        {
            if (menuBaseDeDatos == null && nodo.UrnPath == RutaUrnBaseDeDatos)
            {
                menuBaseDeDatos = (HierarchyObject)nodo.GetService(typeof(IMenuHandler));
                menuBaseDeDatos.AddChild(string.Empty, new CBaseDeDatos(paquete));
            }
        }
    }
}
