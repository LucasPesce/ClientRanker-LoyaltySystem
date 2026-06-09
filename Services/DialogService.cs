using System;
using Microsoft.Extensions.DependencyInjection;
using Client_Ranker.View;

namespace Client_Ranker.Services
{
    /// Implementación del servicio de navegación que utiliza el proveedor de servicios
    /// para instanciar vistas con todas sus dependencias inyectadas.
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OpenAddCustomer()
        {
            var window = ActivatorUtilities.CreateInstance<AddCustomerWindow>(_serviceProvider);
            window.ShowDialog();
        }

        public void OpenEditCustomer(int customerId)
        {
            // ActivatorUtilities nos permite inyectar parámetros de tiempo de ejecución (customerId)
            // junto con los servicios registrados en el contenedor de dependencias (IDbContextFactory).
            var window = ActivatorUtilities.CreateInstance<EditCustomerWindow>(_serviceProvider, customerId);
            window.ShowDialog();
        }

        public void OpenAddPurchase(int customerId)
        {
            var window = ActivatorUtilities.CreateInstance<AddPurchaseWindow>(_serviceProvider, customerId);
            window.ShowDialog();
        }

        public void OpenSettings(Action onSaveSuccess)
        {
            var window = ActivatorUtilities.CreateInstance<SettingsWindow>(_serviceProvider, onSaveSuccess);
            window.ShowDialog();
        }

        public void OpenDashboard()
        {
            var window = ActivatorUtilities.CreateInstance<DashboardWindow>(_serviceProvider);
            window.ShowDialog();
        }

        public bool OpenLoginSupervisor()
        {
            var window = ActivatorUtilities.CreateInstance<LoginSupervisorWindow>(_serviceProvider);
            window.ShowDialog();
            return window.IsAuthorized;
        }
    }
}