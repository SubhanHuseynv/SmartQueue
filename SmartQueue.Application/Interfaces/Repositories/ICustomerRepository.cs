using SmartQueue.Domain.Entities;
using System.Linq.Expressions;

namespace SmartQueue.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(
        Expression<Func<Customer, bool>>? func = null,
        Expression<Func<Customer, object>>? order = null,
        bool isDesc = false
        );
    Task<Customer?> GetByIdAsync(long id);
    void Create(Customer customer);
    void Update(Customer customer);
    void Detach(Customer customer);
    void Delete(Customer customer);
    Task SaveChangesAsync();
    Task<Customer?> GetNextWaitingCustomerAsync();
    Task<bool> AnyAsync(Expression<Func<Customer, bool>> func);
    Task<int> CountAsync(Expression<Func<Customer, bool>>? func = null);
}
