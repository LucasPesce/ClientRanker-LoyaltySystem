using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.Models
{
    /// Entidad histórica. Representa una "foto" o cierre estadístico de un mes específico.
    /// Se utiliza exclusivamente para alimentar el Dashboard y evaluar el crecimiento (Año vs Año, Mes vs Mes).
    public class MonthlySummary
    {
        #region Identificador y Período

        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        #endregion

        #region Rendimiento Comercial

        /// Sumatoria total de ingresos generados en este mes.
        public decimal TotalRevenue { get; set; }

        /// Cantidad total de compras/visitas registradas en el mes.
        public int TotalTickets { get; set; }

        /// Cantidad de personas distintas que compraron al menos una vez en el mes.
        public int UniqueCustomers { get; set; }

        #endregion

        #region Foto de Fidelización (Gamificación)

        // Snapshot de cuántos clientes había en cada categoría en el momento del cierre.

        public int BronzeCount { get; set; }
        public int SilverCount { get; set; }
        public int GoldCount { get; set; }
        public int PlatinumCount { get; set; }
        public int DiamondCount { get; set; }

        #endregion
    }
}
