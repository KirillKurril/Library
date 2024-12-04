using Microsoft.AspNetCore.Mvc;

namespace Library.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Author : ControllerBase
    {
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

        [HttpPost]
        public IActionResult Create(CreateAuthorDTO createAuthorDTO)
        {
            return Ok();
        }

        [HttpPut]
        public IActionResult Update(UpdateBookDTO updateAuthorDTO)
        {
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
