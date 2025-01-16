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
        Task<List<Category>> GetCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task DeleteCategoryAsync(int? id);
        Task UpdateCategoryAsync(Category category);
    }
}
