using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BestelSysteem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;

namespace BestelSysteem.Pages
{
    public class OrderOverview : PageModel
    {
        private readonly ILogger<OrderOverview> _logger;
        private readonly IDocumentStore _documentStore;

        public List<OrderDocument> Orders { get; set; }

        public OrderOverview(ILogger<OrderOverview> logger, IDocumentStore documentStore)
        {
            _logger = logger;
            _documentStore = documentStore;
        }

        public void OnGet()
        {
            Orders = GetOrders();
        }

        private List<OrderDocument> GetOrders()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<OrderDocument>().ToList();
            }
        }

    }
}