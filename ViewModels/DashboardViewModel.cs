using Client_Ranker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalTickets { get; set; }
        public decimal AverageTicket { get; set; }

        public int BronzeCount { get; set; }
        public int SilverCount { get; set; }
        public int GoldCount { get; set; }
        public int PlatinumCount { get; set; }
        public int DiamondCount { get; set; }

        public string InsightMessage { get; set; }

        public DashboardViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            using var context = new AppDbContext();

            // Traemos el último resumen mensual guardado
            var lastSummary = context.MonthlySummaries
                                     .OrderByDescending(m => m.Year)
                                     .ThenByDescending(m => m.Month)
                                     .FirstOrDefault();

            if (lastSummary != null)
            {
                TotalRevenue = lastSummary.TotalRevenue;
                TotalTickets = lastSummary.TotalTickets;
                AverageTicket = TotalTickets > 0 ? TotalRevenue / TotalTickets : 0;

                BronzeCount = lastSummary.BronzeCount;
                SilverCount = lastSummary.SilverCount;
                GoldCount = lastSummary.GoldCount;
                PlatinumCount = lastSummary.PlatinumCount;
                DiamondCount = lastSummary.DiamondCount;

                // Motor básico de deducciones (Soporte a la decisión)
                if (TotalTickets == 0)
                {
                    InsightMessage = "No hay registro de visitas en el último mes cerrado.";
                }
                else if (AverageTicket < 2000) // Este umbral podría ser configurable a futuro
                {
                    InsightMessage = "Sugerencia: El gasto por visita es bajo. Considerá implementar combos o productos complementarios en la caja para elevar el ticket promedio.";
                }
                else
                {
                    InsightMessage = "Sugerencia: El ticket promedio es muy sólido. Enfocate en campañas para atraer clientes nuevos manteniendo esta calidad de venta.";
                }
            }
            else
            {
                InsightMessage = "Todavía no hay un cierre de mes registrado. Las estadísticas aparecerán cuando cambie el mes.";
            }
        }
    }
}