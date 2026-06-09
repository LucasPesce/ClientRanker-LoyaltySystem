using Client_Ranker.Data;
using Client_Ranker.ViewModels;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Windows;

namespace Client_Ranker.View
{
    public partial class DashboardWindow : Window
    {
        // Agregamos el constructor que recibe la fábrica de DbContext
        public DashboardWindow(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            InitializeComponent();
            DataContext = new DashboardViewModel(dbContextFactory);
        }
    }

    // Helper simple para crear una fábrica por defecto si no hay DI configurada
    public class DefaultDbContextFactory<TContext> : IDbContextFactory<TContext> where TContext : DbContext
    {
        public TContext CreateDbContext()
        {
            // Asumiendo que AppDbContext tiene un constructor público sin argumentos
            return (TContext)Activator.CreateInstance(typeof(TContext))!;
        }
    }
}