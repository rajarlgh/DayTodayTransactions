using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions.Pages;

public partial class ManageCategoriesPage : ContentPage
{
    private ManageCategoriesViewModel ViewModel => BindingContext as ManageCategoriesViewModel;

    public ManageCategoriesPage(ManageCategoriesViewModel viewModel)
    {
		InitializeComponent();
		this.BindingContext = viewModel;
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null)
            await ViewModel.InitializeAsync();
    }
}