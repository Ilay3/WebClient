using WebAiko.Application.Interfaces.Repositories;
using WebAiko.Domain.Entities;

namespace WebAiko.Application.UseCases
{
    public class AddMultipleClientsUseCase
    {
        private readonly IClientRepository _clientRepository;

        public AddMultipleClientsUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Client>> ExecuteAsync(IEnumerable<Client> clients)
        {
            var clientList = clients.ToList();
            var clientIds = clientList.Select(c => c.ClientId).ToList();

            var existingClientIds = await _clientRepository.GetExistingClientIdsAsync(clientIds);

            var uniqueClients = clientList.Where(c => !existingClientIds.Contains(c.ClientId)).ToList();

            if (uniqueClients.Any())
            {
                await _clientRepository.AddRangeAsync(uniqueClients);
            }

            var duplicateClients = clientList.Where(c => existingClientIds.Contains(c.ClientId)).ToList();

            return duplicateClients;
        }
    }
}
