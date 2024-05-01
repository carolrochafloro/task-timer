using System.Linq.Expressions;

namespace task_timer.Repositories;

public interface IRepository<T>
{
    public Task<IEnumerable<T>> GetAllAsync();

    // The parameter is a lambda expression that defines the condition
    // the returned object must satisfy. For example: (p => p.Id == idT).
    public T? Get(Expression<Func<T, bool>> predicate);
    public Task<T> CreateAsync(T entity);
    public T UpdateAsync(T entity);
    public T DeleteAsync(T entity);

}
