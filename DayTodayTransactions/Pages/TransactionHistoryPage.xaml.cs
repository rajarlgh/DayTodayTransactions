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
        _incomeExpenseChart = new DonutChart
        {
            Entries = new[]
            {
            new ChartEntry(200) { Label = "Income", ValueLabel = "200", Color = SKColor.Parse("#00FF00") },
            new ChartEntry(150) { Label = "Expenses", ValueLabel = "150", Color = SKColor.Parse("#FF0000") },
        }
        };
    }
    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        // Render the chart
        var chartView = new ChartView
        {
            Chart = _incomeExpenseChart
        };
        chartView.Chart.Draw(canvas, e.Info.Width, e.Info.Height);
    }
    private async void OnFilterChanged(object sender, EventArgs e)
    {
        _viewModel.FilterTransactions();
    }
}