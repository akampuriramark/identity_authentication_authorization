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
    public class EditModel : DI_BasePageModel
    {
        public EditModel(
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
            // check that an id was passed
            if (id == null)
            {
                return NotFound();
            }

            // get the invoice whose id was passed
            Invoice = await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceId == id);

            // Return not found if there's no invoice of id supplied
            if (Invoice == null)
            {
                return NotFound();
            }

            // check if the user is allowed to edit the invoice
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                      User, Invoice,
                                                      InvoiceOperations.Update);
            //forbid acces if the user has no rights to edit an invoice
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // first we validaste the invoice model to make usre it is valid
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch invoice from DB to get its creator.
            var invoice = await Context
                .Invoice.AsNoTracking()
                .FirstOrDefaultAsync(m => m.InvoiceId == id);

            // if there's no invoice with that invoice id, we return a 404 response
            if (invoice == null)
            {
                return NotFound();
            }
            // verify if the user has rights to edit the invoice
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, invoice,
                                                     InvoiceOperations.Update);
            // if user has no rights to edit, forbid access to the edit invoice action
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Invoice.CreatorId = invoice.CreatorId;

            Context.Attach(Invoice).State = EntityState.Modified;

            if (Invoice.Status == InvoiceStatus.Approved)
            {
                // If the invoice is updated after approval, 
                // and the user who edited it has no rights to approve,
                // we set the status back to submitted so the update can be
                // checked and approved.
                var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                        Invoice,
                                        InvoiceOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    Invoice.Status = InvoiceStatus.Submitted;
                }
            }

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // if we are trying to update an invoice that does not exist,
                // return a 404 response else, throw an exception
                if (!InvoiceExists(Invoice.InvoiceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        // method to check whether an invoice exists by invoice id
        private bool InvoiceExists(int id)
        {
            return Context.Invoice.Any(e => e.InvoiceId == id);
        }
    }
}
