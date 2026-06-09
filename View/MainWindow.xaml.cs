using Client_Ranker.ViewModels;
using System.Windows;

namespace Client_Ranker.View
{
    public partial class MainWindow : Window
    {
        // El contenedor de dependencias inyectará automáticamente el MainViewModel aquí
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel; // Asignamos el ViewModel ya resuelto
        }
    }
}