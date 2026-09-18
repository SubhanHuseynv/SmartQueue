using SmartQueue.Application.DTOs.Customers;

namespace SmartQueue.Application.Interfaces.Services;

public interface IQueueService
{
    Task<IReadOnlyList<GetAllCustomerDto>> GetAllQueueAsync();
    Task<GetByIdCustomerDto?> GetQueueByIdAsync(long id);
    Task CreateQueueAsync(PostCustomerDto customerDto);
    Task<CallNextCustomerDto?> CallNextCustomerAsync();
    Task DeleteQueueAsync(long id);
}
