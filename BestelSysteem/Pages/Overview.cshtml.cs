using System.Collections.Generic;
using System.Linq;
using BestelSysteem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace MyApp.Namespace
{
    public class OverviewModel : PageModel
    {
        private readonly IDocumentStore _documentStore;

        public OverviewModel(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public List<Product> Products { get; set; }

        public void OnGet()
        {
            using (var session = _documentStore.OpenSession())
            {
                var products = session.Query<Product>().ToList();
                foreach (var product in products)
                {
                    var counter = session.CountersFor(product.Id).Get("Stock");
                    product.Stock = (int)(counter ?? 0); // if the counter doesn't exist, default to 0
                }
                Products = products;
            }
        }



    }
}
