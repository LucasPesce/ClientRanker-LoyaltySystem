using Client_Ranker.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Client_Ranker.ViewModels
{
    /// ViewModel encargado de gestionar la configuración global del sistema (umbrales de puntos)
    /// y las credenciales de seguridad del supervisor.
    public class SettingsViewModel // ¡CORRECCIÓN CRÍTICA!: Ya no hereda de MainViewModel
    {
        #region Dependencias y Delegados

        private readonly IDbContextFactory<AppDbContext>? _dbContextFactory;

        // Acciones inyectadas desde la vista/orquestador para mantener el desacople
        private readonly Action _onSaveSuccess;
        private readonly Action _closeWindow;

        #endregion

        #region Propiedades de Configuración (Reglas de Negocio)

        public decimal PesosPorPunto { get; set; }
        public int SilverThreshold { get; set; }
        public int GoldThreshold { get; set; }
        public int PlatinumThreshold { get; set; }
        public int DiamondThreshold { get; set; }

        #endregion

        #region Propiedades de Seguridad

        public string SupervisorPassword { get; set; } = string.Empty;
        public string RecoveryPin { get; set; } = string.Empty;

        #endregion

        #region Comandos

        public ICommand SaveCommand { get; set; }

        #endregion

        #region Constructor e Inicialización

        /// Inicializa el ViewModel de configuraciones.
        /// <param name="onSaveSuccess">Acción a ejecutar al guardar con éxito (ej. recargar la tabla en MainViewModel).</param>
        /// <param name="closeWindow">Acción para cerrar la ventana actual de forma limpia.</param>
        /// <param name="dbContextFactory">Fábrica de contextos (DI).</param>
        public SettingsViewModel(Action onSaveSuccess, Action closeWindow, IDbContextFactory<AppDbContext>? dbContextFactory = null)
        {
            _onSaveSuccess = onSaveSuccess;
            _closeWindow = closeWindow;
            _dbContextFactory = dbContextFactory;

            SaveCommand = new RelayCommand(SaveSettings);

            LoadCurrentSettings();
        }

        private AppDbContext CreateContext()
        {
            return _dbContextFactory != null
                ? _dbContextFactory.CreateDbContext()
                : new AppDbContext();
        }

        #endregion

        #region Lógica de Base de Datos

        /// Obtiene la configuración actual de la base de datos para poblar los TextBox de la vista.
        private void LoadCurrentSettings()
        {
            using var context = CreateContext();
            var config = context.Configurations.FirstOrDefault();

            if (config != null)
            {
                PesosPorPunto = config.PesosPorPunto;
                SilverThreshold = config.SilverThreshold;
                GoldThreshold = config.GoldThreshold;
                PlatinumThreshold = config.PlatinumThreshold;
                DiamondThreshold = config.DiamondThreshold;

                SupervisorPassword = config.SupervisorPassword;
                RecoveryPin = config.RecoveryPin;
            }
        }

        /// Valida y persiste los cambios de configuración. Notifica el éxito y cierra la ventana.
        private void SaveSettings()
        {
            // 1. Validaciones básicas
            if (string.IsNullOrWhiteSpace(SupervisorPassword) || string.IsNullOrWhiteSpace(RecoveryPin))
            {
                MessageBox.Show("La contraseña y el PIN de rescate no pueden estar vacíos.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using var context = CreateContext();
            var config = context.Configurations.FirstOrDefault();

            if (config != null)
            {
                // 2. Mapeo de nuevas configuraciones
                config.PesosPorPunto = PesosPorPunto;
                config.SilverThreshold = SilverThreshold;
                config.GoldThreshold = GoldThreshold;
                config.PlatinumThreshold = PlatinumThreshold;
                config.DiamondThreshold = DiamondThreshold;

                config.SupervisorPassword = SupervisorPassword;
                config.RecoveryPin = RecoveryPin;

                // 3. Persistencia
                context.SaveChanges();

                MessageBox.Show("Configuración del sistema y credenciales actualizadas con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // 4. Ejecutamos la recarga en el MainViewModel
                _onSaveSuccess?.Invoke();

                // 5. Cerramos la ventana de manera limpia (Desacoplado)
                _closeWindow?.Invoke();
            }
        }

        #endregion
    }
}