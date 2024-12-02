using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using WebAiko.Application.UseCases;
using WebAiko.Application.Interfaces.Repositories;

namespace WebAiko.Tests.UseCases
{
    public class DeleteClientUseCaseTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly DeleteClientUseCase _deleteClientUseCase;

        public DeleteClientUseCaseTests()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _deleteClientUseCase = new DeleteClientUseCase(_clientRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldDeleteClient_WhenClientExists()
        {
            var clientId = 1L;

            _clientRepositoryMock
                .Setup(repo => repo.ExistsAsync(clientId))
                .ReturnsAsync(true);

            _clientRepositoryMock
                .Setup(repo => repo.DeleteAsync(clientId))
                .Returns(Task.CompletedTask);

            await _deleteClientUseCase.ExecuteAsync(clientId);

            _clientRepositoryMock.Verify(repo => repo.ExistsAsync(clientId), Times.Once);
            _clientRepositoryMock.Verify(repo => repo.DeleteAsync(clientId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenClientDoesNotExist()
        {
            var clientId = 2L;

            _clientRepositoryMock
                .Setup(repo => repo.ExistsAsync(clientId))
                .ReturnsAsync(false);

            Func<Task> act = async () => await _deleteClientUseCase.ExecuteAsync(clientId);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Клиент не найден.");

            _clientRepositoryMock.Verify(repo => repo.ExistsAsync(clientId), Times.Once);
            _clientRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<long>()), Times.Never);
        }
    }
}
