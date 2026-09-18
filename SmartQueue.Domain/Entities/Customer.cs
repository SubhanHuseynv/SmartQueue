using SmartQueue.Domain.Enums;

namespace SmartQueue.Domain.Entities;

public class Customer
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public QueueStatus Status { get; set; }
    public int Version { get; set; }
}
