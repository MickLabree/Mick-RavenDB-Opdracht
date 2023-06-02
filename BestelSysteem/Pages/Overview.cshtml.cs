using System.Collections.Generic;
using System.Linq;
using BestelSysteem.Models;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult OnPostAddProduct(string productId, int quantity)
        {
            using (var session = _documentStore.OpenSession())
            {
                var product = session.Load<Product>(productId);
                if (product != null)
                {
                    // Check if the product is already in the basket
                    var orderedProduct = session.Query<OrderedProduct>()
                        .FirstOrDefault(op => op.ProductId == productId);

                    if (orderedProduct != null)
                    {
                        // Product is already in the basket, update the quantity and product total
                        orderedProduct.Quantity += quantity;
                        orderedProduct.ProductTotal = orderedProduct.Quantity * product.Price;
                    }
                    else
                    {
                        var stock = session.CountersFor(productId).Get("Stock");
                        // Product is not in the basket, create a new ordered product entry
                        orderedProduct = new OrderedProduct
                        {
                            ProductId = productId,
                            ProductName = product.Name,
                            Quantity = quantity,
                            Price = product.Price,
                            ProductTotal = quantity * product.Price
                        };
                        session.Store(orderedProduct);
                    }

                    // Decrement the stock counter
                    var counter = session.CountersFor(product.Id).Get("Stock");
                    if (counter >= quantity)
                    {
                        session.CountersFor(product.Id).Increment("Stock", -quantity);
                        session.SaveChanges();
                    }
                    else
                    {
                        // Not enough stock available, handle the error
                        // For example, display an error message to the user
                        return RedirectToPage("/Error");
                    }

                    // Redirect to a confirmation page or perform any other action
                    return RedirectToPage("/Overview");
                }
            }

            // If the product is not found or any error occurred, redirect back to the overview page
            return RedirectToPage("Overview");
        }

    }
}
