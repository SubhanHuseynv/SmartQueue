using Microsoft.EntityFrameworkCore;
using SmartQueue.Application.DTOs.Customers;
using SmartQueue.Application.Exceptions;
using SmartQueue.Application.Interfaces.Repositories;
using SmartQueue.Application.Interfaces.Services;
using SmartQueue.Domain.Entities;
using SmartQueue.Domain.Enums;

namespace SmartQueue.Persistence.Implementations.Services;


internal class QueueService : IQueueService
{


    private readonly ICustomerRepository _repository;
    public QueueService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetAllCustomerDto>> GetAllQueueAsync()
    {
        IReadOnlyList<Customer> customers = await _repository.GetAllAsync(
            func: c => c.Status == QueueStatus.Waiting,
            order:c =>c.CreatedAt,
            isDesc: false);
        return customers.Select(c => new GetAllCustomerDto(
            c.Id,
            c.Name,
            c.CreatedAt
            )).ToList();
    }

    public async Task<GetByIdCustomerDto?> GetQueueByIdAsync(long id)
    {
        Customer? customer = await _repository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with id {id} not found");
        }

        if (customer.Status != QueueStatus.Waiting)
        {
            return new GetByIdCustomerDto
            (
                customer.Id,
                customer.Name,
                customer.Status.ToString(),
                null
            );
        }

        var position = await _repository.CountAsync(c => c.Status == QueueStatus.Waiting && customer.CreatedAt > c.CreatedAt) + 1;

        return new GetByIdCustomerDto(
            customer.Id,
            customer.Name,
            customer.Status.ToString(),
            position
            );
    }

    public async Task CreateQueueAsync(PostCustomerDto customerDto)
    {
        bool resultName = await _repository.AnyAsync(c => c.Name == customerDto.name);
        if (resultName) throw new BadRequestException($"Customer with name {customerDto.name} already exists");


        Customer customer = new()
        {
            Name = customerDto.name,
            Status = QueueStatus.Waiting
        };
        _repository.Create(customer);
        await _repository.SaveChangesAsync();
    }

    public async Task<CallNextCustomerDto?> CallNextCustomerAsync()
    {
        const int maxAttempts = 3;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            Customer? customer = await _repository.GetNextWaitingCustomerAsync();

            if (customer is null)
            {
                throw new NotFoundException("No customers in the queue.");
            }

            customer.Status = QueueStatus.Serving;
            customer.Version++; 

            try
            {
                await _repository.SaveChangesAsync();

                return new CallNextCustomerDto(
                    customer.Id,
                    customer.Name,
                    customer.CreatedAt,
                    customer.Status.ToString()
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                _repository.Detach(customer);
            }
        }

        throw new ConflictException("Could not call the next customer.");
    }


    public async Task DeleteQueueAsync(long id)
    {
        Customer? customer = await _repository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with id {id} not found");
        }
        _repository.Delete(customer);
        await _repository.SaveChangesAsync();
    }
}
