using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestelSysteem.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
    }

}