// WebAiko.Presentation/Controllers/ClientsController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAiko.Application.UseCases;
using WebAiko.Domain.Entities;

namespace WebAiko.Presentation.Controllers
{
    public class ClientsController : Controller
    {
        private readonly GetClientUseCase _getClientUseCase;
        private readonly AddClientUseCase _addClientUseCase;
        private readonly UpdateClientUseCase _updateClientUseCase;
        private readonly DeleteClientUseCase _deleteClientUseCase;
        private readonly AddMultipleClientsUseCase _addMultipleClientsUseCase;

        public ClientsController(
            GetClientUseCase getClientUseCase,
            AddClientUseCase addClientUseCase,
            UpdateClientUseCase updateClientUseCase,
            DeleteClientUseCase deleteClientUseCase,
            AddMultipleClientsUseCase addMultipleClientsUseCase)
        {
            _getClientUseCase = getClientUseCase;
            _addClientUseCase = addClientUseCase;
            _updateClientUseCase = updateClientUseCase;
            _deleteClientUseCase = deleteClientUseCase;
            _addMultipleClientsUseCase = addMultipleClientsUseCase;
        }
        public async Task<IActionResult> Index()
        {
            var clients = await _getClientUseCase.ExecuteAsync();
            return View(clients);
        }

        public async Task<IActionResult> Details(long id)
        {
            var client = await _getClientUseCase.ExecuteAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,Username,SystemId")] Client client)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _addClientUseCase.ExecuteAsync(client);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(client);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var client = await _getClientUseCase.ExecuteAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ClientId,Username,SystemId")] Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _updateClientUseCase.ExecuteAsync(client);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(client);
        }

        public async Task<IActionResult> Delete(long id)
        {
            var client = await _getClientUseCase.ExecuteAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                await _deleteClientUseCase.ExecuteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpPost]
        [Route("Clients/AddMultiple")]
        public async Task<IActionResult> AddMultiple([FromBody] IEnumerable<Client> clients)
        {
            if (clients == null || !clients.Any())
            {
                return BadRequest(" лиенты не предоставлены.");
            }

            try
            {
                var duplicateClients = await _addMultipleClientsUseCase.ExecuteAsync(clients);
                return Ok(duplicateClients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"¬нутренн€€ ошибка сервера: {ex.Message}");
            }
        }
    }
}
