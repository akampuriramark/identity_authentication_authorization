#nullable disable
using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IList<Invoice> Invoice { get;set; }

        public async Task OnGetAsync()
        {
            var invoices = from c in Context.Invoice
                           select c;
            // verify that the user is either a manager or administrator
            var isAuthorized = User.IsInRole(Constants.InvoiceManagersRole) ||
                               User.IsInRole(Constants.InvoiceAdministratorsRole);
            // get the current user's id.
            var currentUserId = UserManager.GetUserId(User);

            // Only approved invoices are shown UNLESS you're authorized to see them
            // or you are the owner.
            if (!isAuthorized)
            {
                invoices = invoices.Where(c => c.Status == InvoiceStatus.Approved
                                            || c.CreatorId == currentUserId);
            }
            // return the invoices that the user has rights to view ( theirs and approved invoices)
            Invoice = await invoices.ToListAsync();
        }
    }
}
