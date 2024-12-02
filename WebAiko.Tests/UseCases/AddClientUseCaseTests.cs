using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using WebAiko.Application.UseCases;
using WebAiko.Application.Interfaces.Repositories;
using WebAiko.Domain.Entities;

namespace WebAiko.Tests.UseCases
{
    public class AddClientUseCaseTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly AddClientUseCase _addClientUseCase;

        public AddClientUseCaseTests()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _addClientUseCase = new AddClientUseCase(_clientRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldAddClient_WhenClientDoesNotExist()
        {
            var newClient = new Client
            {
                ClientId = 1L,
                Username = "NewUser",
                SystemId = Guid.NewGuid()
            };

            _clientRepositoryMock
                .Setup(repo => repo.ExistsAsync(newClient.ClientId))
                .ReturnsAsync(false);

            _clientRepositoryMock
                .Setup(repo => repo.AddAsync(newClient))
                .Returns(Task.CompletedTask);

            await _addClientUseCase.ExecuteAsync(newClient);

            _clientRepositoryMock.Verify(repo => repo.ExistsAsync(newClient.ClientId), Times.Once);
            _clientRepositoryMock.Verify(repo => repo.AddAsync(newClient), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenClientExists()
        {
            var existingClient = new Client
            {
                ClientId = 2L,
                Username = "ExistingUser",
                SystemId = Guid.NewGuid()
            };

            _clientRepositoryMock
                .Setup(repo => repo.ExistsAsync(existingClient.ClientId))
                .ReturnsAsync(true);

            Func<Task> act = async () => await _addClientUseCase.ExecuteAsync(existingClient);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Клиент с таким идентификатором уже существует");

            _clientRepositoryMock.Verify(repo => repo.ExistsAsync(existingClient.ClientId), Times.Once);
            _clientRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Client>()), Times.Never);
        }
    }
}
