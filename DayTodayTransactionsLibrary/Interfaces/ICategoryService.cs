using DayTodayTransactionsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayTodayTransactionsLibrary.Interfaces
{
    public interface ICategoryService
    {
        Task InitializeAsync();

        Task<List<Category>> GetCategoriesAsync();
        Task<Category> AddCategoryAsync(Category category);
        
        Task DeleteCategoryAsync(int? id);
        Task UpdateCategoryAsync(Category category);
    }
}
