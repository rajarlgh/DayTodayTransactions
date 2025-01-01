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
            DependencyService.Register<TransactionHistoryViewModel>();

            // Register other services here if needed
            MainPage = new AppShell(_serviceProvider);
        }

    }
}