using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.Models
{
    public class MonthlySummary
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        // Métricas de facturación y frecuencia
        public decimal TotalRevenue { get; set; }
        public int TotalTickets { get; set; } 
        public int UniqueCustomers { get; set; }

        // Foto de las categorías en ese mes (para el gráfico de torta)
        public int BronzeCount { get; set; }
        public int SilverCount { get; set; }
        public int GoldCount { get; set; }
        public int PlatinumCount { get; set; }
        public int DiamondCount { get; set; }
    }
}
