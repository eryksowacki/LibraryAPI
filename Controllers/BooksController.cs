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
        private readonly libraryContext _context;

        public BooksController(libraryContext context)
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

            var shelfNumber = book.ShelfNumber;
            var ids = _context.Books.Select(b => b.Id);
            int numberOfBooks = ids.Count();

            if((shelfNumber == 1 || shelfNumber == 2 || shelfNumber == 3) && numberOfBooks > 2)
            {
                _context.Entry(book).State = EntityState.Modified;
            }

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
        public async Task<ActionResult<Book>> PostBook(Book book)//, int shelfNumber, int numberOfBooks)
        {
            //var shelfNumber = _context.Books.Select(b => b.ShelfNumber);
            //int numberOfShelves = shelfNumber.Count();
            var shelfNumber = book.ShelfNumber;

            string firstError = "W bazie znajdują się tylko półki o numerach 1, 2 i 3";
            string secondError = "Na tej półce nie ma już miejsca na kolejną książkę.";
            var id = _context.Books.Select(b => b.Id);
            int numberOfBooks = id.Count();
           
            if(shelfNumber < 1 && shelfNumber > 3)
            {
                return BadRequest(firstError);
            }
            
            if(numberOfBooks > 2)
            {
                return BadRequest(secondError);
            }

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
