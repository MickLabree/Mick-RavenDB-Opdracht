using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestelSysteem.Models
{
    public class Cart
    {
        public List<OrderedProduct> ItemsInCart { get; set; }
        public decimal Total { get; set; }

        public Cart()
        {
            ItemsInCart = new List<OrderedProduct>();
        }

    }
}
