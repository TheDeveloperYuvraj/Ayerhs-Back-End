using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Interfaces.AccountManagement;
using Ayerhs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ayerhs.Application.Repositories.AccountManagement
{
    public class AccountRepository(ApplicationDbContext context) : IAccountRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Clients> AddClientAsync(Clients client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<Clients?> GetClientByEmailAsync(string email)
        {
            return await _context.Clients.SingleOrDefaultAsync(c => c.ClientEmail == email);
        }

        public async Task<Clients?> GetClientByUsernameAsync(string username)
        {
            return await _context.Clients.SingleOrDefaultAsync(c => c.ClientUsername == username);
        }
    }
}
