using Shared.Models;
using Shared.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class Loans : BaseEntity
    {
        // Client Information
        public string CustomerFullName { get; set; } = string.Empty;
        public IdentificationType CustomerIdentificationType { get; set; }
        public string CustomerIdentification { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;


        // Loan Information
        public decimal Amount  { get; set; }
        public decimal InterestRate { get; set; }
        public int TermInMonths { get; set; }
        public CurrencyType Currency { get; set; } = CurrencyType.DOP;
        public LoanStatus Status { get; set; } = LoanStatus.Pending;
        public PaymentFrequency Frequency { get; set; } = PaymentFrequency.Monthly;
        public int? CustomDaysInterval { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
       

        // Payments
        public List<Payment> Payments { get; set; } = new();

        // Computed properties
        public DateTime DueDate => StartDate.AddMonths(TermInMonths);
        public decimal MonthlyInterest => Amount * (InterestRate / 100);
        public decimal MonthlyPayment => (Amount  + MonthlyInterest * TermInMonths) / TermInMonths;
        public decimal TotalRepayment => MonthlyPayment * TermInMonths;
        public decimal TotalPaid => Payments.Sum(p => p.Amount);
        public bool IsFullyPaid => Payments.Count >= TermInMonths;
        public decimal TotalLateFees => Payments.Sum(p => p.LateFee);
        public decimal RemainingBalance => TotalRepayment - TotalPaid + TotalLateFees;

        public void AddPayment(decimal amount, DateTime paymentDate)
        {
            var dueDate = StartDate.AddMonths(Payments.Count);
            var isLate = paymentDate > dueDate;
            var lateFee = isLate ? MonthlyPayment * 0.02m : 0m; // 2% late fee

            Payments.Add(new Payment
            {
                Amount = amount,
                PaymentDate = paymentDate,
                DueDate = dueDate,
                LateFee = lateFee
            });

            if (IsFullyPaid)
            {
                Status = LoanStatus.Closed;
            }
            else if (Payments.Count > 0)
            {
                Status = paymentDate > DueDate ? LoanStatus.Overdue : LoanStatus.Active;
            }
        }
    }
}
