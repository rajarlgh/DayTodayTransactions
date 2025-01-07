using CommunityToolkit.Mvvm.ComponentModel;
using DayTodayTransactions.ViewModels;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using Microcharts;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DayTodayTransactions.Pages
{
    // Inherit from ObservableObject to enable property change notifications
    public partial class TransactionHistoryPage : ContentPage, INotifyPropertyChanged
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

        public Category SelectedCategory { get; set; } = new Category();
        public string CategoryType { get; set; }



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
                viewModel?.ShowBreakdownForCategory(this.SelectedCategory, this.CategoryType);
                donutIncomeChart.Chart = _viewModel.IncomeChart;
                donutExpenseChart.Chart = _viewModel.ExpenseChart;
            }
        }

        private void OnIncomeItemSelected(object sender, SelectionChangedEventArgs e)
        {
            // Deselect the previously selected item in CollectionView2
            var selectedEntry = e.CurrentSelection.FirstOrDefault() as ChartEntry;
            if (selectedEntry != null)
            {
                // Assuming the Label is the Category name
                this.SelectedCategory.Name = selectedEntry.Label;

                // Call the method in the ViewModel to fetch the breakdown
                var viewModel = BindingContext as TransactionHistoryViewModel;
                this.CategoryType = "Income";
                viewModel?.ShowBreakdownForCategory(this.SelectedCategory, this.CategoryType);
                collectionViewExpense.SelectedItem = null;
            }
        }

        private void OnExpenseItemSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedEntry = e.CurrentSelection.FirstOrDefault() as ChartEntry;
            if (selectedEntry != null)
            {
                // Assuming the Label is the Category name
                this.SelectedCategory.Name = selectedEntry.Label;

                // Call the method in the ViewModel to fetch the breakdown
                var viewModel = BindingContext as TransactionHistoryViewModel;
                this.CategoryType = "Expense";
                viewModel?.ShowBreakdownForCategory(this.SelectedCategory, this.CategoryType);
                collectionViewIncome.SelectedItem = null;
            }
        }
    }
}
