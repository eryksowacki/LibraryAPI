using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bibliotekaAPI.Models;
using System.Threading;

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
        public Task<List<Book>> GetBooks(CancellationToken token)
        {
            return _context.Books.ToListAsync(token);
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

            if (!BookExists(id))
            {
                return NotFound();
            }
            //czy półka w ogóle istnieje?
            if (!ShelfExists(id))
            {
                return NotFound();
            }
            //sprawdzenie czy zmienił się numer półki 
            
            if (book.ShelfNumber != _context.Shelves.Select(s => s.Id).FirstOrDefault())
            {
                string error = "Na tej półce nie ma już miejsca";
                var numberOfBooksOnShelf = _context.Books.Where(b => b.ShelfNumber == book.ShelfNumber).Count();
                var maxNumberOfBooks = _context.Shelves.Where(s => s.Id == book.ShelfNumber).Select(s => s.NumberOfBooks).FirstOrDefault();

                if (numberOfBooksOnShelf > (maxNumberOfBooks - 1))
                {
                    return BadRequest(error);
                }
            }
            
            _context.Entry(book).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book, CancellationToken token, long id)
        {
            id = book.ShelfNumber;
            //czy półka w ogóle istnieje?
            if (!ShelfExists(id))
            {
                return NotFound();
            }
            string error = "Na tej półce nie ma już miejsca";
            var numberOfBooksOnShelf = _context.Books.Where(b => b.ShelfNumber == book.ShelfNumber).Count();
            var maxNumberOfBooks = _context.Shelves.Where(s => s.Id == book.ShelfNumber).Select(s => s.NumberOfBooks).First();

            if (numberOfBooksOnShelf > (maxNumberOfBooks-1))
            {
                return BadRequest(error);
            }
                    _context.Books.Add(book);
                        await _context.SaveChangesAsync(token);

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
        private bool ShelfExists(long id)
        {
            return _context.Shelves.Any(e => e.Id == id);
        }
    }
}
