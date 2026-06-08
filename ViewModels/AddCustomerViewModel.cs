using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Client_Ranker.Data;
using Client_Ranker.Models;

namespace Client_Ranker.ViewModels
{
    public class AddCustomerViewModel
    {
        // Propiedades enlazadas a los TextBox del formulario
        public string DocumentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public decimal MonthlySpending { get; set; }

        public ICommand SaveCommand { get; set; }

        // Un delegado para poder cerrar la ventana desde el ViewModel
        private Action _closeWindow;

        public AddCustomerViewModel(Action closeWindow)
        {
            _closeWindow = closeWindow;
            SaveCommand = new RelayCommand(SaveCustomer);
        }

        private void SaveCustomer()
        {
            // 1. Validaciones básicas
            if (string.IsNullOrWhiteSpace(DocumentId) || string.IsNullOrWhiteSpace(FirstName))
            {
                MessageBox.Show("El documento y el nombre son campos obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new AppDbContext();

            // 2. Verificar que no exista un documento duplicado
            if (context.Customers.Any(c => c.DocumentId == DocumentId))
            {
                MessageBox.Show("Ya existe un cliente registrado con este documento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Crear y guardar el cliente
            var newCustomer = new Customer
            {
                DocumentId = this.DocumentId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                CurrentMonthSpending = this.MonthlySpending // Guardamos directamente en el mes en curso
            };

            context.Customers.Add(newCustomer);
            context.SaveChanges();

            // 4. Cerramos la ventana una vez guardado
            _closeWindow();
        }
    }
}
