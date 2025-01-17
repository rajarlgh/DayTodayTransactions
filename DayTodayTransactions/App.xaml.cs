using DayTodayTransactions.Pages;
using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Register ViewModel with DI
            // DependencyService.Register<TransactionHistoryViewModel>(); // No need to register with DependencyService

            // Register other services here if needed

            // Resolve TransactionHistoryViewModel and pass it to AppShell
            MainPage = new AppShell(_serviceProvider, _serviceProvider.GetRequiredService<TransactionHistoryViewModel>());
        }
    }
}
