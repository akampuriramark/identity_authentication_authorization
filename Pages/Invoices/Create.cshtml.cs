#nullable disable
using Microsoft.AspNetCore.Mvc;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    public class CreateModel : DI_BasePageModel
    {

        // we use the constructor for the base page model to create the CreateModel instance
        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // get and set the id of the accountant that created the invoice
            Invoice.CreatorId = UserManager.GetUserId(User);

            // check whether the user has rights to create an invoice
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                            User, Invoice,
                                            InvoiceOperations.Create);
            // if the user is not allowed to create an invoice, forbid access the create page
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Invoice.Add(Invoice);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
