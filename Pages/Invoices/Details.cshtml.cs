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
    public class DetailsModel : DI_BasePageModel
    {

        public DetailsModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // if the invoice does not exist, return a 404 response
            if (id == null)
            {
                return NotFound();
            }

            Invoice = await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (Invoice == null)
            {
                return NotFound();
            }
            
            // verify whether the user is either a manager or administrator authorized to
            // view the invoice in any state.
            var isAuthorized = User.IsInRole(Constants.InvoiceManagersRole) ||
                   User.IsInRole(Constants.InvoiceAdministratorsRole);
            // get the id og the current user
            var currentUserId = UserManager.GetUserId(User);

            // if the user is not authorized but they are the creator of the invoice, they own it an therefore 
            // should be able to perform actions on it irrespective of the status not being approved.
            if (!isAuthorized
                && currentUserId != Invoice.CreatorId
                && Invoice.Status != InvoiceStatus.Approved)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, InvoiceStatus status)
        {
            // get the invoice by supplied id
            Invoice = await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceId == id);

            // if the invoice does not exist, return a 404 response
            if (Invoice == null)
            {
                return NotFound();
            }

            // if the invoice is not getting approved, then it is being rejected. 
            var invoiceOperation = (status == InvoiceStatus.Approved)
                                                       ? InvoiceOperations.Approve
                                                       : InvoiceOperations.Reject;

            // verify that the user has rights to approve or reject an invoice
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice,
                                        invoiceOperation);

            // if the user has no right to perform the action, block the user from performing the action
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            // else change the status of the invoice in the system. ie (approved or rejected)
            Invoice.Status = status;
            Context.Invoice.Update(Invoice);

            // persist the change to the database
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
