using DataAccess.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Entities;
using Shared.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class LoanConfig : BaseEntityConfiguration<Loans, Guid>
    {
        public LoanConfig() : base(SchemaNames.Transaction)
        {
        }

        public override void Configure(EntityTypeBuilder<Loans> builder)
        {
            base.Configure(builder);

            builder
                .Property(l => l.CustomerFullName)
                .IsRequired();

            builder
                .Property(l => l.CustomerIdentificationType)
                .IsRequired();

            builder
                .Property(l => l.CustomerIdentification)
                .IsRequired();

            builder
                .Property(l => l.PhoneNumber)
                .IsRequired();

            builder
                .Property(l => l.Address)
                .IsRequired();


            builder
               .Property(l => l.Amount)
               .IsRequired();

            builder
               .Property(l => l.InterestRate)
               .IsRequired();

            builder
               .Property(l => l.TermInMonths)
               .IsRequired();

            builder
               .Property(l => l.Currency)
               .IsRequired();

            builder
               .Property(l => l.Status)
               .IsRequired();

            builder
               .Property(l => l.Frequency)
               .IsRequired();

            builder
               .Property(l => l.CustomDaysInterval);

            builder
               .Property(l => l.StartDate)
               .IsRequired();

            builder
                .HasMany(l => l.Payments)
                .WithOne(p => p.Loan)
                .HasForeignKey(p => p.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Ignore(l => l.DueDate)
                .Ignore(l => l.MonthlyInterest)
                .Ignore(l => l.MonthlyPayment)
                .Ignore(l => l.TotalRepayment)
                .Ignore(l => l.TotalPaid)
                .Ignore(l => l.IsFullyPaid)
                .Ignore(l => l.TotalLateFees)
                .Ignore(l => l.RemainingBalance);
        }
    }
}
