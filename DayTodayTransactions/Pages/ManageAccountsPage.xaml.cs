using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions.Pages;

public partial class ManageAccountsPage : ContentPage
{
    private readonly ManageAccountsViewModel _viewModel;

    public ManageAccountsPage(ManageAccountsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAccountsAsync();
    }
}
