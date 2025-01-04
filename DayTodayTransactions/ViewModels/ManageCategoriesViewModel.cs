using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using System.Collections.ObjectModel;

namespace DayTodayTransactions.ViewModels
{
    public partial class ManageCategoriesViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> categories;

        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private ObservableCollection<Category> listOfCategories;

        [ObservableProperty]
        private string newCategory;

        private ICategoryService _categoryService;
        public ManageCategoriesViewModel(ICategoryService categoryService)
        {
            // Load existing categories (you can fetch these from a database or other storage)
            Categories = new ObservableCollection<string>
            {
                "Car", "Food", "Pet", "Health", "Cafe", "Bar", "Dental"
            };
            _categoryService = categoryService;
            var categories = _categoryService.GetCategoriesAsync().Result;
            listOfCategories = new ObservableCollection<Category>(categories);
        }

        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            if (!string.IsNullOrWhiteSpace(NewCategory))
            {
                var category = new Category
                {
                    Id = Id,
                    Name = NewCategory,
                };

                try
                {
                    if (category.Id == 0)
                    {
                        // Add new category
                        await _categoryService.AddCategoryAsync(category);

                        // Reload categories to get the latest ID
                        var savedCategories = await _categoryService.GetCategoriesAsync();
                        category = savedCategories.LastOrDefault(c => c.Name == NewCategory);

                        // Add to observable collection
                        ListOfCategories.Add(category);
                    }
                    else
                    {
                        // Update existing category
                        await _categoryService.UpdateCategoryAsync(category);

                        // Find and update the category in the observable collection
                        var existingCategory = ListOfCategories.FirstOrDefault(c => c.Id == category.Id);
                        if (existingCategory != null)
                        {
                            existingCategory.Name = category.Name;

                            // Notify the UI of the change
                            var index = ListOfCategories.IndexOf(existingCategory);
                            ListOfCategories[index] = existingCategory;
                        }
                    }

                    // Show success message
                    await Application.Current.MainPage.DisplayAlert("Success", "Category saved successfully.", "OK");
                }
                catch (Exception ex)
                {
                    // Handle exception
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }

                // Reset form properties
                NewCategory = string.Empty;
                Id = 0;
            }
        }


        //[RelayCommand]
        //private void RemoveCategory(string category)
        //{
        //    if (Categories.Contains(category))
        //    {
        //        Categories.Remove(category);
        //    }
        //}

        [RelayCommand]
        private async Task RemoveCategoryAsync(Category category)
        {
            if (category != null && ListOfCategories.Contains(category))
            {
                try
                {
                    await _categoryService.DeleteCategoryAsync(category.Id);
                    ListOfCategories.Remove(category);

                    await Application.Current.MainPage.DisplayAlert("Success", "Category removed successfully.", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }


    }
}