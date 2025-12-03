using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.Models;

namespace JapaneseTrainer.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}


