using SQLite;

namespace DayTodayTransactionsLibrary.Models
{
    public class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string Type { get; set; } // "Income" or "Expense"
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }

}
