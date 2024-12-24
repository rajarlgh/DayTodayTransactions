using DayTodayTransactions.Pages;
using DayTodayTransactions.ViewModels;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsService;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DayTodayTransactions
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
            .UseMauiApp<App>()
                .UseMicrocharts()
                //.UseSkiaSharp() // Register SkiaSharp handlers
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

            // Register pages and their ViewModels with the required dbPath
            RegisterPageWithViewModel<TransactionViewModel, AddTransactionPage>(builder);
            RegisterPageWithViewModel<TransactionHistoryViewModel, TransactionHistoryPage>(builder, dbPath);

            // Update to use the App constructor that accepts IServiceProvider
            builder.Services.AddSingleton<App>();

            return builder.Build();
        }

        // Common function to register pages and ViewModels with a factory for ViewModel creation
        private static void RegisterPageWithViewModel<TViewModel, TPage>(MauiAppBuilder builder, string dbPath = null)
            where TViewModel : class
            where TPage : class
        {
            // If dbPath is provided, use a factory to pass the dbPath to the ViewModel constructor
            if (dbPath != null)
            {
                builder.Services.AddSingleton<TViewModel>(provider =>
                {
                    return (TViewModel)Activator.CreateInstance(typeof(TViewModel), dbPath);
                });
            }
            else
            {
                builder.Services.AddSingleton<TViewModel>();
            }

            builder.Services.AddSingleton<TPage>();
        }
    }
}
