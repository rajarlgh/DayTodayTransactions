using DayTodayTransactionsLibrary.Models;
using Newtonsoft.Json;
using SQLite;

public class Transaction
{
    [PrimaryKey, AutoIncrement]
    public int? Id { get; set; }
    public int FromAccountId { get; set; } // Foreign key linking to Account
    public int? CategoryId { get; set; } // Foreign key linking to Category
    public int? ToAccountId { get; set; } // Foreign key linking to Account
    public decimal Amount { get; set; }
    public string Type { get; set; } // "Income" or "Expense"
    public string Reason { get; set; }
    public DateTime Date { get; set; }


    [Ignore]
    public Category Category { get; set; } // Ignore the direct Category property

    public string CategorySerialized
    {
        get => JsonConvert.SerializeObject(Category); // Convert Category to JSON string
        set => Category = JsonConvert.DeserializeObject<Category>(value); // Convert back from JSON string to Category
    }

    public int? AccountId { get; set; } // Foreign key linking to Account
}
