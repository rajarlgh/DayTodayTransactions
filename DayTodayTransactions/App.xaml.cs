using DayTodayTransactions.Pages;

namespace DayTodayTransactions
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Resolve the AddTransactionPage from the DI container
            //MainPage = new NavigationPage(_serviceProvider.GetRequiredService<AddTransactionPage>());
            MainPage = new NavigationPage(_serviceProvider.GetRequiredService<TransactionHistoryPage>());
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}
    }
}