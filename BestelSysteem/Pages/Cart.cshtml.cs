using System.Collections.Generic;
using System.Linq;
using BestelSysteem.Models;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult OnPostUpdateProduct(string productId, int quantity)
        {
            using (var session = _documentStore.OpenSession())
            {
                var orderedProduct = session.Query<OrderedProduct>()
                    .FirstOrDefault(op => op.ProductId == productId);

                if (orderedProduct != null)
                {
                    var product = session.Load<Product>(productId);
                    session.CountersFor(product.Id).Increment("Stock", orderedProduct.Quantity - quantity);

                    orderedProduct.Quantity = quantity;
                    orderedProduct.ProductTotal = orderedProduct.Quantity * product.Price;

                    if (orderedProduct.Quantity <= 0)
                    {
                        session.Delete(orderedProduct);
                    }

                    session.SaveChanges();
                }
            }

            return RedirectToPage("/Cart");
        }

        public IActionResult OnPostSaveOrder()
        {
            // Save the items in the cart to a document with timeseries data
            using (var session = _documentStore.OpenSession())
            {
                // Get the items from the cart
                var cartItems = session.Query<OrderedProduct>().ToList();
                
                // Create a new document to hold the cart items and timeseries data
                var orderDocument = new OrderDocument
                {
                    CartItems = cartItems,
                    Timestamp = DateTime.UtcNow
                };

                // Save the order document to the session
                session.Store(orderDocument);
                
                // Delete the cart items from the session
                foreach (var item in cartItems)
                {
                    session.Delete(item);
                }

                session.SaveChanges();
            }

            // Redirect to a confirmation page or any other desired page
            return RedirectToPage("/Confirmation");
        }

        public int GetProductStock(string productId)
        {
            var cartQuantity = ItemsInCart
                .Where(item => item.ProductId == productId)
                .Sum(item => item.Quantity);

            using (var session = _documentStore.OpenSession())
            {
                var product = session.Load<Product>(productId);
                var availableStock = session.CountersFor(product.Id).Get("Stock");
                var maxQuantity = (int)(availableStock + cartQuantity);

                return maxQuantity > 0 ? maxQuantity : 0;
            }
        }



    }
}
