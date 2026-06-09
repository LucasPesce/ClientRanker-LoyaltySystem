using Client_Ranker.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Client_Ranker.ViewModels
{
    /// ViewModel para la modificación de los datos personales de un cliente existente.
    public class EditCustomerViewModel
    {
        #region Dependencias y Delegados

        private readonly IDbContextFactory<AppDbContext>? _dbContextFactory;
        private readonly int _customerId;
        private readonly Action _closeWindow;

        #endregion

        #region Propiedades Enlazadas (UI Bindings)

        public string DocumentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        #endregion

        #region Comandos

        public ICommand UpdateCommand { get; set; }

        #endregion

        #region Constructor e Inicialización

        /// Inicializa el ViewModel de edición y carga los datos actuales del cliente.
        /// <param name="customerId">ID único del cliente a editar.</param>
        /// <param name="closeWindow">Acción para cerrar la ventana.</param>
        /// <param name="dbContextFactory">Fábrica de contextos (DI).</param>
        public EditCustomerViewModel(int customerId, Action closeWindow, IDbContextFactory<AppDbContext>? dbContextFactory = null)
        {
            _customerId = customerId;
            _closeWindow = closeWindow;
            _dbContextFactory = dbContextFactory;

            UpdateCommand = new RelayCommand(UpdateCustomer);

            LoadCustomerData();
        }

        private AppDbContext CreateContext()
        {
            return _dbContextFactory != null
                ? _dbContextFactory.CreateDbContext()
                : new AppDbContext();
        }

        #endregion

        #region Lógica de Base de Datos

        /// Obtiene los datos del cliente desde la base de datos para poblar los campos del formulario.
        private void LoadCustomerData()
        {
            using var context = CreateContext();
            var customer = context.Customers.Find(_customerId);

            if (customer != null)
            {
                DocumentId = customer.DocumentId;
                FirstName = customer.FirstName;
                LastName = customer.LastName;
            }
        }

        /// Valida los cambios introducidos e intenta guardarlos en la base de datos.
        private void UpdateCustomer()
        {
            // 1. Validaciones básicas
            if (string.IsNullOrWhiteSpace(DocumentId) || string.IsNullOrWhiteSpace(FirstName))
            {
                MessageBox.Show("El documento y el nombre no pueden estar vacíos.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = CreateContext();
            var customer = context.Customers.Find(_customerId);

            if (customer != null)
            {
                // 2. Validación de Colisión de DNI: 
                // Aseguramos que el documento que intenta guardar no pertenezca a OTRO cliente distinto.
                bool isDniTaken = context.Customers.Any(c => c.DocumentId == DocumentId && c.Id != _customerId);

                if (isDniTaken)
                {
                    MessageBox.Show("Ese documento ya pertenece a otro cliente registrado en el sistema.", "Error de Duplicidad", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 3. Aplicación de cambios a la entidad rastreada
                customer.DocumentId = DocumentId;
                customer.FirstName = FirstName;
                customer.LastName = LastName;

                // 4. Persistencia
                context.SaveChanges();

                // 5. Cierre limpio
                _closeWindow?.Invoke();
            }
        }

        #endregion
    }
}