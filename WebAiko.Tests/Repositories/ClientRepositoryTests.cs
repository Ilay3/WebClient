using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using WebAiko.Domain.Entities;
using WebAiko.Infrastructure.Data;
using WebAiko.Infrastructure.Repositories;

namespace WebAiko.Tests.Repositories
{
    public class ClientRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ClientRepository _clientRepository;

        public ClientRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _clientRepository = new ClientRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddClient()
        {
            var client = new Client
            {
                ClientId = 1,
                Username = "TestUser",
                SystemId = Guid.NewGuid()
            };

            await _clientRepository.AddAsync(client);
            var retrievedClient = await _clientRepository.GetByIdAsync(client.ClientId);

            retrievedClient.Should().NotBeNull();
            retrievedClient.ClientId.Should().Be(client.ClientId);
            retrievedClient.Username.Should().Be(client.Username);
            retrievedClient.SystemId.Should().Be(client.SystemId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllClients()
        {
            var clients = new List<Client>
            {
                new Client { ClientId = 1, Username = "User1", SystemId = Guid.NewGuid() },
                new Client { ClientId = 2, Username = "User2", SystemId = Guid.NewGuid() }
            };

            await _context.Clients.AddRangeAsync(clients);
            await _context.SaveChangesAsync();

            var result = await _clientRepository.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(clients);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenClientExists()
        {
            var client = new Client { ClientId = 3, Username = "User3", SystemId = Guid.NewGuid() };
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            var exists = await _clientRepository.ExistsAsync(client.ClientId);

            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenClientDoesNotExist()
        {
            var clientId = 4L;

            var exists = await _clientRepository.ExistsAsync(clientId);

            exists.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateClient()
        {
            var client = new Client { ClientId = 5, Username = "User5", SystemId = Guid.NewGuid() };
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            client.Username = "UpdatedUser5";
            await _clientRepository.UpdateAsync(client);
            var updatedClient = await _clientRepository.GetByIdAsync(client.ClientId);

            updatedClient.Username.Should().Be("UpdatedUser5");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveClient()
        {
            var client = new Client { ClientId = 6, Username = "User6", SystemId = Guid.NewGuid() };
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            await _clientRepository.DeleteAsync(client.ClientId);
            var deletedClient = await _clientRepository.GetByIdAsync(client.ClientId);

            deletedClient.Should().BeNull();
        }

        [Fact]
        public async Task GetExistingClientIdsAsync_ShouldReturnExistingIds()
        {
            var clients = new List<Client>
            {
                new Client { ClientId = 7, Username = "User7", SystemId = Guid.NewGuid() },
                new Client { ClientId = 8, Username = "User8", SystemId = Guid.NewGuid() },
                new Client { ClientId = 9, Username = "User9", SystemId = Guid.NewGuid() }
            };

            await _context.Clients.AddRangeAsync(clients);
            await _context.SaveChangesAsync();

            var clientIdsToCheck = new List<long> { 7, 10, 9 };

            var existingIds = await _clientRepository.GetExistingClientIdsAsync(clientIdsToCheck);

            existingIds.Should().BeEquivalentTo(new List<long> { 7, 9 });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
