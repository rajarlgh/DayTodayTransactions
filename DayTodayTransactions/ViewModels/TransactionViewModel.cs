using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayTodayTransactions.Pages;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using System.Collections.ObjectModel;

public partial class TransactionViewModel : ObservableObject
{
    private readonly ITransactionService _transactionService;
    private readonly ICategoryService _categoryService;

    public TransactionViewModel(ITransactionService transactionService, ICategoryService categoryService)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
    }
    public void LoadData()
    {
        this.TransactionText = "Edit Transaction";

    }
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private string reason = string.Empty;

    [ObservableProperty]
    private string type = string.Empty; // Income or Expense

    [ObservableProperty]
    private string transactionText = "Add Transaction";

    [ObservableProperty]
    private ObservableCollection<Category> listOfCategories;

    //[ObservableProperty]
    //private Category category;

    [ObservableProperty]
    private Category selectedCategory;

    [ObservableProperty]
    private DateTime date = DateTime.Now;


    partial void OnSelectedCategoryChanged(Category value)
    {
        if (value != null && value.Name == "Add New Category")
        {
            Shell.Current.GoToAsync(nameof(ManageCategoriesPage));
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
            Category = SelectedCategory, // Use the name of the selected category
            
            Date = DateTime.Now
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
        SelectedCategory = null;
    }

    public async Task LoadCategoriesAsync(Category selectedCategory)
    {
        var categories = await _categoryService.GetCategoriesAsync();
        ListOfCategories = new ObservableCollection<Category>(categories);

        // Add a special entry for adding new categories
        ListOfCategories.Add(new Category { Id = -1, Name = "Add New Category" });

        // Set the selected category after loading
        if (selectedCategory != null)
        {
            SelectedCategory = ListOfCategories.FirstOrDefault(c => c.Id == selectedCategory.Id);
        }
    }

}
