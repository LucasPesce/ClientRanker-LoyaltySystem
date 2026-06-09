using Client_Ranker.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Input;

namespace Client_Ranker.ViewModels
{
    /// ViewModel encargado de registrar una nueva compra/consumo a un cliente existente.
    /// Incrementa el gasto acumulado del mes y el contador de visitas.
    public class AddPurchaseViewModel
    {
        #region Dependencias y Estado Interno

        private readonly IDbContextFactory<AppDbContext>? _dbContextFactory;

        /// ID del cliente al que se le aplicará la compra.
        private readonly int _customerId;

        /// Delegado para cerrar la ventana desde el ViewModel.
        private readonly Action _closeWindow;

        #endregion

        #region Propiedades Enlazadas (UI Bindings)

        /// El monto total de la nueva compra que el cajero digita en la interfaz.
        public decimal PurchaseAmount { get; set; }

        #endregion

        #region Comandos

        public ICommand SavePurchaseCommand { get; set; }

        #endregion

        #region Constructor e Inicialización

        /// Inicializa el ViewModel para agregar una compra.
        /// <param name="customerId">ID del cliente seleccionado en la tabla principal.</param>
        /// <param name="closeWindow">Acción para cerrar la ventana tras guardar.</param>
        /// <param name="dbContextFactory">Fábrica de contextos (DI).</param>
        public AddPurchaseViewModel(int customerId, Action closeWindow, IDbContextFactory<AppDbContext>? dbContextFactory = null)
        {
            _customerId = customerId;
            _closeWindow = closeWindow;
            _dbContextFactory = dbContextFactory;

            SavePurchaseCommand = new RelayCommand(SavePurchase);
        }

        private AppDbContext CreateContext()
        {
            return _dbContextFactory != null
                ? _dbContextFactory.CreateDbContext()
                : new AppDbContext();
        }

        #endregion

        #region Lógica de Negocio y Base de Datos

        /// Valida el monto ingresado, recupera al cliente de la base de datos 
        /// y suma los valores al acumulado del mes en curso.
        private void SavePurchase()
        {
            // 1. Regla de Negocio: No se pueden cargar compras en cero o en negativo.
            if (PurchaseAmount <= 0)
            {
                // Un pequeño toque de humor corporativo en el mensaje, muy común en apps internas ;)
                MessageBox.Show("El monto debe ser mayor a cero. No somos una obra de caridad.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = CreateContext();
            var customer = context.Customers.Find(_customerId);

            if (customer != null)
            {
                // 2. Aplicación de la transacción (Incremento en memoria)
                customer.CurrentMonthSpending += PurchaseAmount;
                customer.CurrentMonthVisits++;

                // 3. Persistencia en la Base de Datos
                context.SaveChanges();

                // 4. Cierre de ventana exitoso
                _closeWindow?.Invoke();
            }
            else
            {
                // Manejo de caso extremo: si el cajero tuviera la ventana abierta y otro usuario borrara el cliente.
                MessageBox.Show("No se encontró el cliente en la base de datos.", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}