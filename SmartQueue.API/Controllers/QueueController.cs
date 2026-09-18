using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartQueue.Application.DTOs.Customers;
using SmartQueue.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace SmartQueue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly IQueueService _queueService;

        public QueueController(IQueueService queueService)
        {
            _queueService = queueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWaitingCustomers()
        {
            return Ok(await _queueService.GetAllQueueAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQueueById(long id)
        {
            var queue = await _queueService.GetQueueByIdAsync(id);
            if (queue == null)
            {
                return NotFound();
            }
            return Ok(queue);
        }
        [HttpPost]
        public async Task<IActionResult> CreateQueue([FromBody] PostCustomerDto customerDto)
        {
            await _queueService.CreateQueueAsync(customerDto);
            return Created();
        }

        [HttpPost("next")]
        public async Task<IActionResult> NextCustomer()
        {
            var nextCustomer = await _queueService.CallNextCustomerAsync();

            return Ok(nextCustomer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQueue(long id)
        {
            await _queueService.DeleteQueueAsync(id);
            return NoContent();
        }
    }
}
