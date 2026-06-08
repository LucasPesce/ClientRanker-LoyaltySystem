using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.ViewModels
{
    public class CustomerRow
    {
        public int Id { get; set; }
        public string DocumentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public decimal Spending { get; set; }
        public bool IsActive { get; set; }

        // NUEVO: Variables de configuración inyectadas
        public decimal PesosPorPunto { get; set; }
        public int SilverThreshold { get; set; }
        public int GoldThreshold { get; set; }
        public int PlatinumThreshold { get; set; }
        public int DiamondThreshold { get; set; }

        public int Points => PesosPorPunto > 0 ? (int)(Spending / PesosPorPunto) : 0;

        public string Category
        {
            get
            {
                if (Points < SilverThreshold) return "Bronce";
                if (Points < GoldThreshold) return "Plata";
                if (Points < PlatinumThreshold) return "Oro";
                if (Points < DiamondThreshold) return "Platino";
                return "Diamante";
            }
        }
    }
}
