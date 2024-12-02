
using Moq;
using FluentAssertions;
using WebAiko.Application.UseCases;
using WebAiko.Application.Interfaces.Repositories;
using WebAiko.Domain.Entities;

namespace WebAiko.Tests.UseCases
{
    public class GetClientUseCaseTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly GetClientUseCase _getClientUseCase;

        public GetClientUseCaseTests()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _getClientUseCase = new GetClientUseCase(_clientRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnClient_WhenClientExists()
        {
            var clientId = 1L;
            var expectedClient = new Client
            {
                ClientId = clientId,
                Username = "TestUser",
                SystemId = Guid.NewGuid()
            };

            _clientRepositoryMock
                .Setup(repo => repo.GetByIdAsync(clientId))
                .ReturnsAsync(expectedClient);

            var result = await _getClientUseCase.ExecuteAsync(clientId);

            result.Should().NotBeNull();
            result.ClientId.Should().Be(expectedClient.ClientId);
            result.Username.Should().Be(expectedClient.Username);
            result.SystemId.Should().Be(expectedClient.SystemId);
            _clientRepositoryMock.Verify(repo => repo.GetByIdAsync(clientId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnNull_WhenClientDoesNotExist()
        {
            var clientId = 2L;

            _clientRepositoryMock
                .Setup(repo => repo.GetByIdAsync(clientId))
                .ReturnsAsync((Client)null);

            var result = await _getClientUseCase.ExecuteAsync(clientId);

            result.Should().BeNull();
            _clientRepositoryMock.Verify(repo => repo.GetByIdAsync(clientId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnAllClients_WhenClientIdIsZero()
        {
            var expectedClients = new[]
            {
                new Client { ClientId = 1, Username = "User1", SystemId = Guid.NewGuid() },
                new Client { ClientId = 2, Username = "User2", SystemId = Guid.NewGuid() }
            };

            _clientRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedClients);

            var result = await _getClientUseCase.ExecuteAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(expectedClients);
            _clientRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}
