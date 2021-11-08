using System;
 
namespace bibliotekaAPI.Models
{
    public class Book
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int ShelfNumber { get; set; }
    }
}