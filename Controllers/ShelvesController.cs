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
        public  Task<List<Shelf>> GetShelves(CancellationToken token)
        {
            return  _context.Shelves.ToListAsync(token);
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

            if (!ShelfExists(id))
            {
                return NotFound();
            }

            //zabezpieczenie przed zmiana ilosci ksiazek
            if (shelf.NumberOfBooks < _context.Shelves.Select(s => s.NumberOfBooks).FirstOrDefault())
            {
                string error = "Nie można zmienić ilości książek na mniejszą niż zadeklarowana";
                return BadRequest(error);
            }

            _context.Entry(shelf).State = EntityState.Modified;
            
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
        public async Task<ActionResult<Shelf>> PostShelf(Shelf shelf, CancellationToken token)
        {
            _context.Shelves.Add(shelf);
            await _context.SaveChangesAsync(token);

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
