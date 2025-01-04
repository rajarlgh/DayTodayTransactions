using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions.Pages;

public partial class ManageCategoriesPage : ContentPage
{
	public ManageCategoriesPage(ManageCategoriesViewModel viewModel)
    {
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}