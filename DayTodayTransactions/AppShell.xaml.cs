using DayTodayTransactions.Pages;

namespace DayTodayTransactions
{
    public partial class AppShell : Shell
    {
        private readonly IServiceProvider _serviceProvider;

        // Constructor accepting IServiceProvider
        public AppShell(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Register your routes here
            Routing.RegisterRoute(nameof(AddTransactionPage), typeof(AddTransactionPage));
            Routing.RegisterRoute(nameof(TransactionHistoryPage), typeof(TransactionHistoryPage));
        }
    }
}