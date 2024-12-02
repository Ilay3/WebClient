using Microsoft.EntityFrameworkCore;
using WebAiko.Application.Interfaces.Repositories;
using WebAiko.Domain.Entities;
using WebAiko.Infrastructure.Data;

namespace WebAiko.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Client> clients)
    {
        await _context.Clients.AddRangeAsync(clients);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client != null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long clientId)
    {
        return await _context.Clients.AnyAsync(c => c.ClientId == clientId);
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task<Client> GetByIdAsync(long clientId)
    {
        return await _context.Clients.FindAsync(clientId);
    }

    public async Task<IEnumerable<long>> GetExistingClientIdsAsync(IEnumerable<long> clientIds)
    {
        return await _context.Clients
            .Where(c => clientIds.Contains(c.ClientId))
            .Select(c => c.ClientId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }
}
