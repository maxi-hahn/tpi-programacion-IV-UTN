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
}