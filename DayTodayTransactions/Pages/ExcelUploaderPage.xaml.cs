using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions.Pages;

public partial class ExcelUploaderPage : ContentPage
{
	public ExcelUploaderPage(ExcelUploaderViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}