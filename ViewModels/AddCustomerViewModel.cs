using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Client_Ranker.Data;
using Client_Ranker.Models;
using Microsoft.EntityFrameworkCore;

namespace Client_Ranker.ViewModels
{
    /// ViewModel encargado de gestionar la creación de un nuevo cliente en el sistema.
    /// Valida los datos ingresados y previene la duplicidad de documentos.
    public class AddCustomerViewModel
    {
        #region Dependencias y Delegados

        private readonly IDbContextFactory<AppDbContext>? _dbContextFactory;

        // Delegado inyectado para cerrar la ventana sin acoplar el ViewModel a WPF.
        private readonly Action _closeWindow;

        #endregion

        #region Propiedades Enlazadas (UI Bindings)

        public string DocumentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public decimal MonthlySpending { get; set; }

        #endregion

        #region Comandos

        public ICommand SaveCommand { get; set; }

        #endregion

        #region Constructor

        /// Inicializa el ViewModel de creación de cliente.
        /// <param name="closeWindow">Acción para cerrar la ventana actual.</param>
        /// <param name="dbContextFactory">Fábrica de contextos (DI).</param>
        public AddCustomerViewModel(Action closeWindow, IDbContextFactory<AppDbContext>? dbContextFactory = null)
        {
            _closeWindow = closeWindow;
            _dbContextFactory = dbContextFactory;

            SaveCommand = new RelayCommand(SaveCustomer);
        }

        private AppDbContext CreateContext()
        {
            return _dbContextFactory != null
                ? _dbContextFactory.CreateDbContext()
                : new AppDbContext();
        }

        #endregion

        #region Lógica de Negocio y Base de Datos

        /// Valida los campos ingresados, comprueba duplicados en la base de datos 
        /// y persiste al nuevo cliente.
        private void SaveCustomer()
        {
            // 1. Validaciones básicas de integridad de datos UI
            if (string.IsNullOrWhiteSpace(DocumentId) || string.IsNullOrWhiteSpace(FirstName))
            {
                MessageBox.Show("El documento y el nombre son campos obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = CreateContext();

            // 2. Validación de regla de negocio: No permitir DNI duplicados
            // Esto complementa el índice único en la BD para dar un mensaje amigable.
            if (context.Customers.Any(c => c.DocumentId == DocumentId))
            {
                MessageBox.Show("Ya existe un cliente registrado con este documento.", "Error de Duplicidad", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Mapeo a la entidad y establecimiento de valores iniciales
            var newCustomer = new Customer
            {
                DocumentId = this.DocumentId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                CurrentMonthSpending = this.MonthlySpending, // Si el cliente hace una compra al registrarse
                CurrentMonthVisits = 1,                      // Por defecto cuenta como la primera visita
                IsActive = true
            };

            // 4. Persistencia
            context.Customers.Add(newCustomer);
            context.SaveChanges();

            // 5. Cierre limpio de la ventana
            _closeWindow?.Invoke();
        }

        #endregion
    }
}
