using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestelSysteem.Models
{
    public class OrderDocument
    {
        public List<OrderedProduct> CartItems { get; set; }
        public DateTime Timestamp { get; set; }
    }
}