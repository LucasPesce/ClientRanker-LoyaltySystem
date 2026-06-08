using Client_Ranker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client_Ranker.ViewModels
{
    public class SettingsViewModel
    {
        private Action _closeAction;
        private Action _refreshGridAction;

        public decimal PesosPorPunto { get; set; }
        public int SilverThreshold { get; set; }
        public int GoldThreshold { get; set; }
        public int PlatinumThreshold { get; set; }
        public int DiamondThreshold { get; set; }

        public ICommand SaveCommand { get; set; }

        public SettingsViewModel(Action closeAction, Action refreshGridAction)
        {
            _closeAction = closeAction;
            _refreshGridAction = refreshGridAction;
            LoadCurrentSettings();
            SaveCommand = new RelayCommand(SaveChanges);
        }

        private void LoadCurrentSettings()
        {
            using var context = new AppDbContext();
            var config = context.Configurations.FirstOrDefault();

            if (config != null)
            {
                if (config.PesosPorPunto == 0)
                {
                    config.PesosPorPunto = 100m;
                    config.SilverThreshold = 100;
                    config.GoldThreshold = 300;
                    config.PlatinumThreshold = 600;
                    config.DiamondThreshold = 900;
                    context.SaveChanges();
                }

                PesosPorPunto = config.PesosPorPunto;
                SilverThreshold = config.SilverThreshold;
                GoldThreshold = config.GoldThreshold;
                PlatinumThreshold = config.PlatinumThreshold;
                DiamondThreshold = config.DiamondThreshold;
            }
        }

        private void SaveChanges()
        {
            if (PesosPorPunto <= 0)
            {
                MessageBox.Show("El valor de pesos por punto debe ser mayor a cero.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new AppDbContext();
            var config = context.Configurations.FirstOrDefault();

            if (config != null)
            {
                config.PesosPorPunto = PesosPorPunto;
                config.SilverThreshold = SilverThreshold;
                config.GoldThreshold = GoldThreshold;
                config.PlatinumThreshold = PlatinumThreshold;
                config.DiamondThreshold = DiamondThreshold;

                context.SaveChanges();
            }

            // Recargamos la tabla principal para que aplique los nuevos rangos instantáneamente
            _refreshGridAction();
            _closeAction();
        }
    }
}
