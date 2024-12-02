using WebAiko.Application.Interfaces.Repositories;
using WebAiko.Domain.Entities;

namespace WebAiko.Application.UseCases
{
    public class AddClientUseCase
    {
        private readonly IClientRepository _clientRepository;

        public AddClientUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task ExecuteAsync(Client client)
        {
            if (!await _clientRepository.ExistsAsync(client.ClientId))
            {
                await _clientRepository.AddAsync(client);
            }
            else
            {
                throw new Exception("Клиент с таким идентификатором уже существует.");
            }
        }
    }
}
