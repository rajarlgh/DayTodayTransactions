using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayTodayTransactionsLibrary.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string Type { get; set; } // "Income" or "Expense"
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }

}
