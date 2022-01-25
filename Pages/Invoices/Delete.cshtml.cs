#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(
               ApplicationDbContext context,
               IAuthorizationService authorizationService,
               UserManager<IdentityUser> userManager)
               : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // verify that an invoice id has been supplied
            if (id == null)
            {
                return NotFound();
            }

            // get invoice whose to be deleted by Id
            Invoice = await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceId == id);
            
            // if there's no invoice with the id supplied, return a 404 response
            if (Invoice == null)
            {
                return NotFound();
            }
            // check that the user has rights to delete an invoice
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, Invoice,
                                                     InvoiceOperations.Delete);
            // if user has no rights, forbid access
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            // verify that an invoice id has been supplied
            if (id == null)
            {
                return NotFound();
            }
            // get invoice by id
            Invoice = await Context.Invoice.FindAsync(id);

            // if there's no invoice with the id supplied, return a 404 response
            if (Invoice == null)
            {
                return NotFound();
            }
            // verify that the user has rights to delete an invoice from the system.
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                 User, Invoice,
                                                 InvoiceOperations.Delete);

            // if the user has no deletion rights, stop the user from executing the delete action
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            // delete the invoice from the system.
            Context.Invoice.Remove(Invoice);
            await Context.SaveChangesAsync();


            return RedirectToPage("./Index");
        }
    }
}
