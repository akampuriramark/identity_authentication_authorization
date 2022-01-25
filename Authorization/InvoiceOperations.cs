using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace IdentityApp.Authorization
{
    public class InvoiceOperations
    {
        // add a create invoice authorization operation 
        public static OperationAuthorizationRequirement Create =
          new OperationAuthorizationRequirement { Name = Constants.CreateOperationName };
        // add a read invoice authorization operation 
        public static OperationAuthorizationRequirement Read =
          new OperationAuthorizationRequirement { Name = Constants.ReadOperationName };
        // add an update invoice authorization operation 
        public static OperationAuthorizationRequirement Update =
          new OperationAuthorizationRequirement { Name = Constants.UpdateOperationName };
        // add a delete invoice authorization operation 
        public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };
        // add an approve invoice authorization operation 
        public static OperationAuthorizationRequirement Approve =
          new OperationAuthorizationRequirement { Name = Constants.ApproveOperationName };
        // add a reject invoice authorization operation 
        public static OperationAuthorizationRequirement Reject =
          new OperationAuthorizationRequirement { Name = Constants.RejectOperationName };
    }
    public class Constants
    {
        // define constants for operation and role names.
        // for example CreateOperation name is named Create, ReadOperationName is Read, and so on
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public static readonly string InvoiceAdministratorsRole = "InvoiceAdministrators";
        public static readonly string InvoiceManagersRole = "InvoiceManagers";
    }
}
