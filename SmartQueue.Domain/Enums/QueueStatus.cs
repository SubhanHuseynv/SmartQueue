using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQueue.Domain.Enums
{
    public enum QueueStatus
    {
        Waiting=1,
        Serving=2,
        Completed=3
    }
}
