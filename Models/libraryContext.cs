using Microsoft.EntityFrameworkCore;
using bibliotekaAPI.Models;

namespace bibliotekaAPI.Models
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }
        public DbSet<bibliotekaAPI.Models.Book> Books { get; set; } 
        public DbSet<bibliotekaAPI.Models.Shelf> Shelves { get; set; }
    }
}