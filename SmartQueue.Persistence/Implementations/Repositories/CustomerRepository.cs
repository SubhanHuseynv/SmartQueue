using Microsoft.EntityFrameworkCore;
using SmartQueue.Application.Interfaces.Repositories;
using SmartQueue.Domain.Entities;
using SmartQueue.Domain.Enums;
using SmartQueue.Persistence.Context;
using System.Linq.Expressions;

namespace SmartQueue.Persistence.Implementations.Repositories;

internal class CustomerRepository : ICustomerRepository
{
    private readonly AppDBContext _context;
    public CustomerRepository(AppDBContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(
        Expression<Func<Customer,bool>>? func = null,
        Expression<Func<Customer, object>>? order = null,
         bool isDesc = false
        )
    {
        IQueryable<Customer> query = _context.Customers.AsNoTracking();
        if (func is not null)
        { 
        query = query.Where(func);
        }
        if(order is not null)
        {
            query = isDesc ? query.OrderByDescending(order) : query.OrderBy(order);
        }

        return await query.ToListAsync();
    } 

    public async Task<Customer?> GetByIdAsync(long id)
    {
        Customer? data = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c =>c.Id == id);
        return data;
    }

    public async Task<Customer?> GetNextWaitingCustomerAsync()
    {
        // DİQQƏT: AsNoTracking YOXDUR — tracked olmalıdır!
        return await _context.Customers
            .Where(c => c.Status == QueueStatus.Waiting)
            .OrderBy(c => c.CreatedAt)
            .ThenBy(c => c.Id)
            .FirstOrDefaultAsync();
    }

    public void Detach(Customer customer)
    {
        _context.Entry(customer).State = EntityState.Detached;
    }

    public void Create(Customer customer)
    {
        _context.Customers.Add(customer);
    }

    public void Update(Customer customer)
    {
        _context.Customers.Update(customer);
    }

    public void Delete(Customer customer)
    {
        _context.Customers.Remove(customer);
    }

    public async Task<bool> AnyAsync(Expression<Func<Customer,bool>> func)
    {
        return await _context.Customers.AnyAsync(func);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public  async Task<int> CountAsync(Expression<Func<Customer,bool>>? func = null)
    {
        IQueryable<Customer> query = _context.Customers;
        if (func is not null)
        {
            query = query.Where(func);
        }
        return await query.CountAsync();
    }
}
