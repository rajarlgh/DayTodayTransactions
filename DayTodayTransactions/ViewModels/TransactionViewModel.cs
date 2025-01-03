using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;

namespace DayTodayTransactions.ViewModels
{
    public partial class TransactionViewModel : ObservableObject
    {
        private readonly ITransactionService _transactionService;

        public TransactionViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private string reason;

        [ObservableProperty]
        private string type; // Income or Expense

        [ObservableProperty]
        private string category;

        [ObservableProperty]
        private DateTime date = DateTime.Now;

        [RelayCommand]
        public async Task AddTransactionAsynca()
        {
            var transaction = new Transaction
            {
                Amount = Amount,
                Reason = Reason,
                Type = Type,
                Category = Category,
                Date = Date
            };

            await _transactionService.AddTransactionAsync(transaction);

            // Reset properties to default values
            Amount = 0;
            Reason = string.Empty;
            Type = null; // Reset to null if no default type
            Category = null; // Reset to null if no default category
            Date = DateTime.Now;

            // Show success message
            await Application.Current.MainPage.DisplayAlert("Success", "Transaction saved successfully.", "OK");

        }
    }

}
