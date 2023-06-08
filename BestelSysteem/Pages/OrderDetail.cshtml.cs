using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;

namespace BestelSysteem.Pages
{
    public class OrderDetail : PageModel
    {
        private readonly ILogger<OrderDetail> _logger;
        private readonly IDocumentStore _documentStore;
        

        public OrderDetail(ILogger<OrderDetail> logger, IDocumentStore documentStore)
        {
            _logger = logger;
            _documentStore = documentStore;
        }

        public void OnGet()
        {
        }
    }
}