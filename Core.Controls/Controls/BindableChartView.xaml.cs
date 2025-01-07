using Microcharts.Maui;
using Microcharts;

namespace Core.Controls.Controls;

public partial class BindableChartView : ContentView
{
    public static readonly BindableProperty EntriesProperty =
        BindableProperty.Create(nameof(Entries), typeof(IEnumerable<ChartEntry>), typeof(BindableChartView), propertyChanged: OnEntriesChanged);

    public IEnumerable<ChartEntry> Entries
    {
        get => (IEnumerable<ChartEntry>)GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    public BindableChartView()
    {
        InitializeComponent(); // Load the XAML layout
    }

    private static void OnEntriesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BindableChartView chartView && newValue is IEnumerable<ChartEntry> entries)
        {
            chartView.UpdateChart(entries);
        }
    }

    private void UpdateChart(IEnumerable<ChartEntry> entries)
    {
        // Update the chart in the XAML-defined ChartView
        ChartView.Chart = new DonutChart { Entries = entries };
    }
}