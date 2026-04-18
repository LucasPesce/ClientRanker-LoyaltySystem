using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.Models
{
    public class Customer
    {
        private const decimal PesosPorPunto = 100;

        public int Id { get; set; } 
        public string DocumentId { get; set; } = string.Empty; // DNI o CUIT
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public decimal MonthlySpending { get; set; }

        public int Points => (int)(MonthlySpending / PesosPorPunto);

        // Lógica de categorías según tus puntos
        public string Category => Points switch
        {
            <= 100 => "Bronce",
            <= 300 => "Plata",
            <= 600 => "Oro",
            <= 900 => "Platino",
            _ => "Diamante"
        };
    }
}
