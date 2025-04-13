using CommunityToolkit.Mvvm.ComponentModel;
using DayTodayTransactions.ViewModels;
using DayTodayTransactionsLibrary.Models;
using System.Collections.ObjectModel;

namespace DayTodayTransactions.Pages;
[QueryProperty(nameof(Type), "type")]  // Bind the 'type' query parameter to this property
[QueryProperty(nameof(TransactionViewModel), "TransactionViewModel")]
[QueryProperty(nameof(Transaction), "Transaction")]

public partial class AddTransactionPage : ContentPage
{
    private  TransactionViewModel _transactionViewModel;
    private Transaction _transaction;
    public AddTransactionPage(TransactionViewModel viewModel)
    {
        InitializeComponent();
        _transactionViewModel = viewModel;
        this.BindingContext = _transactionViewModel;
    }
    // Property to receive the 'type' parameter
    public string Type
    {
        get => _transactionViewModel.Type;
        set => _transactionViewModel.Type = value;
    }
    public TransactionViewModel TransactionViewModel
    {
        get => _transactionViewModel;
        set
        {
            _transactionViewModel = value;
            OnPropertyChanged(nameof(TransactionViewModel));
            BindingContext = _transactionViewModel;
        }
    }

    public Transaction Transaction
    {
        get => _transaction;
        set => _transaction = value;
    }




    // Handle receiving the transaction details for editing
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TransactionViewModel transactionViewModel)
        {
            if (Transaction != null)
            {
                // Load categories and set the selected category
                await transactionViewModel.LoadCategoriesAsync(Transaction.Category);

                // Load categories and set the selected category
                await transactionViewModel.LoadAccountsAsync(Transaction.Id);

                // Update other fields
                transactionViewModel.TransactionText = "Edit Transaction";
                transactionViewModel.Id = Transaction.Id;
                transactionViewModel.Type = Transaction.Type;
                transactionViewModel.Amount = Transaction.Amount;
                transactionViewModel.Reason = Transaction.Reason;
                transactionViewModel.Date = Transaction.Date;
            }
            else
            {
                transactionViewModel.Type = Type;
                await transactionViewModel.LoadCategoriesAsync(null);
                await transactionViewModel.LoadAccountsAsync(0);
            }

        }
    }

}
