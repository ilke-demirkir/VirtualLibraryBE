using Microsoft.AspNetCore.Mvc;
using VirtualLibraryAPI.Models;
using VirtualLibraryAPI.Services;
using VirtualLibraryAPI.Data;
namespace VirtualLibraryAPI.Controllers{

    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase{
        private readonly BookService _service;
        private readonly LibraryDbContext _context;
        public BooksController(BookService service, LibraryDbContext context){
            _service = service;
            _context = context;
        }

        [HttpGet]
        public ActionResult<Book> GetAll()
        {
            return Ok(_service.GetAll());
        }
        [HttpGet("{id}")]
        public ActionResult<Book> GetById(int id){
            var book = _service.GetById(id);
            return book is null ? NotFound(): Ok(book);
        }

        [HttpPost]
        public IActionResult Create([FromBody]Book book)
        {
            Console.WriteLine($"Received book: {book}");
            _service.Add(book);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        
        public IActionResult Update(int id, Book updatedBook){

            var book = _service.GetById(id);
            if (book is null) return NotFound();
            updatedBook.Id = id;
            _service.Update(updatedBook);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var book = _service.GetById(id);
            if (book is null) return NotFound();
            _service.Delete(id);
            return NoContent();
        }

    }
}