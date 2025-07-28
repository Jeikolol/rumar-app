using Shared.Models;

namespace Shared.Entities
{
    public class Payment : BaseEntity
    {
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool IsLate => PaymentDate > DueDate;
        public DateTime DueDate { get; set; }
        public decimal LateFee { get; set; } = 0m;

        public Guid LoanId { get; set; }
        public Loans Loan { get; set; } = default!;
    }
}
