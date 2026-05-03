
using Domain.Entity.UsersChild;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientController(IClientService service)
        {
            _service = service;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> Get()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(Guid id)
        {
            var client = await _service.GetById(id);
            if (client == null) return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> Post(Client client)
        {

            if (client == null) return NotFound();
            if (string.IsNullOrEmpty(client.Name)) return BadRequest("Name is required.");
            if (string.IsNullOrEmpty(client.Email)) return BadRequest("Email is required.");
            if (string.IsNullOrEmpty(client.Password)) return BadRequest("Password is required.");

            var created = await _service.Create(client);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
            
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, Client client)
        {
            var updated = await _service.Update(id, client);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.Delete(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
