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

        // Initialize the chart
        //_incomeExpenseChart = new DonutChart
        //{
        //    Entries = new[]
        //    {
        //        new ChartEntry(200) { Label = "Income", ValueLabel = "200", Color = SKColor.Parse("#00FF00") },
        //        new ChartEntry(150) { Label = "Expenses", ValueLabel = "150", Color = SKColor.Parse("#FF0000") },
        //    }
        //};
        //        _barChat = new[]
        //        {
        //            new ChartEntry(212) { Label = "Car", ValueLabel = "212", Color = SKColor.Parse("#00FF00") }, // Green
        //            new ChartEntry(248) { Label = "Food", ValueLabel = "248", Color = SKColor.Parse("#FF5733") }, // Orange
        //            new ChartEntry(128) { Label = "Pet", ValueLabel = "128", Color = SKColor.Parse("#3498DB") }, // Blue
        //            new ChartEntry(514) { Label = "Health", ValueLabel = "514", Color = SKColor.Parse("#9B59B6") }, // Purple
        //            new ChartEntry(300) { Label = "Cafe", ValueLabel = "300", Color = SKColor.Parse("#1ABC9C") }, // Teal
        //            new ChartEntry(450) { Label = "Bar", ValueLabel = "450", Color = SKColor.Parse("#F1C40F") }, // Yellow
        //            new ChartEntry(380) { Label = "Dental", ValueLabel = "380", Color = SKColor.Parse("#E74C3C") }, // Red
        //            new ChartEntry(420) { Label = "Home", ValueLabel = "420", Color = SKColor.Parse("#34495E") }, // Dark Blue
        //            new ChartEntry(310) { Label = "Mobile", ValueLabel = "310", Color = SKColor.Parse("#2ECC71") }, // Light Green
        //            new ChartEntry(270) { Label = "Cloths", ValueLabel = "270", Color = SKColor.Parse("#E67E22") }, // Amber
        //            new ChartEntry(390) { Label = "Sports", ValueLabel = "390", Color = SKColor.Parse("#16A085") }, // Emerald Green
        //            new ChartEntry(460) { Label = "Gift", ValueLabel = "460", Color = SKColor.Parse("#8E44AD") }, // Violet
        //            new ChartEntry(510) { Label = "Fuel", ValueLabel = "510", Color = SKColor.Parse("#BDC3C7") }, // Light Gray
        //};

        //donutChart.Chart = _viewModel.IncomeExpenseChart;
        // Bind the chart from the ViewModel to the ChartView
        donutChart.Chart = _viewModel.Chart;

    }
    //private bool _isExpanded = false;
    //private ChartEntry[] _barChat;
    //private void OnToggleAccordionClicked(object sender, EventArgs e)
    //{
    //    _isExpanded = !_isExpanded;
    //    ToggleAccordionButton.Text = _isExpanded ? "Hide Chart" : "Show Chart";
    //    //AccordionContent.IsVisible = true;

    //    ChartLayout.IsVisible = _isExpanded;
    //    //LabelsList.IsVisible = !_isExpanded;
    //}
    //private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    //{
    //    var canvas = e.Surface.Canvas;
    //    canvas.Clear();

    //    // Render the chart
    //    var chartView = new ChartView
    //    {
    //        Chart = _incomeExpenseChart
    //    };
    //    chartView.Chart.Draw(canvas, e.Info.Width, e.Info.Height);
    //}
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
}