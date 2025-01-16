using DayTodayTransactionsLibrary.Models;
using Newtonsoft.Json;
using SQLite;

public class Transaction
{
    [PrimaryKey, AutoIncrement]
    public int? Id { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public string Type { get; set; } // "Income" or "Expense"

    [Ignore]
    public Category Category { get; set; } // Ignore the direct Category property

    public string CategorySerialized
    {
        get => JsonConvert.SerializeObject(Category); // Convert Category to JSON string
        set => Category = JsonConvert.DeserializeObject<Category>(value); // Convert back from JSON string to Category
    }

    public DateTime Date { get; set; }

    public int? AccountId { get; set; } // Foreign key linking to Account
}
