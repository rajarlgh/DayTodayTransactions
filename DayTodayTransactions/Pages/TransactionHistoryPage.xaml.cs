using DayTodayTransactions.ViewModels;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace DayTodayTransactions.Pages;

public partial class TransactionHistoryPage : ContentPage
{
    private readonly TransactionHistoryViewModel _viewModel;
    private Chart _incomeExpenseChart;
    public TransactionHistoryPage(TransactionHistoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = _viewModel;
        donutIncomeChart.Chart = _viewModel.IncomeChart;
        donutExpenseChart.Chart = _viewModel.ExpenseChart;

    }

    private async void OnFilterChanged(object sender, EventArgs e)
    {
        _viewModel.FilterTransactions();
    }
    private void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        if (BindingContext is TransactionHistoryViewModel viewModel && sender is ScrollView scrollView)
        {
            viewModel.UpdateScrollMessage(e.ScrollY, scrollView.ContentSize.Height, scrollView.Height);
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is TransactionHistoryViewModel viewModel)
        {
            await viewModel.RefreshDataAsync();
            donutIncomeChart.Chart = _viewModel.IncomeChart;
            donutExpenseChart.Chart = _viewModel.ExpenseChart;
        }
    }
}