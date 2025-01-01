using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions.Pages;
[QueryProperty(nameof(Type), "type")]  // Bind the 'type' query parameter to this property

public partial class AddTransactionPage : ContentPage
{
    private readonly TransactionViewModel _viewModel;

    public AddTransactionPage(TransactionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = _viewModel;
    }
    // Property to receive the 'type' parameter
    public string Type
    {
        get => _viewModel.Type;
        set => _viewModel.Type = value;
    }

}
