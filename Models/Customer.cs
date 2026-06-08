using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string DocumentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public decimal CurrentMonthSpending { get; set; }
        public decimal LastMonthSpending { get; set; }
        public bool IsActive { get; set; } = true;
        public int CurrentMonthVisits { get; set; }
        public void RotatePeriod()
        {
            LastMonthSpending = CurrentMonthSpending;
            CurrentMonthSpending = 0;
        }
    }
}
