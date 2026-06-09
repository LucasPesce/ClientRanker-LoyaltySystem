using Microsoft.EntityFrameworkCore;
using Client_Ranker.Models;

namespace Client_Ranker.Data
{
    /// Contexto principal de la base de datos Entity Framework Core.
    /// Define las tablas (DbSets) y las reglas de modelado (Fluent API) para SQLite.
    public class AppDbContext : DbContext
    {
        #region Tablas (DbSets)

        public DbSet<Customer> Customers { get; set; }
        public DbSet<AppConfig> Configurations { get; set; }
        public DbSet<MonthlySummary> MonthlySummaries { get; set; }

        #endregion

        #region Constructores

        /// Constructor por defecto necesario para migraciones y fallback sin DI.
        /// Aplica las migraciones pendientes automáticamente al instanciar.
        public AppDbContext()
        {
            // Nota: En producción masiva, Database.Migrate() suele ejecutarse al inicio de la app,
            // no en cada instancia del contexto. Se mantiene aquí por practicidad en SQLite.
            this.Database.Migrate();
        }

        /// Constructor utilizado por la Inyección de Dependencias (DI).
        /// Permite inyectar opciones (como la cadena de conexión o un DB en memoria para Testing).
        /// <param name="options">Opciones de configuración del DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            this.Database.Migrate();
        }

        #endregion

        #region Configuración

        /// Configura el proveedor de base de datos.
        /// Solo se ejecuta si las opciones no fueron provistas por el contenedor de DI.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // El if verifica que no estemos sobreescribiendo la configuración de DI
            if (!optionsBuilder.IsConfigured)
            {
                // Motor ligero e ideal para aplicaciones de escritorio locales
                optionsBuilder.UseSqlite("Data Source=loyalty_database.db");
            }
        }

        /// Define restricciones y reglas de negocio a nivel de base de datos usando Fluent API.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Regla de Negocio: No pueden existir dos clientes activos con el mismo Documento.
            // Esto previene errores humanos en la carga y problemas al buscar clientes.
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.DocumentId)
                .IsUnique();
        }

        #endregion
    }
}