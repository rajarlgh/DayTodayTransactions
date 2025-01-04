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
        private int id;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private string reason=string.Empty;

        [ObservableProperty]
        private string type = string.Empty; // Income or Expense

        [ObservableProperty]
        private string category = string.Empty;

        [ObservableProperty]
        private DateTime date = DateTime.Now;

        public void LoadData()
        {
            this.TransactionText = "Edit Transaction";
        }

        [ObservableProperty]
        private string transactionText = "Add Transaction";

        [RelayCommand]
        public async Task AddTransactionAsynca()
        {
            var transaction = new Transaction
            {
                Id = Id,
                Amount = Amount,
                Reason = Reason,
                Type = Type,
                Category = Category,
                Date = Date
            };

            try
            {
                if (transaction.Id == 0)
                    await _transactionService.AddTransactionAsync(transaction);
                else
                    await _transactionService.UpdateTransactionAsync(transaction);
            }
            catch(Exception ex)
            {


            }
            // Reset properties to default values
            Amount = 0;
            Reason = string.Empty;
            //Type = null; // Reset to null if no default type
            //Category = null; // Reset to null if no default category
            Date = DateTime.Now;

            // Show success message
            await Application.Current.MainPage.DisplayAlert("Success", "Transaction saved successfully.", "OK");

        }
    }

}
