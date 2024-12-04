using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        [HttpGet]
        [Route("my-books")]
        public IActionResult GetBorrowedList(
            [FromQuery] int userId,
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage)
        {
            return Ok();
        }

        [HttpGet]
        [Route("catalog")]
        public IActionResult GetFiltredList(
            [FromQuery] string? searchTerm,
            [FromQuery] string? genre,
            [FromQuery] int? AuthorId,
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage)
        {
            return Ok();
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetAllList()
        {
            return Ok();
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById(int id)
        {
            return Ok();
        }

        [HttpGet]
        [Route("{isbn:string}")]
        public IActionResult GetByISBN(string isbn)
        {
            return Ok();
        }

        [HttpPost("{id}/borrow")]
        public IActionResult BorrowBook(int id)
        {
            return Ok();
        }

        [HttpPost("{id}/return")]
        public IActionResult ReturnBook(int id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Create(CreateBookDTO createBookDTO)
        {
            return Ok();
        }

        [HttpPut]
        public IActionResult Update(UpdateBookDTO updateBookDTO)
        {
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            return Ok();
        }

        [HttpPost("{id}/image")]
        public IActionResult UploadImage(int id, IFormFile image)
        {
            return Ok();
        }
    }
}
