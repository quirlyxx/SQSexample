using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SQSexample
{
    public class SupportRequest
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
