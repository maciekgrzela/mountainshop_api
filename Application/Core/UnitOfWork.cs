using System.Threading.Tasks;
using Persistence.Context;

namespace Application
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }
        
        public async Task CommitTransactionsAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}