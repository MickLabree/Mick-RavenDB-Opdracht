using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;

namespace BestelSysteem.Pages;

public class IndexModel : PageModel
{
    private readonly IDocumentStore _documentStore;

    public IndexModel(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        using (var session = _documentStore.OpenAsyncSession())
        {
            var dog = await session.LoadAsync<Dog>("Dog");
            if (dog == null)
            {
                return Content("Dog not found!");
            }

            return Content($"Dog name: {dog.Name}, breed: {dog.Breed}");
        }
    }
}


