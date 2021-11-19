using System;

namespace bibliotekaAPI.Models
{
    public class Shelf
    {
        public long Id { get; set; }
        public string Category { get; set; }
        public int NumberOfBooks { get; set; }
    }
}