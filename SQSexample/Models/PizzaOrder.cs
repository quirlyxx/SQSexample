using Amazon.Runtime.EventStreams;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQSexample.Models
{
    public class PizzaOrder
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string PizzaType { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
