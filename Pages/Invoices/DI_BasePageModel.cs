using IdentityApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Invoices
{
    // the base class inherits all properties of a Razor page.
    // this is because all other Razor pages will inherit from it instead of the PageModel directly.
    public class DI_BasePageModel : PageModel
    {
        protected ApplicationDbContext Context { get; }
        protected IAuthorizationService AuthorizationService { get; }
        protected UserManager<IdentityUser> UserManager { get; }

        // we inject the services that all other pages will use to perform actions on the invoice model
        // UserManaager will be used to get the logged in user id to verify that they are the invoice owner.
        // IAuthorizationService will be used to verify that a user is allowed to perform an action.
        public DI_BasePageModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base()
        {
            Context = context;
            UserManager = userManager;
            AuthorizationService = authorizationService;
        }
    }
}
