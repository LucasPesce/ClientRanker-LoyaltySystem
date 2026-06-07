using Client_Ranker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client_Ranker.ViewModels
{
    public class AddPurchaseViewModel
    {
        private readonly int _customerId;
        private Action _closeWindow;

        // El monto nuevo que el cajero tipea
        public decimal PurchaseAmount { get; set; }

        public ICommand SavePurchaseCommand { get; set; }

        public AddPurchaseViewModel(int customerId, Action closeWindow)
        {
            _customerId = customerId;
            _closeWindow = closeWindow;
            SavePurchaseCommand = new RelayCommand(SavePurchase);
        }

        private void SavePurchase()
        {
            if (PurchaseAmount <= 0)
            {
                MessageBox.Show("El monto debe ser mayor a cero. No somos una obra de caridad.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new AppDbContext();
            var customer = context.Customers.Find(_customerId);

            if (customer != null)
            {
                customer.CurrentMonthSpending += PurchaseAmount;
                context.SaveChanges();
                _closeWindow();
            }
        }
    }
}