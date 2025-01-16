using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DayTodayTransactionsService
{
    public class CategoryService : ICategoryService
    {
        private readonly SQLiteAsyncConnection _database;

        public CategoryService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Category>().Wait();
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return _database.Table<Category>().ToListAsync();
        }

        public Task AddCategoryAsync(Category category)
        {
            return _database.InsertAsync(category);
        }

        public Task DeleteCategoryAsync(int? id)
        {
            return _database.DeleteAsync<Category>(id);
        }

        public Task UpdateCategoryAsync(Category category)
        {
            return _database.UpdateAsync(category);
        }
    }
}
