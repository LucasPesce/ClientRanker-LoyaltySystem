using Client_Ranker.Data;
using Client_Ranker.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Client_Ranker.ViewModels
{
    public class MainViewModel
    {
        private int _selectedPeriodIndex = 0;

        // Lista mapeada que la UI va a renderizar
        public ObservableCollection<CustomerRow> CustomersDisplay { get; set; } = new();

        // Propiedad para capturar qué fila tiene seleccionada el usuario
        public CustomerRow? SelectedCustomer { get; set; }

        // Propiedad para detectar el cambio de mes en el ComboBox
        public int SelectedPeriodIndex
        {
            get => _selectedPeriodIndex;
            set
            {
                _selectedPeriodIndex = value;
                LoadCustomers(); // Cada vez que cambia el combo, recargamos la lista con los montos de ese mes
            }
        }

        public ICommand OpenAddCustomerCommand { get; set; }
        public ICommand DeleteCustomerCommand { get; set; }
        public ICommand EditCustomerCommand { get; set; }
        public ICommand AddPurchaseCommand { get; set; }
        public ICommand RestoreCustomerCommand { get; set; }

        public MainViewModel()
        {
            OpenAddCustomerCommand = new RelayCommand(OpenAddCustomerWindow);
            DeleteCustomerCommand = new RelayCommand(DeleteCustomer);
            EditCustomerCommand = new RelayCommand(OpenEditCustomerWindow);
            AddPurchaseCommand = new RelayCommand(OpenAddPurchaseWindow);
            RestoreCustomerCommand = new RelayCommand(RestoreCustomer);

            CheckAndCloseMonth();

            LoadCustomers();
        }

        public void LoadCustomers()
        {
            using var context = new AppDbContext();
            var dbCustomers = context.Customers
                                         .OrderByDescending(c => c.IsActive)
                                         .ThenBy(c => c.LastName)
                                         .ToList();
            CustomersDisplay.Clear();

            foreach (var c in dbCustomers)
            {
                // Evaluamos qué monto mostrar según la selección del ComboBox
                decimal spendingToShow = (_selectedPeriodIndex == 0) ? c.CurrentMonthSpending : c.LastMonthSpending;

                CustomersDisplay.Add(new CustomerRow
                {
                    Id = c.Id,
                    DocumentId = c.DocumentId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Spending = spendingToShow,
                    IsActive = c.IsActive
                });
            }
        }

        private void DeleteCustomer()
        {
            if (SelectedCustomer == null) return;

            using var context = new AppDbContext();
            var customerInDb = context.Customers.Find(SelectedCustomer.Id);

            if (customerInDb != null)
            {
                if (customerInDb.IsActive)
                {
                    // Primer clic: Borrado lógico (inhabilitar)
                    customerInDb.IsActive = false;
                    context.SaveChanges();
                    LoadCustomers();
                }
                else
                {
                    // Segundo clic (ya estaba inhabilitado): Borrado físico letal
                    var result = MessageBox.Show($"¿Destruir el registro de {SelectedCustomer.FirstName} para siempre? No hay vuelta atrás.", "Borrado Permanente", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        context.Customers.Remove(customerInDb);
                        context.SaveChanges();
                        LoadCustomers();
                    }
                }
            }
        }

        private void RestoreCustomer()
        {
            if (SelectedCustomer == null || SelectedCustomer.IsActive) return;

            using var context = new AppDbContext();
            var customerInDb = context.Customers.Find(SelectedCustomer.Id);

            if (customerInDb != null)
            {
                customerInDb.IsActive = true;
                context.SaveChanges();
                LoadCustomers();
            }
        }

        private void CheckAndCloseMonth()
        {
            using var context = new AppDbContext();
            var config = context.Configurations.FirstOrDefault();

            int currentMonth = System.DateTime.Now.Month;
            int currentYear = System.DateTime.Now.Year;

            // Si es la primera vez que se abre el programa en su historia
            if (config == null)
            {
                config = new AppConfig { LastClosedMonth = currentMonth, LastClosedYear = currentYear };
                context.Configurations.Add(config);
                context.SaveChanges();
                return;
            }

            // Si el mes o el año actual es distinto al último mes cerrado, ejecutamos la rotación
            if (config.LastClosedMonth != currentMonth || config.LastClosedYear != currentYear)
            {
                // 1. Exterminar definitivamente a los inactivos antes de rotar
                var inactiveCustomers = context.Customers.Where(c => !c.IsActive).ToList();
                context.Customers.RemoveRange(inactiveCustomers);

                // 2. Rotar el período de los sobrevivientes activos
                var activeCustomers = context.Customers.Where(c => c.IsActive).ToList();
                foreach (var c in activeCustomers)
                {
                    c.RotatePeriod();
                }

                config.LastClosedMonth = currentMonth;
                config.LastClosedYear = currentYear;
                context.SaveChanges();
            }
        }
        private void OpenEditCustomerWindow()
        {
            if (SelectedCustomer == null)
            {
                MessageBox.Show("Selecciona un cliente de la lista para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new EditCustomerWindow(SelectedCustomer.Id);
            editWindow.ShowDialog();
            LoadCustomers();
        }

        private void OpenAddPurchaseWindow()
        {
            if (SelectedCustomer == null)
            {
                MessageBox.Show("Primero seleccioná un cliente de la tabla para cargarle una compra.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Solo permitimos sumar compras al mes actual. Si está mirando el historial viejo, lo bloqueamos o le avisamos.
            if (SelectedPeriodIndex != 0)
            {
                MessageBox.Show("Solo podés cargar compras en el Mes Actual. Cambiá el selector de período.", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var purchaseWindow = new AddPurchaseWindow(SelectedCustomer.Id);
            purchaseWindow.ShowDialog();
            LoadCustomers(); // Recargamos para ver los puntos subir en tiempo real
        }


        private void OpenAddCustomerWindow()
        {
            var addWindow = new AddCustomerWindow();
            addWindow.ShowDialog();
            LoadCustomers();
        }


    }
}