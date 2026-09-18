using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQueue.Application.DTOs.Customers
{
    public record GetAllCustomerDto
    (
        long id,
        string name,
        DateTime createdAt
        );
}
