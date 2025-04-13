using Microcharts.Maui;
using Microcharts;

namespace Core.Controls.Controls;

public partial class BindableChartView : ContentView
{
    public static readonly BindableProperty EntriesProperty =
        BindableProperty.Create(nameof(Entries), typeof(IEnumerable<ChartEntryWrapper>), typeof(BindableChartView), propertyChanged: OnEntriesChanged);

    public IEnumerable<ChartEntryWrapper> Entries
    {
        get => (IEnumerable<ChartEntryWrapper>)GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    public BindableChartView()
    {
        InitializeComponent(); // Load the XAML layout
    }

    private static void OnEntriesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BindableChartView chartView && newValue is IEnumerable<ChartEntryWrapper> entries)
        {
            chartView.UpdateChart(entries);
        }
    }

    private void UpdateChart(IEnumerable<ChartEntryWrapper> entries)
    {
        // Convert ChartEntryWrapper to ChartEntry
        var chartEntries = entries.Select(e => e.Entry);

        // Update the chart
        ChartView.Chart = new DonutChart
        {
            Entries = chartEntries
        };
    }
}