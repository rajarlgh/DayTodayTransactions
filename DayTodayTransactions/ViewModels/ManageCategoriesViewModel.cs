using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DayTodayTransactions.ViewModels
{
    public partial class ManageCategoriesViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> categories;

        [ObservableProperty]
        private string newCategory;

        public ManageCategoriesViewModel()
        {
            // Load existing categories (you can fetch these from a database or other storage)
            Categories = new ObservableCollection<string>
        {
            "Car", "Food", "Pet", "Health", "Cafe", "Bar", "Dental"
        };
        }

        [RelayCommand]
        private void AddCategory()
        {
            if (!string.IsNullOrWhiteSpace(this.NewCategory) && !Categories.Contains(NewCategory))
            {
                Categories.Add(NewCategory);
                NewCategory = string.Empty;
            }
        }

        [RelayCommand]
        private void RemoveCategory(string category)
        {
            if (Categories.Contains(category))
            {
                Categories.Remove(category);
            }
        }


    }
}