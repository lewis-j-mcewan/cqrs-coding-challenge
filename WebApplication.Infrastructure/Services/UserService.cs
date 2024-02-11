using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly InMemoryContext _dbContext;

        public UserService(InMemoryContext dbContext)
        {
            _dbContext = dbContext;

            // this is a hack to seed data into the in memory database. Do not use this in production.
            _dbContext.Database.EnsureCreated();
        }

        /// <inheritdoc />
        public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            User? user = await _dbContext.Users.Where(user => user.Id == id)
                                         .Include(x => x.ContactDetail)
                                         .FirstOrDefaultAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> FindAsync(string? givenNames, string? lastName, CancellationToken cancellationToken = default)
        {
            IEnumerable<User> users = await _dbContext.Users
                .AsNoTracking()
                .Where(user => user.GivenNames.Equals(givenNames) || user.LastName.Equals(lastName))
                .Include(user => user.ContactDetail)
                .ToListAsync(cancellationToken);

            return users;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetPaginatedAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            IEnumerable<User> usersOnPage =
                await _dbContext.Users.Skip((page - 1) * count).Take(count).ToListAsync(cancellationToken);

            return usersOnPage;
        }

        /// <inheritdoc />
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.Users.AddAsync(user, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            
                return user;
            }
            catch
            {
                return default;
            }
        }

        /// <inheritdoc />
        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            User? retrievedUser = await _dbContext.Users
                                        .Where(x => x.Id == user.Id)
                                        .Include(x => x.ContactDetail)
                                        .FirstOrDefaultAsync(cancellationToken);

            if (retrievedUser == null) return default;
            
            retrievedUser.Id = user.Id;
            retrievedUser.GivenNames = user.GivenNames;
            retrievedUser.LastName = user.LastName;
            retrievedUser.ContactDetail = user.ContactDetail;
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return user;
        }

        /// <inheritdoc />
        public async Task<User?> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {

            User? user = await _dbContext.Users
                                .Where(user => user.Id == id)
                                .Include(x => x.ContactDetail)
                                .FirstOrDefaultAsync(cancellationToken);
            
            if (user == null) return default;
            
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        
            return user;
        }

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            int totalUserCount = await _dbContext.Users.CountAsync(cancellationToken);

            return totalUserCount;
        }
    }
}
