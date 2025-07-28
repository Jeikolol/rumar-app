using Shared.Entities;
using Shared.Shared.Enums;

namespace Shared.Shared.Dto
{
    public class AmortizationEntry
    {
        public int PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal TotalPayment => Principal + Interest;
        public decimal RemainingBalance { get; set; }
    }

    public static class AmortizationScheduleGenerator
    {
        public static List<AmortizationEntry> Generate(Loans loan)
        {
            var schedule = new List<AmortizationEntry>();
            var balance = loan.Amount;
            var periodRate = loan.InterestRate / 12;

            int totalPayments;
            TimeSpan interval;

            switch (loan.Frequency)
            {
                case PaymentFrequency.Weekly:
                    totalPayments = loan.TermInMonths * 4;
                    interval = TimeSpan.FromDays(7);
                    periodRate = loan.InterestRate / 52;
                    break;
                case PaymentFrequency.BiWeekly:
                    totalPayments = loan.TermInMonths * 2;
                    interval = TimeSpan.FromDays(14);
                    periodRate = loan.InterestRate / 26;
                    break;
                case PaymentFrequency.Quarterly:
                    totalPayments = loan.TermInMonths / 3;
                    interval = TimeSpan.FromDays(90);
                    periodRate = loan.InterestRate / 4;
                    break;
                case PaymentFrequency.Custom when loan.CustomDaysInterval.HasValue:
                    var totalDays = loan.TermInMonths * 30;
                    totalPayments = totalDays / loan.CustomDaysInterval.Value;
                    interval = TimeSpan.FromDays(loan.CustomDaysInterval.Value);
                    periodRate = loan.InterestRate / (365 / loan.CustomDaysInterval.Value);
                    break;
                default: // Monthly
                    totalPayments = loan.TermInMonths;
                    interval = TimeSpan.FromDays(30);
                    periodRate = loan.InterestRate / 12;
                    break;
            }

            var paymentAmount = CalculatePayment(loan.Amount, periodRate, totalPayments);

            for (int i = 1; i <= totalPayments; i++)
            {
                var interest = balance * periodRate;
                var principal = paymentAmount - interest;
                balance -= principal;

                schedule.Add(new AmortizationEntry
                {
                    PaymentNumber = i,
                    PaymentDate = loan.StartDate.AddDays(interval.TotalDays * i),
                    Principal = principal,
                    Interest = interest,
                    RemainingBalance = balance > 0 ? balance : 0
                });
            }

            return schedule;
        }

        private static decimal CalculatePayment(decimal loanAmount, decimal ratePerPeriod, int numberOfPayments)
        {
            if (ratePerPeriod == 0)
                return loanAmount / numberOfPayments;

            var denominator = (decimal)(1 - Math.Pow(1 + (double)ratePerPeriod, -numberOfPayments));
            return loanAmount * ratePerPeriod / denominator;
        }
    }
}
