using System;

namespace bibliotekaAPI.Models
{
    public class Library
    {
        public long Id { get; set; }
        public int NumberOfShelves { get; set; }
        public int NumberOfBooks { get; set; }
    }
}