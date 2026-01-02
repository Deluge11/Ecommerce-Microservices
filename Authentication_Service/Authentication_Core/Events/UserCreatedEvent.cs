using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Core.Events
{
    public record UserCreatedEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public int UserId { get; init; }
        public string UserName { get; init; }
        public string UserEmail { get; init; }
    }
}
