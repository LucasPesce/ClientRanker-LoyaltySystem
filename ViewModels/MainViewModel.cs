using ClosedXML.Excel;
using Client_Ranker.Data;
using Client_Ranker.View;
using Client_Ranker.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System;
using Client_Ranker.Services;
using Microsoft.EntityFrameworkCore; // Necesario para IDbContextFactory

namespace Client_Ranker.ViewModels
{
    /// ViewModel principal de la aplicación.
    /// Gestiona la visualización del listado de clientes, la interacción con la base de datos
    /// y la orquestación hacia otras ventanas (CRUD y utilidades).
    public class MainViewModel
    {
        #region Dependencias (DI)

        // Inyectamos un Factory en lugar del DbContext directamente. 
        // Esto previene fugas de memoria en WPF, ya que permite crear contextos de vida corta.
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly IDialogService _dialogService;
        #endregion

        #region Campos Privados

        private int _selectedPeriodIndex = 0;
        private string _searchText = string.Empty;

        #endregion

        #region Propiedades de Enlace (Bindings)

        /// Colección observable que notifica a la vista (DataGrid) cuando hay cambios en la lista de clientes.
        /// </summary>
        public ObservableCollection<CustomerRow> CustomersDisplay { get; set; } = new();

        /// Fila actualmente seleccionada en el DataGrid por el usuario.
        public CustomerRow? SelectedCustomer { get; set; }

        /// Índice del ComboBox que define si vemos el mes actual (0) o el mes pasado (1).
        public int SelectedPeriodIndex
        {
            get => _selectedPeriodIndex;
            set
            {
                _selectedPeriodIndex = value;
                LoadCustomers(); // Recargamos la grilla al cambiar de período
            }
        }

