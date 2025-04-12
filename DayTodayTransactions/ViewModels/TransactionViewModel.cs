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
    private readonly IAccountService _accountService;

    public TransactionViewModel(ITransactionService transactionService, ICategoryService categoryService, IAccountService accountService)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
        _accountService = accountService;

    }

    [ObservableProperty]
    private bool isCategoryVisible = false;

    public void LoadData()
    {
        this.TransactionText = "Edit Transaction";
    }

    [ObservableProperty]
    private int? id;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private string reason = string.Empty;

    [ObservableProperty]
    private string type = string.Empty;

    [ObservableProperty]
    private string transactionText = "Transaction";

    [ObservableProperty]
    private ObservableCollection<Category> listOfCategories;

    [ObservableProperty]
    private ObservableCollection<Account> listOfAccounts;

    [ObservableProperty]
    private Category selectedCategory;

    [ObservableProperty]
    private Account selectedAccount;

    [ObservableProperty]
    private DateTime date = DateTime.Now;

    partial void OnSelectedCategoryChanged(Category value)
    {
        if (value != null && value.Name == "Add New Category")
        {
            Shell.Current.GoToAsync(nameof(ManageCategoriesPage));
        }
    }

    partial void OnSelectedAccountChanged(Account value)
    {
        if (value != null && value.Name == "Add New Account")
        {
            Shell.Current.GoToAsync(nameof(ManageAccountsPage));
        }
    }

    public async Task LoadCategoriesAsync(Category selectedCategory)
    {
        var categories = await _categoryService.GetCategoriesAsync();
        ListOfCategories = new ObservableCollection<Category>(categories);
        ListOfCategories.Add(new Category { Id = -1, Name = "Add New Category" });

        if (selectedCategory != null)
        {
            SelectedCategory = ListOfCategories.FirstOrDefault(c => c.Id == selectedCategory.Id);
        }
    }

    public async Task LoadAccountsAsync(int? accountId )
    {

        var accounts = await _accountService.GetAccountsAsync();
        ListOfAccounts = new ObservableCollection<Account>(accounts);
        ListOfAccounts.Add(new Account { Id = -1, Name = "Add New Account" });

        var selectedAccount = accounts.FirstOrDefault(r => r.Id == accountId);
        if (selectedAccount != null)
        {
            SelectedAccount = ListOfAccounts.FirstOrDefault(a => a.Id == selectedAccount.Id);
        }

        //if (type == "Income")
        //{
        //    this.isCategoryVisible = false;
        //}
        //else
        //    this.isCategoryVisible = true;
        OnPropertyChanged(nameof(IsCategoryVisible));
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
            Category = SelectedCategory,
            AccountId = SelectedAccount?.Id ?? 0,
            Date = DateTime.Now
        };

        try
        {
            if (transaction.Id == null || transaction.Id == 0)
                await _transactionService.AddTransactionAsync(transaction);
            else
                await _transactionService.UpdateTransactionAsync(transaction);

            ResetTransactionForm();
            await Application.Current.MainPage.DisplayAlert("Success", "Transaction saved successfully.", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void ResetTransactionForm()
    {
        Amount = 0;
        Reason = string.Empty;
        SelectedCategory = null;
        SelectedAccount = null;
    }
}
