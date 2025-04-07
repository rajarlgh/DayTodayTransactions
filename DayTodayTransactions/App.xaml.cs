using DayTodayTransactions.Pages;
using DayTodayTransactions.ViewModels;
using DayTodayTransactionsLibrary.Interfaces;

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

            // Initialize DB in background
            Task.Run(InitializeAsync);
        }
        private async Task InitializeAsync()
        {
            try
            {
                var accountService = _serviceProvider.GetRequiredService<IAccountService>();
                await accountService.InitializeAsync();
                var categoryService = _serviceProvider.GetRequiredService<ICategoryService>();
                await categoryService.InitializeAsync();
            }
            catch (Exception ex)
            {
                // Log the exception, don't crash silently
                System.Diagnostics.Debug.WriteLine($"DB Init Failed: {ex.Message}");
            }
        }
    }
}
