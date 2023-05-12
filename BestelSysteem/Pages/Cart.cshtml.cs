using System.Collections.Generic;
using System.Linq;
using BestelSysteem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;

namespace BestelSysteem.Pages
{
    public class Cart : PageModel
    {
        private readonly IDocumentStore _documentStore;
        public decimal Total { get; set; }

        public Cart(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public List<OrderedProduct> ItemsInCart { get; set; }

        public void OnGet()
        {
            // Retrieve the cart items from your data source
            // For example, if you're using RavenDB, you can use its session to query the database
            using (var session = _documentStore.OpenSession())
            {
                // Query the cart items and store them in the ItemsInCart property
                ItemsInCart = session.Query<OrderedProduct>().ToList();
            }
            Total = CalculateTotal();
        }

        private decimal CalculateTotal()
        {
            decimal total = 0;

            foreach (var product in ItemsInCart)
            {
                total += product.ProductTotal;
            }

            return total;
        }

    }
}
