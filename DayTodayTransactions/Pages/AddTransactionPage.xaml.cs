using DayTodayTransactions.ViewModels;

namespace DayTodayTransactions.Pages;
[QueryProperty(nameof(Type), "type")]  // Bind the 'type' query parameter to this property
[QueryProperty(nameof(TransactionViewModel), "TransactionViewModel")]
public partial class AddTransactionPage : ContentPage
{
    private  TransactionViewModel _transactionViewModel;

    public AddTransactionPage(TransactionViewModel viewModel)
    {
        InitializeComponent();
        _transactionViewModel = viewModel;
        this.BindingContext = _transactionViewModel;
    }
    // Property to receive the 'type' parameter
    public string Type
    {
        get => _transactionViewModel.Type;
        set => _transactionViewModel.Type = value;
    }
    public TransactionViewModel TransactionViewModel
    {
        get => _transactionViewModel;
        set
        {
            _transactionViewModel = value;
            OnPropertyChanged(nameof(TransactionViewModel));
            BindingContext = _transactionViewModel;
        }
    }
    protected override void OnNavigatedTo(Microsoft.Maui.Controls.NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        //// Retrieve the passed data
        //if (Shell.Current?.Navigation?.NavigationStack.LastOrDefault()?.BindingContext is TransactionViewModel transactionViewModel)
        //{
        //    // Populate the ViewModel
        //    _transactionViewModel.Amount = transactionViewModel.Amount;
        //    _transactionViewModel.Reason = transactionViewModel.Reason;
        //    _transactionViewModel.Type = transactionViewModel.Type;
        //    _transactionViewModel.Category = transactionViewModel.Category;
        //    _transactionViewModel.Date = transactionViewModel.Date;

        //    // Update the BindingContext
        //    BindingContext = _transactionViewModel;
        //}
    }



    // Handle receiving the transaction details for editing
    protected override void OnAppearing()
    {
        base.OnAppearing();

        //if (BindingContext is TransactionViewModel transactionViewModel)
        //{
        //    _transactionViewModel = transactionViewModel;

        //    // Populate fields
        //    AmountEntry.Text = _transactionViewModel.Amount.ToString();
        //    ReasonEntry.Text = _transactionViewModel.Reason;
        //    TypePicker.SelectedItem = _transactionViewModel.Type;
        //    CategoryPicker.SelectedItem = _transactionViewModel.Category;
        //    DatePicker.Date = _transactionViewModel.Date;
        //}
    }
}
