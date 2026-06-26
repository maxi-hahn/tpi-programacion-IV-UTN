using Application.Dtos.Responses;
using Application.Dtos.Request;
using Domain.Entity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Trabajop4.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetById(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task Add(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task Update(User user)
    {
        _context.Users.Update(user);
    }

    public async Task Delete(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    }
    //public async Task<SingInResponse?> SingIn(SingInRequest userData)
    //{
    //    var user = await GetByEmail(userData.Email);
    //    if (user == null) return null;
    //    // Aquí deberías usar un servicio de hashing para verificar la contraseña
    //    // Por ejemplo: var isValid = _hasher.Verify(userData.Password, user.Password);
    //    // Si no es válido, retorna null
    //    // Si es válido, retorna un SingInResponse con los datos del usuario
    //    return new SingInResponse(user.Id, user.Email);
    //}
}
