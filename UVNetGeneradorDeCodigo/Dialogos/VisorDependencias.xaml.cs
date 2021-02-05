using Microsoft.VisualStudio.PlatformUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using UVNetGeneradorDeCodigo.API;
using UVNetGeneradorDeCodigo.Modelos;

namespace UVNetGeneradorDeCodigo.Dialogos
{
    /// <summary>
    /// Interaction logic for VisorDependencias.xaml
    /// </summary>
    public partial class VisorDependencias : DialogWindow
    {
        public ObservableCollection<Procedimiento> procedimientos;
        public VisorDependencias(List<CDependenciaProcedimiento> dependencias)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            var allNodes = dependencias.Select(x => new Procedimiento { Nombre = x.ObjetoDependiente, Padre = x.Objeto }).ToList();
            var lookup = allNodes.ToLookup(x => x.Padre);
            Title = "Visor de dependencias";
            foreach (var node in allNodes)
            {
                node.Procedimientos = new ObservableCollection<Procedimiento>(lookup[node.Nombre]);
            }
            procedimientos = new ObservableCollection<Procedimiento>(lookup[null]);
            InitializeComponent();
            tvDependencias.ItemsSource = procedimientos;
        }
    }

    public class Procedimiento : INotifyPropertyChanged
    {
        public string Nombre { get; set; }

        public string Padre { get; set; }

        public ObservableCollection<Procedimiento> Procedimientos { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
