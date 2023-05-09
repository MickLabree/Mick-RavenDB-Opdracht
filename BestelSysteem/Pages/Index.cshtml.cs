using BestelSysteem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;

namespace BestelSysteem.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IDocumentStore _documentStore;

    public IndexModel(ILogger<IndexModel> logger, IDocumentStore documentStore)
    {
        _logger = logger;
        _documentStore = documentStore;
    }

    public void OnGet()
    {

    }

    public IActionResult OnPostLogin(string username, string password)
    {
        using (var session = _documentStore.OpenSession())
        {
            var user = session.Query<User>()
                .FirstOrDefault(u => u.FirstName == username && u.Password == password);

            if (user != null)
            {
                // gebruiker is gevonden en gevalideerd, stuur door naar Overview-pagina
                return RedirectToPage("/Overview");
            }
            else
            {
                // Pass the error message to the frontend
                ViewData["ErrorMessage"] = "Incorrect username or password.";

                return Page();
            }
        }
    }
}