using Shared.Shared.Enums;

namespace Shared.Shared.Dto
{
    public class CreateLoanRequest
    {
        public string CustomerFullName { get; set; } = string.Empty;
        public IdentificationType CustomerIdentificationType { get; set; }
        public string CustomerIdentification { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int TermInMonths { get; set; }
        public CurrencyType Currency { get; set; } = CurrencyType.DOP;
        public LoanStatus Status { get; set; } = LoanStatus.Pending;
        public PaymentFrequency Frequency { get; set; } = PaymentFrequency.Monthly;
        public int? CustomDaysInterval { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
    }

    public class UpdateLoanRequest : CreateLoanRequest
    {
        public Guid Id { get; set; }
    }
}
