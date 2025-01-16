using SQLite;

namespace DayTodayTransactionsLibrary.Models
{
    public class Account
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public string Name { get; set; } // e.g., "Savings Account"
        public string Currency { get; set; } // e.g., "USD", "EUR"
    }

}
