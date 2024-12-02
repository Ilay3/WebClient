using WebAiko.Domain.Entities;

namespace WebAiko.Application.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<Client> GetByIdAsync(long clientId);
        Task<IEnumerable<Client>> GetAllAsync();
        Task AddAsync(Client client);
        Task AddRangeAsync(IEnumerable<Client> clients);
        Task UpdateAsync(Client client);
        Task DeleteAsync(long clientId);
        Task<bool> ExistsAsync(long clientId);
        Task<IEnumerable<long>> GetExistingClientIdsAsync(IEnumerable<long> clientIds);
    }
}
