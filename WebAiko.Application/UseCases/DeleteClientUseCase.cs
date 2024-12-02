using WebAiko.Application.Interfaces.Repositories;

namespace WebAiko.Application.UseCases
{
    public class DeleteClientUseCase
    {
        private readonly IClientRepository _clientRepository;

        public DeleteClientUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task ExecuteAsync(long clientId)
        {
            if (await _clientRepository.ExistsAsync(clientId))
            {
                await _clientRepository.DeleteAsync(clientId);
            }
            else
            {
                throw new Exception("Клиент не найден.");
            }
        }
    }
}
