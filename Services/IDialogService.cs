using System;

namespace Client_Ranker.Services
{
    /// Interfaz para gestionar la apertura de ventanas en la aplicación sin acoplar los ViewModels a WPF.
    public interface IDialogService
    {
        void OpenAddCustomer();
        void OpenEditCustomer(int customerId);
        void OpenAddPurchase(int customerId);
        void OpenSettings(Action onSaveSuccess);
        void OpenDashboard();
        bool OpenLoginSupervisor();
    }
}