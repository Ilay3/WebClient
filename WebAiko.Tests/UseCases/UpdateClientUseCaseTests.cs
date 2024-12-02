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
    public class UpdateClientUseCaseTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly UpdateClientUseCase _updateClientUseCase;

        public UpdateClientUseCaseTests()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _updateClientUseCase = new UpdateClientUseCase(_clientRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUpdateClient_WhenClientExists()
        {
            var existingClient = new Client
            {
                ClientId = 1L,
                Username = "UpdatedUser",
                SystemId = Guid.NewGuid()
            };

            _clientRepositoryMock
                .Setup(repo => repo.ExistsAsync(existingClient.ClientId))
                .ReturnsAsync(true);

            _clientRepositoryMock
                .Setup(repo => repo.UpdateAsync(existingClient))
                .Returns(Task.CompletedTask);

            await _updateClientUseCase.ExecuteAsync(existingClient);

            _clientRepositoryMock.Verify(repo => repo.ExistsAsync(existingClient.ClientId), Times.Once);
            _clientRepositoryMock.Verify(repo => repo.UpdateAsync(existingClient), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenClientDoesNotExist()
        {
            var nonExistingClient = new Client
            {
                ClientId = 2L,
                Username = "NonExistingUser",
                SystemId = Guid.NewGuid()
            };

            _clientRepositoryMock
                .Setup(repo => repo.ExistsAsync(nonExistingClient.ClientId))
                .ReturnsAsync(false);

            Func<Task> act = async () => await _updateClientUseCase.ExecuteAsync(nonExistingClient);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Клиент не найден.");

            _clientRepositoryMock.Verify(repo => repo.ExistsAsync(nonExistingClient.ClientId), Times.Once);
            _clientRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Client>()), Times.Never);
        }
    }
}
