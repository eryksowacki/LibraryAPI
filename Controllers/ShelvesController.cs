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
    [Route("api/[controller]")]
    [ApiController]
    public class ShelvesController : ControllerBase
    {
        private readonly LibraryContext _context;

        public ShelvesController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shelf>>> GetShelves()
        {
            return await _context.Shelves.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shelf>> GetShelf(long id)
        {
            var shelf = await _context.Shelves.FindAsync(id);

            if (shelf == null)
            {
                return NotFound();
            }

            return shelf;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutShelf(long id, Shelf shelf)
        {
            if (id != shelf.Id)
            {
                return BadRequest();
            }

            _context.Entry(shelf).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShelfExists(id))
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
        public async Task<ActionResult<Shelf>> PostShelf(Shelf shelf)
        {
            _context.Shelves.Add(shelf);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShelf), new { id = shelf.Id }, shelf);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShelf(long id)
        {
            var shelf = await _context.Shelves.FindAsync(id);
            if (shelf == null)
            {
                return NotFound();
            }

            _context.Shelves.Remove(shelf);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShelfExists(long id)
        {
            return _context.Shelves.Any(e => e.Id == id);
        }
    }
}
