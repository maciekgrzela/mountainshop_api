using Microsoft.AspNetCore.Http;
using Persistence.Context;

namespace Application
{
    public abstract class BaseService
    {
        protected IUnitOfWork UnitOfWork { get; private set; }
        protected DataContext DataContext { get; private set; }

        public BaseService(IUnitOfWork unitOfWork, DataContext dataContext)
        {
            UnitOfWork = unitOfWork;
            DataContext = dataContext;
        }
    }
}