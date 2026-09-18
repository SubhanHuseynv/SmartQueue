using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartQueue.Application.Interfaces.Repositories;
using SmartQueue.Application.Interfaces.Services;
using SmartQueue.Persistence.Context;
using SmartQueue.Persistence.Implementations.Repositories;
using SmartQueue.Persistence.Implementations.Services;

namespace SmartQueue.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<AppDBContext>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("Default"))
        );



        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IQueueService, QueueService>();
    }
}
