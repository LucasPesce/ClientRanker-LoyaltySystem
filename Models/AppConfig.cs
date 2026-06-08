using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.Models
{
    public class AppConfig
    {
        public int Id { get; set; }
        public int LastClosedMonth { get; set; }
        public int LastClosedYear { get; set; }

        public decimal PesosPorPunto { get; set; } = 100m;

        public int SilverThreshold { get; set; } = 100;
        public int GoldThreshold { get; set; } = 300;
        public int PlatinumThreshold { get; set; } = 600;
        public int DiamondThreshold { get; set; } = 900;
    }
}
