using Application.Dtos.Request;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;
using System.Text.RegularExpressions;

public class UserService : IUserService
{
    protected readonly IUserRepository _repo;
    protected readonly IPasswordHasherService _hasher;
    protected readonly IUserContext _userContext;
    public UserService(IUserRepository repo, IPasswordHasherService hasher, IUserContext userContext)
    {
        _repo = repo;
        _hasher = hasher;
        _userContext = userContext;
    }

    
    public async Task<User?> UpdateUser(UpdateUserRequest request)
    {
        var user = await _repo.GetById(_userContext.UserId);

        if (user == null)
            throw new NotFoundException("User not found");

        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name.Length < 3)
        {
            throw new ValidationException("Invalid Name");
        }

        string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!string.IsNullOrWhiteSpace(request.Email) &&
        !Regex.IsMatch(request.Email, patron))
        {
            throw new ValidationException("Invalid Email");
        }

        user.Name = request.Name ?? user.Name;
        user.Email = request.Email ?? user.Email;
        user.Password = request.Password != null ? _hasher.Hash(request.Password) : user.Password;
        await _repo.Update(user);
        await _repo.Save();
        return user;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var user = await _repo.GetAll();

        return user.FirstOrDefault(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _repo.GetAll();
    }

    public async Task<User?> GetById(Guid id)
    {
        var user = await _repo.GetById(id);
        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }

    public async Task<User> Create(User user)
    {
        if (user == null) throw new BadRequestException("Invalid Data");
        if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
            throw new ValidationException("Name and Email are required.");

        user.Id = Guid.NewGuid();
        user.Password = _hasher.Hash(user.Password);

        await _repo.Add(user);
        await _repo.Save();

        return user;
    }

    public async Task Update(Guid id, User updatedUser)
    {
        var user = await _repo.GetById(id);

        if (user == null)
            throw new NotFoundException("User not found");

        user.Name = updatedUser.Name ?? user.Name;
        user.Email = updatedUser.Email ?? user.Email;

        await _repo.Update(user);
        await _repo.Save();
    }
    public async Task Delete(Guid id)
    {
        var user = await _repo.GetById(id);
        if (user == null)
            throw new NotFoundException("User not found");
        await _repo.Delete(user);
        await _repo.Save();
    }
}