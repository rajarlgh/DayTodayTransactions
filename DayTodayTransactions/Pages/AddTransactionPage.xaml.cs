using CommunityToolkit.Mvvm.ComponentModel;
using DayTodayTransactions.ViewModels;
using DayTodayTransactionsLibrary.Models;

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
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TransactionViewModel transactionViewModel)
        {
            if (Transaction != null)
            {
                // Populate fields
                _transactionViewModel = transactionViewModel;
                _transactionViewModel.LoadData();
                _transactionViewModel.Id = Transaction.Id;
                _transactionViewModel.Category = Transaction.Category;
                _transactionViewModel.Type = Transaction.Type;
                _transactionViewModel.Amount = Transaction.Amount;
                _transactionViewModel.Reason = Transaction.Reason;
                _transactionViewModel.Date = Transaction.Date;

            }
        }
    }
}
