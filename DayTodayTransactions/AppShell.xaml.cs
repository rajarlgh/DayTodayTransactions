using DayTodayTransactions.Pages;
using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions
{
    public partial class AppShell : Shell
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TransactionHistoryViewModel _viewModel;
        // Constructor accepting IServiceProvider
        public AppShell(IServiceProvider serviceProvider, TransactionHistoryViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            this.BindingContext = _viewModel;

            _serviceProvider = serviceProvider;

            // Register your routes here
            Routing.RegisterRoute(nameof(AddTransactionPage), typeof(AddTransactionPage));
            Routing.RegisterRoute(nameof(TransactionHistoryPage), typeof(TransactionHistoryPage));
            Routing.RegisterRoute(nameof(ManageCategoriesPage), typeof(ManageCategoriesPage));
            Routing.RegisterRoute(nameof(ManageAccountsPage), typeof(ManageAccountsPage));
            Routing.RegisterRoute(nameof(ExcelUploaderPage), typeof(ExcelUploaderPage));
        }
    }
}