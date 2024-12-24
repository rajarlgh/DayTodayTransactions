using System.Globalization;

namespace DayTodayTransactions.ViewModels
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "False" : "True"; // Flip the value
            }
            return "True"; // Default if not a boolean
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue == "True"; // Convert back to boolean
            }
            return false; // Default if not a recognized string
        }
    }
}
