using System;
using System.Windows;
using Client_Ranker.Data;
using Client_Ranker.Services;
using Client_Ranker.View;
using Client_Ranker.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Client_Ranker
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Iniciamos la ventana principal resolviéndola desde el contenedor
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 1. Base de Datos (Fábrica de Contextos)
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite("Data Source=loyalty_database.db"));

            // 2. Servicios de Infraestructura (Singleton para que haya una sola instancia en toda la app)
            services.AddSingleton<IDialogService, DialogService>();

            // 3. Registrar ViewModels
            services.AddTransient<MainViewModel>();

            // 4. Registrar Vistas (Windows)
            services.AddTransient<MainWindow>();
        }
    }
}