using WebAiko.Application.Interfaces.Repositories;
using WebAiko.Domain.Entities;

namespace WebAiko.Application.UseCases
{
    public class GetClientUseCase
    {
        private readonly IClientRepository _clientRepository;

        public GetClientUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public Task<Client> ExecuteAsync(long clientId)
        {
            return _clientRepository.GetByIdAsync(clientId);
        }

        public Task<IEnumerable<Client>> ExecuteAsync()
        {
            return _clientRepository.GetAllAsync();
        }
    }
}
