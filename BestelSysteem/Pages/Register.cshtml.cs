using System.ComponentModel.DataAnnotations;
using BestelSysteem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BestelSysteem
{
    public class RegisterModel : PageModel
    {
        private readonly IDocumentStore _documentStore;

        public RegisterModel(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        [BindProperty]
        public RegistrationRequest RegistrationRequest { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var session = _documentStore.OpenSession())
            {
                // Check if user already exists with the same email address
                if (session.Query<User>().Any(u => u.Email == RegistrationRequest.Email))
                {
                    ModelState.AddModelError(string.Empty, "A user with this email address already exists.");
                    return Page();
                }

                // Create new user
                var user = new User
                {
                    FirstName = RegistrationRequest.FirstName,
                    LastName = RegistrationRequest.LastName,
                    Email = RegistrationRequest.Email,
                    Password = RegistrationRequest.Password
                };

                // Save user to RavenDB
                session.Store(user);
                session.SaveChanges();
            }

            return RedirectToPage("/Index");
        }
    }
}
