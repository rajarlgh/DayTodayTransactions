using DayTodayTransactions.Pages;
using DayTodayTransactions.ViewModels;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsService;
using Microsoft.Extensions.Logging;

namespace DayTodayTransactions
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Dependency Injection Registration
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "expenses.db");
            builder.Services.AddSingleton<ITransactionService>(new TransactionService(dbPath));

            builder.Services.AddSingleton<TransactionViewModel>();
            builder.Services.AddSingleton<AddTransactionPage>();

            // Update to use the App constructor that accepts IServiceProvider
            builder.Services.AddSingleton<App>();

            return builder.Build();
        }
    }
}
