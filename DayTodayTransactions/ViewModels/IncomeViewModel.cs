using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Controls.Controls;
using Microcharts;
using System.Collections.ObjectModel;

namespace DayTodayTransactions.ViewModels
{
    public partial class IncomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isIncomeExpanded = false;

        [ObservableProperty]
        private ObservableCollection<ChartEntryWrapper> incomeChartEntryWrappers;

        [RelayCommand]
        private void ToggleIncome()
        {
            IsIncomeExpanded = !IsIncomeExpanded;
        }

        private void LoadTransactionsAndSetGrid(IList<Transaction> transactions)
        {
            var incomeGroupedData = transactions
                .Where(t => t.Category != null && t.Type == "Income")
                .GroupBy(t => t.Category.Id)
                .Select(g =>
                {
                    var first = g.First();
                    return new
                    {
                        CategoryId = g.Key,
                        CategoryName = first.Category.Name,
                        TotalAmount = g.Sum(t => t.Amount)
                    };
                });

            ColorCode cc = new ColorCode();
            var incomeData = incomeGroupedData.Select(data => new ChartEntryWrapper
            {
                Entry = new ChartEntry((float)data.TotalAmount)
                {
                    Label = data.CategoryName,
                    ValueLabel = data.TotalAmount.ToString("F0"),
                    Color = cc.GetCategoryColor(data.CategoryName)
                },
                CategoryId = data.CategoryId
            }).ToList();

            // Set collection of ChartEntryWrapper for CollectionView
            IncomeChartEntryWrappers = new ObservableCollection<ChartEntryWrapper>(incomeData);

            OnPropertyChanged(nameof(IncomeChartEntryWrappers));

            //ChartEntryWrapper cew = new ChartEntryWrapper();
            //// Recreate charts
            //IncomeChart = cew.CreateChart(IncomeChartEntryWrappers);
        }
    }
}
