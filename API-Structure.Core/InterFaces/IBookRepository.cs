using API_Structure.Core.DTOs.BookDtos;
using API_Structure.Core.DTOs.CustomDtos;
using BookManagementSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.Repositories
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        Task<List<CustomBookDto?>> GetBooksByGenreAsync(string genreName);
        Task<string> AddToGenreAsync(AddBookToGenreDto dto);
        Task<bool> ExistsInGenreAsync(AddBookToGenreDto dto);
        Task<string> AddNewBook(BookDto dto);
    }
}
