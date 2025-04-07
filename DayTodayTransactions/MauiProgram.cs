using DayTodayTransactions.Pages;
using DayTodayTransactions.ViewModels;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsService;
using Microcharts.Maui;
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

            // Determine the database path based on the platform
            string dbPath = GetDatabasePath();

            // Dependency Injection Registration
            builder.Services.AddSingleton<ITransactionService>(new TransactionService(dbPath));
            builder.Services.AddSingleton<IAccountService>(provider =>
            {
                //var dbPath = Path.Combine(FileSystem.AppDataDirectory, "transactions.db3");
                return new AccountService(dbPath); // Don't call InitializeAsync here
            });


            builder.Services.AddSingleton<ICategoryService>(

                provider => {
                    return new CategoryService(dbPath);
                    });
            //builder.Services.AddSingleton<IAccountService>(provider => provider.GetRequiredService<AccountService>());

            // Register pages and their ViewModels with the required dbPath
            RegisterPageWithViewModel<TransactionViewModel, AddTransactionPage>(builder);
            RegisterPageWithViewModel<TransactionHistoryViewModel, TransactionHistoryPage>(builder, dbPath);
            RegisterPageWithViewModel<ManageCategoriesViewModel, ManageCategoriesPage>(builder);
            RegisterPageWithViewModel<ManageAccountsViewModel, ManageAccountsPage>(builder);
            RegisterPageWithViewModel<ExcelUploaderViewModel, ExcelUploaderPage>(builder);

            // Update to use the App constructor that accepts IServiceProvider
            builder.Services.AddSingleton<App>();
           

            return builder.Build();
        }

        private static string GetDatabasePath()
        {
            // Use platform-specific code to determine the database path
#if ANDROID
        return Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath ?? FileSystem.AppDataDirectory, "expenses.db");
#elif WINDOWS
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "expenses.db");
#else
            return Path.Combine(FileSystem.AppDataDirectory, "expenses.db");
#endif
        }

        // Common function to register pages and ViewModels with a factory for ViewModel creation
        private static void RegisterPageWithViewModel<TViewModel, TPage>(MauiAppBuilder builder, string dbPath = null)
            where TViewModel : class
            where TPage : class
        {
            // If dbPath is provided, use a factory to pass both the dbPath and ITransactionService to the ViewModel constructor
            if (dbPath != null)
            {
                builder.Services.AddSingleton<TViewModel>(provider =>
                {
                    var transactionService = provider.GetRequiredService<ITransactionService>();
                    var accountService = provider.GetRequiredService<IAccountService>();
                    var categoryService = provider.GetRequiredService<ICategoryService>();

                    return (TViewModel)Activator.CreateInstance(typeof(TViewModel), dbPath, transactionService, accountService, categoryService);
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