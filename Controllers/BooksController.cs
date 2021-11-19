using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bibliotekaAPI.Models;

namespace bibliotekaAPI.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(long id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(long id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            string error = "Na tej półce nie ma już miejsca";
            var numberOfBooksOnShelf = _context.Books.Where(b => b.ShelfNumber == book.ShelfNumber).Count();
            //var numberOfShelves = _context.Shelves.Select(s => s.Id).Count();
            /*if(numberOfShelves < book.ShelfNumber){
                return BadRequest(); 
            }*/
            var maxNumberOfBooks = _context.Shelves.Where(s => s.Id == book.ShelfNumber).Select(s => s.NumberOfBooks).First();

            if (numberOfBooksOnShelf > (maxNumberOfBooks - 1))
            {
                return BadRequest(error);
            }

            _context.Entry(book).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            string error = "Na tej półce nie ma już miejsca";
            var numberOfBooksOnShelf = _context.Books.Where(b => b.ShelfNumber == book.ShelfNumber).Count();
            //var numberOfShelves = _context.Shelves.Select(s => s.Id).Count();//ilosc półek
            var maxNumberOfBooks = _context.Shelves.Where(s => s.Id == book.ShelfNumber).Select(s => s.NumberOfBooks).First();

            if (numberOfBooksOnShelf > (maxNumberOfBooks-1))
            {
                return BadRequest(error);
            }
            /*if(numberOfShelves < book.ShelfNumber){
                return BadRequest(); 
            }*/ 
            
                    _context.Books.Add(book);
                        await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(long id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(long id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
