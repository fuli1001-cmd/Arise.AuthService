using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arise.DDD.Domain.SeedWork
{
    public interface IRepository<T> where T : Entity, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        Task<T> GetByIdAsync(int id);
        Task<List<T>> ListAllAsync();
        Task<List<T>> ListAsync(ISpecification<T> spec);
        T Add(T entity);
        void Update(T entity);
    }
}
