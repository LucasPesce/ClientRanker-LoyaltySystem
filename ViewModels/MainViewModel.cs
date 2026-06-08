using Client_Ranker.Data;
using Client_Ranker.View;
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
        public ICommand OpenSettingsCommand { get; set; }

        public ICommand OpenDashboardCommand { get; set; }

        public MainViewModel()
        {
            OpenAddCustomerCommand = new RelayCommand(OpenAddCustomerWindow);
            DeleteCustomerCommand = new RelayCommand(DeleteCustomer);
            EditCustomerCommand = new RelayCommand(OpenEditCustomerWindow);
            AddPurchaseCommand = new RelayCommand(OpenAddPurchaseWindow);
            RestoreCustomerCommand = new RelayCommand(RestoreCustomer);
            OpenSettingsCommand = new RelayCommand(OpenSettingsWindow);
            OpenDashboardCommand = new RelayCommand(OpenDashboardWindow);

            CheckAndCloseMonth();

            LoadCustomers();
        }

        private string _searchText = string.Empty;


        private void OpenDashboardWindow()
        {
            var dashboardWindow = new DashboardWindow();
            dashboardWindow.ShowDialog();
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                LoadCustomers();
            }
        }

        public void LoadCustomers()
        {
            using var context = new AppDbContext();

            // 1. Obtenemos la configuración actual (o creamos una por defecto si no existe)
            var config = context.Configurations.FirstOrDefault();
            if (config == null)
            {
                config = new AppConfig { LastClosedMonth = System.DateTime.Now.Month, LastClosedYear = System.DateTime.Now.Year };
                context.Configurations.Add(config);
                context.SaveChanges();
            }
            if (config.PesosPorPunto == 0)
            {
                config.PesosPorPunto = 100m;
                config.SilverThreshold = 300;
                config.GoldThreshold = 800;
                config.PlatinumThreshold = 1500;
                config.DiamondThreshold = 2500;
                context.SaveChanges();
            }
            var query = context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                query = query.Where(c =>
                    c.FirstName.ToLower().Contains(searchLower) ||
                    c.LastName.ToLower().Contains(searchLower) ||
                    c.DocumentId.Contains(searchLower));
            }

            var dbCustomers = query
                .OrderByDescending(c => c.IsActive)
                .ThenBy(c => c.LastName)
                .ToList();

            CustomersDisplay.Clear();

            foreach (var c in dbCustomers)
            {
                decimal spendingToShow = (SelectedPeriodIndex == 0) ? c.CurrentMonthSpending : c.LastMonthSpending;

                CustomersDisplay.Add(new CustomerRow
                {
                    Id = c.Id,
                    DocumentId = c.DocumentId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Spending = spendingToShow,
                    IsActive = c.IsActive,

                    // 2. Inyectamos las reglas de negocio al DTO
                    PesosPorPunto = config.PesosPorPunto,
                    SilverThreshold = config.SilverThreshold,
                    GoldThreshold = config.GoldThreshold,
                    PlatinumThreshold = config.PlatinumThreshold,
                    DiamondThreshold = config.DiamondThreshold
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
                    var result = MessageBox.Show($"¿Seguro quiere eliminar de forma definitiva el registro de {SelectedCustomer.FirstName}?.", "Borrado Permanente", MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
            if (config == null) return;

            if (config.LastClosedMonth != DateTime.Now.Month || config.LastClosedYear != DateTime.Now.Year)
            {
                var allCustomers = context.Customers.ToList();
                var clientesConConsumo = allCustomers.Where(c => c.CurrentMonthSpending > 0).ToList();

                // AQUÍ ESTÁ LA LÓGICA REAL: Sumamos las visitas de todos los clientes
                int totalTicketsReales = allCustomers.Sum(c => c.CurrentMonthVisits);

                var summary = new MonthlySummary
                {
                    Year = config.LastClosedYear,
                    Month = config.LastClosedMonth,
                    TotalRevenue = clientesConConsumo.Sum(c => c.CurrentMonthSpending),
                    UniqueCustomers = clientesConConsumo.Count,
                    TotalTickets = totalTicketsReales, // <-- Ahora es un dato real
                                                       // ... (mantené el resto de tus contadores de Bronce, Plata, etc. igual)
                };

                context.MonthlySummaries.Add(summary);

                foreach (var customer in allCustomers)
                {
                    customer.LastMonthSpending = customer.CurrentMonthSpending;
                    customer.CurrentMonthSpending = 0;
                    customer.CurrentMonthVisits = 0; // <-- Reseteamos las visitas reales para el mes siguiente
                }

                config.LastClosedMonth = DateTime.Now.Month;
                config.LastClosedYear = DateTime.Now.Year;
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

        private void OpenSettingsWindow()
        {
            // Le pasamos LoadCustomers como la acción de recarga
            var settingsWindow = new SettingsWindow(LoadCustomers);
            settingsWindow.ShowDialog();
        }
        private void OpenAddCustomerWindow()
        {
            var addWindow = new AddCustomerWindow();
            addWindow.ShowDialog();
            LoadCustomers();
        }


    }
}