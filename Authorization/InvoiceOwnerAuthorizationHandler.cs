using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Authorization
{
    public class InvoiceOwnerAuthorizationHandler : 
                 AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        UserManager<IdentityUser> _userManager;

        // we inject the current user trying to perform an action on the invoice. 
        // We shall use this user id to confirm the logged in user is the creator of the invoice.
        public InvoiceOwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // we overirde the method to make a decision on whether to allow authorization
        // to perform an operation based on whether the accountant is the creator of the invoice.
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       Invoice invoice)
        {
            // if the user or the invopice is null, return
            if (context.User == null || invoice == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            // if the user is the creator, then the requirements for the operation are met.
            // they can perform any operation on their invoice.
            if (invoice.CreatorId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
