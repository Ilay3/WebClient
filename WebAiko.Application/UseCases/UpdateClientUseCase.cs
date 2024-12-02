using WebAiko.Application.Interfaces.Repositories;
using WebAiko.Domain.Entities;

namespace WebAiko.Application.UseCases
{
    public class UpdateClientUseCase
    {
        private readonly IClientRepository _clientRepository;

        public UpdateClientUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task ExecuteAsync(Client client)
        {
            if (await _clientRepository.ExistsAsync(client.ClientId))
            {
                await _clientRepository.UpdateAsync(client);
            }
            else
            {
                throw new Exception("Клиент не найден.");
            }
        }
    }
}
