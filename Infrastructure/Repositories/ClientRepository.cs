using Domain.Entity.UsersChild;
using Domain.Interface.UsersInterface;
using Microsoft.EntityFrameworkCore;
using Trabajop4.Infrastructure;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public ClientRepository(ApplicationDbContext context)
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

    public async Task Add(Client client)
    {
        await _context.Clients.AddAsync(client);
    }

    public async Task Update(Client client)
    {
        _context.Clients.Update(client);
    }

    public async Task Delete(Client client)
    {
        _context.Clients.Remove(client);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}