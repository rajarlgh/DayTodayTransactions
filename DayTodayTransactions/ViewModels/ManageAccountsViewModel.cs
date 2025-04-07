using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using System.Collections.ObjectModel;

namespace DayTodayTransactions.ViewModels
{
    public partial class ManageAccountsViewModel : ObservableObject
    {
        private readonly IAccountService _accountService;

        [ObservableProperty]
        private ObservableCollection<Account> accounts;

        [ObservableProperty]
        private string newAccountName;

        [ObservableProperty]
        private int? accountId;

        public ManageAccountsViewModel(IAccountService accountService)
        {
            _accountService = accountService;
            Accounts = new ObservableCollection<Account>();
            LoadAccountsAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        public async Task LoadAccountsAsync()
        {
            try
            {
                var accounts = await _accountService.GetAccountsAsync();
                Accounts = new ObservableCollection<Account>(accounts);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        public async Task AddOrUpdateAccountAsync()
        {
            if (string.IsNullOrWhiteSpace(NewAccountName))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Account name cannot be empty.", "OK");
                return;
            }

            var account = new Account
            {
                
                Id =   AccountId.Value,
                Name = NewAccountName,
            };

            try
            {
                if (account.Id == null || account.Id == 0)
                {
                    // Add new account
                    await _accountService.AddAccountAsync(account);

                    // Reload or add directly to Accounts
                    var savedAccounts = await _accountService.GetAccountsAsync();
                    account = savedAccounts.LastOrDefault(a => a.Name == NewAccountName);
                    Accounts.Add(account);
                }
                else
                {
                    // Update existing account
                    await _accountService.UpdateAccountAsync(account);

                    var existingAccount = Accounts.FirstOrDefault(a => a.Id == account.Id);
                    if (existingAccount != null)
                    {
                        existingAccount.Name = account.Name;

                        // Notify the UI of the change
                        var index = Accounts.IndexOf(existingAccount);
                        Accounts[index] = existingAccount;
                    }
                }

                await Application.Current.MainPage.DisplayAlert("Success", "Account saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }

            // Reset form
            NewAccountName = string.Empty;
            AccountId = 0;
        }

        [RelayCommand]
        public async Task EditAccountAsync(Account account)
        {
            if (account == null) return;

            // Populate fields for editing
            AccountId = account.Id;
            NewAccountName = account.Name;
        }

        [RelayCommand]
        public async Task DeleteAccountAsync(Account account)
        {
            if (account == null || !Accounts.Contains(account)) return;

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete the account '{account.Name}'?",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    await _accountService.DeleteAccountAsync(account.Id);
                    Accounts.Remove(account);
                    await Application.Current.MainPage.DisplayAlert("Success", "Account removed successfully.", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
    }
}
