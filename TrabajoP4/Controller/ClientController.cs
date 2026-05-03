using Domain.Entity;
using Domain.Entity.UsersChild;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trabajop4.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

   
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> Get()
        {
            return await _context.Clients.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            return client;
        }


        [HttpPost]
        public async Task<ActionResult<Client>> Post(Client client)
        {
            client.Id = Guid.NewGuid();


            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }
    

       [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, Client updatedClient)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            // Ajustá según tus propiedades reales
            client.Name = updatedClient.Name ?? client.Name;
            client.Email = updatedClient.Email ?? client.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