        /// Texto ingresado en la barra de búsqueda.
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                LoadCustomers(); // Filtramos en tiempo real
            }
        }

        #endregion

        #region Comandos (ICommand)

        public ICommand OpenAddCustomerCommand { get; set; }
        public ICommand DeleteCustomerCommand { get; set; }
        public ICommand EditCustomerCommand { get; set; }
        public ICommand AddPurchaseCommand { get; set; }
        public ICommand RestoreCustomerCommand { get; set; }
        public ICommand OpenSettingsCommand { get; set; }
        public ICommand OpenDashboardCommand { get; set; }
        public ICommand ExportReportCommand { get; set; }
        public ICommand TestAppCommand { get; set; }

        #endregion

        #region Constructor e Inicialización

        /// Constructor principal preparado para Inyección de Dependencias.
        /// <param name="dbContextFactory">Fábrica de contextos de BD proveída por el contenedor DI.</param>
       public MainViewModel(IDbContextFactory<AppDbContext> dbContextFactory, IDialogService dialogService)
        {
            _dbContextFactory = dbContextFactory;
            _dialogService = dialogService;

            OpenAddCustomerCommand = new RelayCommand(OpenAddCustomerWindow);
            DeleteCustomerCommand = new RelayCommand(DeleteCustomer);
            EditCustomerCommand = new RelayCommand(OpenEditCustomerWindow);
            AddPurchaseCommand = new RelayCommand(OpenAddPurchaseWindow);
            RestoreCustomerCommand = new RelayCommand(RestoreCustomer);
            OpenSettingsCommand = new RelayCommand(OpenSettingsWindow);
            OpenDashboardCommand = new RelayCommand(OpenDashboardWindow);
            ExportReportCommand = new RelayCommand(ExportToExcel);
            TestAppCommand = new RelayCommand(GenerateTestData);

            CheckAndCloseMonth();
            LoadCustomers();
        }

        /// Método auxiliar para crear el contexto. 
        /// Usa la Inyección de Dependencias si está disponible, sino hace un fallback (temporal).
        private AppDbContext CreateContext()
        {
            return _dbContextFactory != null
                ? _dbContextFactory.CreateDbContext()
                : new AppDbContext();
        }

        #endregion

        #region Lógica de Base de Datos y Negocio

        /// Carga y filtra los clientes desde la base de datos hacia la colección observable de la UI.
        public void LoadCustomers()
        {
            using var context = CreateContext();

            // 1. Obtenemos o creamos la configuración del sistema
            var config = context.Configurations.FirstOrDefault();
            if (config == null)
            {
                config = new AppConfig { LastClosedMonth = DateTime.Now.Month, LastClosedYear = DateTime.Now.Year };
                context.Configurations.Add(config);
                context.SaveChanges();
            }

            // Valores por defecto de seguridad si no existen
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

            // Aplicamos filtro de búsqueda si el usuario escribió algo
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

            // 2. Mapeamos de Entidad a DTO (CustomerRow) inyectando reglas de negocio
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
                    CurrentMonthVisits = c.CurrentMonthVisits,

                    // Inyección de parámetros de categoría
                    PesosPorPunto = config.PesosPorPunto,
                    SilverThreshold = config.SilverThreshold,
                    GoldThreshold = config.GoldThreshold,
                    PlatinumThreshold = config.PlatinumThreshold,
                    DiamondThreshold = config.DiamondThreshold
                });
            }
        }

        /// Lógica automática que evalúa si el mes cambió para resetear contadores y guardar el resumen.
        private void CheckAndCloseMonth()
        {
            using var context = CreateContext();
            var config = context.Configurations.FirstOrDefault();
            if (config == null) return;

            if (config.LastClosedMonth != DateTime.Now.Month || config.LastClosedYear != DateTime.Now.Year)
            {
                var allCustomers = context.Customers.ToList();
                var clientesConConsumo = allCustomers.Where(c => c.CurrentMonthSpending > 0).ToList();

                int totalTicketsReales = allCustomers.Sum(c => c.CurrentMonthVisits);

                // Generamos la foto histórica del mes que se cierra
                var summary = new MonthlySummary
                {
                    Year = config.LastClosedYear,
                    Month = config.LastClosedMonth,
                    TotalRevenue = clientesConConsumo.Sum(c => c.CurrentMonthSpending),
                    UniqueCustomers = clientesConConsumo.Count,
                    TotalTickets = totalTicketsReales
                };

                context.MonthlySummaries.Add(summary);

                // Rotación de métricas de clientes
                foreach (var customer in allCustomers)
                {
                    customer.LastMonthSpending = customer.CurrentMonthSpending;
                    customer.CurrentMonthSpending = 0;
                    customer.CurrentMonthVisits = 0;
                }

                config.LastClosedMonth = DateTime.Now.Month;
                config.LastClosedYear = DateTime.Now.Year;
                context.SaveChanges();
            }
        }

        #endregion

        #region Operaciones CRUD

        private void DeleteCustomer()
        {
            if (SelectedCustomer == null) return;

            using var context = CreateContext();
            var customerInDb = context.Customers.Find(SelectedCustomer.Id);

            if (customerInDb != null)
            {
                if (customerInDb.IsActive)
                {
                    // Borrado lógico
                    customerInDb.IsActive = false;
                    context.SaveChanges();
                    LoadCustomers();
                }
                else
                {
                    // Borrado físico permanente
                    var result = MessageBox.Show($"¿Seguro quiere eliminar de forma definitiva a {SelectedCustomer.FirstName}?", "Borrado Permanente", MessageBoxButton.YesNo, MessageBoxImage.Warning);
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

            using var context = CreateContext();
            var customerInDb = context.Customers.Find(SelectedCustomer.Id);

            if (customerInDb != null)
            {
                customerInDb.IsActive = true;
                context.SaveChanges();
                LoadCustomers();
            }
        }

        #endregion

        #region Navegación a Otras Ventanas (Vistas)

        // para no acoplar la creación de Vistas (new Window()) dentro del ViewModel.

        private void OpenAddCustomerWindow() => _dialogService.OpenAddCustomer();


        private void OpenEditCustomerWindow()
        {
            if (SelectedCustomer == null) return;
            _dialogService.OpenEditCustomer(SelectedCustomer.Id);
        }

        private void OpenAddPurchaseWindow()
        {
            if (SelectedCustomer == null || SelectedPeriodIndex != 0) return;
            _dialogService.OpenAddPurchase(SelectedCustomer.Id);
        }

        private void OpenDashboardWindow() => _dialogService.OpenDashboard();


        private void OpenSettingsWindow()
        {
            if (_dialogService.OpenLoginSupervisor())
            {
                _dialogService.OpenSettings(LoadCustomers);
            }
        }

        #endregion

        #region Utilidades (Exportación a Excel)

        /// Genera y descarga un reporte en formato .xlsx usando la librería ClosedXML.
        private void ExportToExcel()
        {
            if (CustomersDisplay == null || !CustomersDisplay.Any())
            {
                MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Libro de Excel (*.xlsx)|*.xlsx",
                FileName = $"Reporte_Fidelidad_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
                Title = "Guardar Reporte Comercial"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Panel General");

                        // Estilos de Encabezados
                        string[] headers = { "Documento/DNI", "Nombre", "Apellido", "Consumo Período ($)", "Visitas", "Estado Cuenta" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            var cell = worksheet.Cell(1, i + 1);
                            cell.Value = headers[i];
                            cell.Style.Font.Bold = true;
                            cell.Style.Font.FontColor = XLColor.White;
                            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2c3e50");
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        // Mapeo de Filas
                        int currentRow = 2;
                        foreach (var customer in CustomersDisplay)
                        {
                            worksheet.Cell(currentRow, 1).Value = customer.DocumentId;
                            worksheet.Cell(currentRow, 2).Value = customer.FirstName;
                            worksheet.Cell(currentRow, 3).Value = customer.LastName;

                            var moneyCell = worksheet.Cell(currentRow, 4);
                            moneyCell.Value = customer.Spending;
                            moneyCell.Style.NumberFormat.Format = "$#,##0.00";

                            worksheet.Cell(currentRow, 5).Value = customer.CurrentMonthVisits;

                            var statusCell = worksheet.Cell(currentRow, 6);
                            statusCell.Value = customer.IsActive ? "Activo" : "Inhabilitado";
                            statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Zebra striping
                            if (currentRow % 2 == 0)
                            {
                                worksheet.Range(currentRow, 1, currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#f8f9fa");
                            }

                            currentRow++;
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Reporte generado con éxito.", "Exportación", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("El archivo está abierto en otro programa. Ciérrelo e intente nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error crítico al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Datos de Prueba (Testing)

        /// Borra los históricos actuales y genera 12 meses de datos ficticios para poder ver los gráficos en el Dashboard.
        private void GenerateTestData()
        {
            var result = MessageBox.Show("¿Generar datos de prueba? Esto borrará el historial de resúmenes.", "Modo Demo", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            using var context = CreateContext();

            context.MonthlySummaries.RemoveRange(context.MonthlySummaries);

            var random = new Random();
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            for (int i = 13; i >= 1; i--)
            {
                int targetMonth = month - i;
                int targetYear = year;
                if (targetMonth <= 0) { targetMonth += 12; targetYear--; }

                context.MonthlySummaries.Add(new MonthlySummary
                {
                    Year = targetYear,
                    Month = targetMonth,
                    TotalRevenue = random.Next(150000, 500000),
                    TotalTickets = random.Next(50, 200),
                    UniqueCustomers = random.Next(40, 100),
                    BronzeCount = random.Next(20, 50),
                    SilverCount = random.Next(10, 30),
                    GoldCount = random.Next(5, 15),
                    PlatinumCount = random.Next(2, 10),
                    DiamondCount = random.Next(1, 5)
                });
            }

            context.SaveChanges();
            MessageBox.Show("Datos generados. Abrí el Dashboard para verlos.", "Demo");
        }

        #endregion
    }
}