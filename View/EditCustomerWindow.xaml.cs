using Client_Ranker.Models;
using Client_Ranker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client_Ranker.View
{
    /// <summary>
    /// Lógica de interacción para EditCustomerWindow.xaml
    /// </summary>
    public partial class EditCustomerWindow : Window
    {
        public EditCustomerWindow(int customerId)
        {
            InitializeComponent();
            this.DataContext = new EditCustomerViewModel(customerId, this.Close);
        }
    }
}
