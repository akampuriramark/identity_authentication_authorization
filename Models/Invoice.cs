namespace IdentityApp.Models
{
    public class Invoice
    {
        // unique identifier of the invoice in the system 
        public int InvoiceId { get; set; }
        // amount being invoiced
        public double InvoiceAmount { get; set; }
        // month for which the invoice was made.
        public string InvoiceMonth { get; set; }
        // name of the employee whose invoice this is
        public string InvoiceOwner { get; set; }
        // user id of the creator of the invoice
        public string CreatorId { get; set; }
        // status of the invoice in the system
        public InvoiceStatus Status { get; set; }

    }

    // an invoice can be in any of the 3 states: submitted approved and rejected.
    public enum InvoiceStatus
    {
        Submitted,
        Approved,
        Rejected
    }
}
