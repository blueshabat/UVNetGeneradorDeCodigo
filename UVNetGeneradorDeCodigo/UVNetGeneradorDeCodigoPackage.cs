using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using UVNetGeneradorDeCodigo.API;
using Task = System.Threading.Tasks.Task;

namespace UVNetGeneradorDeCodigo
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class UVNetGeneradorDeCodigoPackage : AsyncPackage
    {
        public const string PackageGuidString = "ddcfd77e-757a-456a-96fc-e3abef669250";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await new CExploradorObjetos(this).AsignarProveedorEventosAsync();
            await Comandos.CInicializador.InitializeAsync(this);
        }
    }
}
