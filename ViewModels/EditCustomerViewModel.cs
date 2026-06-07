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
    public class EditCustomerViewModel
    {
        private readonly int _customerId;
        private Action _closeWindow;

        public string DocumentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICommand UpdateCommand { get; set; }

        public EditCustomerViewModel(int customerId, Action closeWindow)
        {
            _customerId = customerId;
            _closeWindow = closeWindow;
            UpdateCommand = new RelayCommand(UpdateCustomer);

            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            using var context = new AppDbContext();
            var customer = context.Customers.Find(_customerId);
            if (customer != null)
            {
                DocumentId = customer.DocumentId;
                FirstName = customer.FirstName;
                LastName = customer.LastName;
            }
        }

        private void UpdateCustomer()
        {
            if (string.IsNullOrWhiteSpace(DocumentId) || string.IsNullOrWhiteSpace(FirstName))
            {
                MessageBox.Show("El documento y el nombre no pueden estar vacíos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new AppDbContext();
            var customer = context.Customers.Find(_customerId);

            if (customer != null)
            {
                // Verificar que no se duplique el DNI con otro cliente
                if (context.Customers.Any(c => c.DocumentId == DocumentId && c.Id != _customerId))
                {
                    MessageBox.Show("Ese documento ya pertenece a otro cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                customer.DocumentId = DocumentId;
                customer.FirstName = FirstName;
                customer.LastName = LastName;

                context.SaveChanges();
                MessageBox.Show("Datos actualizados correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                _closeWindow();
            }
        }
    }
}