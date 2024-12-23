namespace DayTodayTransactions.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public required string Reason { get; set; }
        public required string Type { get; set; } // "Income" or "Expense"
        public required string Category { get; set; }
        public DateTime Date { get; set; }
    }

}
