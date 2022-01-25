using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace IdentityApp.Authorization
{
    public class InvoiceAdminAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        protected override Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                    OperationAuthorizationRequirement requirement,
                                     Invoice invoice)
        {
            // if the user is null, return
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            if (context.User.IsInRole(Constants.InvoiceAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
