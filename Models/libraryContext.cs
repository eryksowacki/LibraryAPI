using Microsoft.EntityFrameworkCore;
using bibliotekaAPI.Models;

namespace bibliotekaAPI.Models
{
    public class libraryContext : DbContext
    {
        public libraryContext(DbContextOptions<libraryContext> options)
            : base(options)
        {
        }
        public DbSet<bibliotekaAPI.Models.Book> Books { get; set; } 
        public DbSet<bibliotekaAPI.Models.Shelf> Shelves { get; set; }
    }
}