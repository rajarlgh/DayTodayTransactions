namespace Core.Controls.Controls;

public partial class CardView : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
           nameof(Title), typeof(string), typeof(CardView), string.Empty);

    public CardView()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}