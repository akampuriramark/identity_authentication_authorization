using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace IdentityApp.Authorization
{
    public class InvoiceManagerAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Invoice invoice)
        {
            if (context.User == null || invoice == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for approval/reject, return.
            // Managers can only approve or reject the Invoice
            if (requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            // if user is a manager, the requirement succeeds
            if (context.User.IsInRole(Constants.InvoiceManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
