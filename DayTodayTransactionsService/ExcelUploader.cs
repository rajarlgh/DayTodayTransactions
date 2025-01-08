

using DayTodayTransactionsLibrary.Models;
using ExcelDataReader;
using System.Globalization;

public class ExcelUploader
{
    public List<Transaction> ReadExcel(string filePath)
    {
        var transactions = new List<Transaction>();

        // Open the file stream
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            // Configure the reader to handle different formats
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataSet = reader.AsDataSet();

                // Assuming the data is in the first sheet
                var dataTable = dataSet.Tables[0];

                // Skip header row (row 0)
                for (int i = 1; i < dataTable.Rows.Count; i++)
                {
                    var row = dataTable.Rows[i];

                    var category = new Category();
                    category.Name = row[2].ToString();
                    var transaction = new Transaction
                    {
                        Date = DateTime.ParseExact(row[0].ToString(), "d/M/yyyy", CultureInfo.InvariantCulture),
                        AccountId = ResolveAccountId(row[1].ToString()),
                        Category = category,
                        Amount = decimal.Parse(row[3].ToString()),
                        Reason = row[4].ToString()
                    };

                    transactions.Add(transaction);
                }
            }

        }

        return transactions;
    }

    // Example method to resolve AccountId from account name
    private int ResolveAccountId(string accountName)
    {
        var accountMapping = new Dictionary<string, int>
        {
            { "Cash in Bank", 1 },
            { "Paytm", 2 },
            { "Cash in hand", 3 }
        };

        return accountMapping.GetValueOrDefault(accountName, 0);
    }
}
