using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions.Pages;

public partial class TransactionHistoryPage : ContentPage
{
    private readonly TransactionHistoryViewModel _viewModel;
    public TransactionHistoryPage(TransactionHistoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = _viewModel;
    }

    private async void OnFilterChanged(object sender, EventArgs e)
    {
        _viewModel.FilterTransactions();
    }
}