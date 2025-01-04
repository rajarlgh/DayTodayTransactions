using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayTodayTransactions.Pages;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using System.Collections.ObjectModel;

namespace DayTodayTransactions.ViewModels
{
    public partial class TransactionViewModel : ObservableObject
    {
        private readonly ITransactionService _transactionService;

        public TransactionViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            LoadCategories();
        }
        public void LoadData()
        {
            this.TransactionText = "Edit Transaction";
        }
        // Transaction properties
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private string reason = string.Empty;

        [ObservableProperty]
        private string type = string.Empty; // Income or Expense

        [ObservableProperty]
        private string category = string.Empty;

        [ObservableProperty]
        private DateTime date = DateTime.Now;

        [ObservableProperty]
        private string transactionText = "Add Transaction";

        // Categories and SelectedCategory
        [ObservableProperty]
        private ObservableCollection<string> categories;

        [ObservableProperty]
        private string selectedCategory;

        partial void OnSelectedCategoryChanged(string value)
        {
            if (value == "Add New Category")
            {
                // Navigate to the Manage Categories page
                Shell.Current.GoToAsync(nameof(ManageCategoriesPage));

                // Reset SelectedCategory to avoid accidental re-selection
                SelectedCategory = null;
            }
        }

        [RelayCommand]
        public async Task AddTransactionAsync()
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

                // Reset form properties
                ResetTransactionForm();

                // Show success message
                await Application.Current.MainPage.DisplayAlert("Success", "Transaction saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                // Handle exception (log or display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void ResetTransactionForm()
        {
            Amount = 0;
            Reason = string.Empty;
            Category = string.Empty;
            Date = DateTime.Now;
        }

        private void LoadCategories()
        {
            Categories = new ObservableCollection<string>
            {
                "Car",
                "Food",
                "Pet",
                "Health",
                "Cafe",
                "Bar",
                "Dental",
                "Home",
                "Mobile",
                "Cloths",
                "Sports",
                "Gift",
                "Fuel",
                "Add New Category" // Special entry for adding new categories
            };
        }

        [RelayCommand]
        public void AddCategory()
        {
            // Add a new category dynamically (replace with input from the user if needed)
            string newCategory = "Travel"; // Example: Replace with user input logic
            if (!Categories.Contains(newCategory))
            {
                Categories.Insert(Categories.Count - 1, newCategory); // Add before 'Add New Category'
            }
        }
    }
}
