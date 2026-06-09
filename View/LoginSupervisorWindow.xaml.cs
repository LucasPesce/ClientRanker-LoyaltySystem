using Client_Ranker.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace Client_Ranker.View
{
    /// Ventana modal para solicitar la credencial de supervisor antes de entrar a configuraciones críticas.
    public partial class LoginSupervisorWindow : Window
    {
        #region Dependencias y Estado

        private readonly IDbContextFactory<AppDbContext>? _dbContextFactory;

        /// Indica si el usuario logró autenticarse correctamente.
        /// El ViewModel que invoca esta ventana lee esta propiedad al cerrarse.
        public bool IsAuthorized { get; private set; } = false;

        #endregion

        #region Constructores

        public LoginSupervisorWindow(IDbContextFactory<AppDbContext>? dbContextFactory = null)
        {
            InitializeComponent();
            _dbContextFactory = dbContextFactory;
            txtPassword.Focus(); // Foco automático para mejorar UX
        }

        private AppDbContext CreateContext()
        {
            return _dbContextFactory != null
                ? _dbContextFactory.CreateDbContext()
                : new AppDbContext();
        }

        #endregion

        #region Lógica de Autenticación

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            using var context = CreateContext();
            var config = context.Configurations.FirstOrDefault();

            if (config != null && txtPassword.Password == config.SupervisorPassword)
            {
                IsAuthorized = true;
                this.Close(); // Autenticación exitosa, cerramos y permitimos el paso
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta. Acceso denegado.", "Error de Seguridad", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPassword.Clear();
            }
        }

        #endregion

        #region Lógica de Rescate (Recuperación de Contraseña)

        private void BtnRecover_Click(object sender, RoutedEventArgs e)
        {
            using var context = CreateContext();
            var config = context.Configurations.FirstOrDefault();

            // Lógica de rescate: El usuario escribe su PIN en la misma caja de contraseña
            if (config != null && txtPassword.Password == config.RecoveryPin)
            {
                config.SupervisorPassword = "admin123";
                context.SaveChanges();

                MessageBox.Show("Contraseña reseteada con éxito a: admin123\nPor favor, ingrese con esa clave y cámbiela en Configuración.", "Rescate Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                txtPassword.Clear();
            }
            else
            {
                MessageBox.Show("Para recuperar su cuenta, ingrese su PIN de Rescate en el campo de contraseña y presione 'Olvidé mi contraseña'. Si el PIN es incorrecto, no se reseteará.", "Instrucciones de Rescate", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion
    }
}