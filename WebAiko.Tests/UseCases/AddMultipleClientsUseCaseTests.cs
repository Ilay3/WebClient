using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using WebAiko.Application.UseCases;
using WebAiko.Application.Interfaces.Repositories;
using WebAiko.Domain.Entities;

namespace WebAiko.Tests.UseCases
{
    public class AddMultipleClientsUseCaseTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly AddMultipleClientsUseCase _addMultipleClientsUseCase;

        public AddMultipleClientsUseCaseTests()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _addMultipleClientsUseCase = new AddMultipleClientsUseCase(_clientRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldAddUniqueClients_AndReturnDuplicates()
        {
            var inputClients = new List<Client>
            {
                new Client { ClientId = 1, Username = "User1", SystemId = Guid.NewGuid() },
                new Client { ClientId = 2, Username = "User2", SystemId = Guid.NewGuid() },
                new Client { ClientId = 3, Username = "User3", SystemId = Guid.NewGuid() },
                new Client { ClientId = 4, Username = "User4", SystemId = Guid.NewGuid() },
                new Client { ClientId = 5, Username = "User5", SystemId = Guid.NewGuid() },
                new Client { ClientId = 6, Username = "User6", SystemId = Guid.NewGuid() },
                new Client { ClientId = 7, Username = "User7", SystemId = Guid.NewGuid() },
                new Client { ClientId = 8, Username = "User8", SystemId = Guid.NewGuid() },
                new Client { ClientId = 9, Username = "User9", SystemId = Guid.NewGuid() },
                new Client { ClientId = 10, Username = "User10", SystemId = Guid.NewGuid() }
            };

            var existingClientIds = new List<long> { 1, 3, 5 };

            _clientRepositoryMock
                .Setup(repo => repo.GetExistingClientIdsAsync(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(existingClientIds);

            _clientRepositoryMock
                .Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Client>>()))
                .Returns(Task.CompletedTask);

            var duplicateClients = await _addMultipleClientsUseCase.ExecuteAsync(inputClients);


            _clientRepositoryMock.Verify(repo => repo.GetExistingClientIdsAsync(It.IsAny<IEnumerable<long>>()), Times.Once);
            _clientRepositoryMock.Verify(repo => repo.AddRangeAsync(It.Is<IEnumerable<Client>>(clients =>
                clients.Select(c => c.ClientId).SequenceEqual(new long[] { 2, 4, 6, 7, 8, 9, 10 })
            )), Times.Once);


            duplicateClients.Should().HaveCount(3);
            duplicateClients.Select(c => c.ClientId).Should().BeEquivalentTo(existingClientIds);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotAddAnyClients_WhenAllClientsAreDuplicates()
        {
            var inputClients = new List<Client>
            {
                new Client { ClientId = 1, Username = "User1", SystemId = Guid.NewGuid() },
                new Client { ClientId = 2, Username = "User2", SystemId = Guid.NewGuid() }
            };

            var existingClientIds = new List<long> { 1, 2 };

            _clientRepositoryMock
                .Setup(repo => repo.GetExistingClientIdsAsync(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(existingClientIds);

            var duplicateClients = await _addMultipleClientsUseCase.ExecuteAsync(inputClients);

            _clientRepositoryMock.Verify(repo => repo.GetExistingClientIdsAsync(It.IsAny<IEnumerable<long>>()), Times.Once);
            _clientRepositoryMock.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Client>>()), Times.Never);

            duplicateClients.Should().HaveCount(2);
            duplicateClients.Select(c => c.ClientId).Should().BeEquivalentTo(existingClientIds);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldAddAllClients_WhenNoDuplicatesExist()
        {
            var inputClients = new List<Client>
            {
                new Client { ClientId = 1, Username = "User1", SystemId = Guid.NewGuid() },
                new Client { ClientId = 2, Username = "User2", SystemId = Guid.NewGuid() }
            };

            var existingClientIds = new List<long>();

            _clientRepositoryMock
                .Setup(repo => repo.GetExistingClientIdsAsync(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(existingClientIds);

            _clientRepositoryMock
                .Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Client>>()))
                .Returns(Task.CompletedTask);

            var duplicateClients = await _addMultipleClientsUseCase.ExecuteAsync(inputClients);

            _clientRepositoryMock.Verify(repo => repo.GetExistingClientIdsAsync(It.IsAny<IEnumerable<long>>()), Times.Once);
            _clientRepositoryMock.Verify(repo => repo.AddRangeAsync(It.Is<IEnumerable<Client>>(clients =>
                clients.Select(c => c.ClientId).SequenceEqual(new long[] { 1, 2 })
            )), Times.Once);

            duplicateClients.Should().BeEmpty();
        }
    }
}
