using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQueue.Application.DTOs.Customers
{
    public record GetByIdCustomerDto
   (
       long id,
       string name,
       string status,
       int? position
       );
}
