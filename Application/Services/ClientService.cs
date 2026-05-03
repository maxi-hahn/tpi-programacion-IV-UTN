using Application.Services;
using Domain.Entity.UsersChild;
using Microsoft.EntityFrameworkCore;
using Trabajop4.Infrastructure;
public class ClientService : IClientService
{
    private readonly ApplicationDbContext _context;

    public ClientService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetAll()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task<Client?> GetById(Guid id)
    {
        return await _context.Clients.FindAsync(id);
    }

    public async Task<Client> Create(Client client)
    {
        client.Id = Guid.NewGuid();
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<bool> Update(Guid id, Client updatedClient)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return false;

        client.Name = updatedClient.Name ?? client.Name;
        client.Email = updatedClient.Email ?? client.Email;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return false;

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
}