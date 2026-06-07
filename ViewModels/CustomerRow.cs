using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.ViewModels
{
    public class CustomerRow
    {
        private const decimal PesosPorPunto = 100;

        public int Id { get; set; }
        public string DocumentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public decimal Spending { get; set; }
        public bool IsActive { get; set; }

        // Cálculos dinámicos basados en el período que se le asigne
        public int Points => (int)(Spending / PesosPorPunto);

        public string Category => Points switch
        {
            <= 200 => "Bronce",
            <= 400 => "Plata",
            <= 700 => "Oro",
            <= 1000 => "Platino",
            _ => "Diamante"
        };
    }
}
