using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions.Pages;

public partial class AddTransactionPage : ContentPage
{
    private readonly TransactionViewModel _viewModel;

    public AddTransactionPage(TransactionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = _viewModel;
    }
}
