using Application.Services;
using Application.Tools;
using Domain.Entity.UsersChild;
using Domain.Interface.UsersInterface;


public class ClientService : IClientService
{
    private readonly IClientRepository _repo;

    private readonly IPasswordHasherService _hasher;

    public ClientService(IClientRepository repo, IPasswordHasherService hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public async Task<IEnumerable<Client>> GetAll()
    {
        return await _repo.GetAll();
    }

    public async Task<Client?> GetById(Guid id)
    {
        return await _repo.GetById(id);
    }

    public async Task<Client> Create(Client client)
    {
        client.Id = Guid.NewGuid();
        client.Password = _hasher.Hash(client.Password);

        await _repo.Add(client);
        await _repo.Save();

        return client;
    }

    public async Task<bool> Update(Guid id, Client updatedClient)
    {
        var client = await _repo.GetById(id);
        if (client == null) return false;

        client.Name = updatedClient.Name ?? client.Name;
        client.Email = updatedClient.Email ?? client.Email;

        await _repo.Update(client);
        await _repo.Save();

        return true;
    }

    public async Task<bool> Delete(Guid id)
    {
        var client = await _repo.GetById(id);
        if (client == null) return false;

        await _repo.Delete(client);
        await _repo.Save();

        return true;
    }
}