using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Entities;
using Shared.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EntityConfigurations
{
    public class PaymentConfig : BaseEntityConfiguration<Payment, Guid>
    {
        public PaymentConfig() : base(SchemaNames.Transaction)
        {
        }

        public override void Configure(EntityTypeBuilder<Payment> builder)
        {
            base.Configure(builder);

            builder
                .Property(x => x.PaymentDate)
                .IsRequired();

            builder
                .Property(x => x.Amount)
                .IsRequired();

            builder
                .Property(x => x.Note)
                .IsRequired()
                .HasDefaultValue(string.Empty);

            builder
                .Property(x => x.DueDate)
                .IsRequired();

            builder.Property(x => x.LateFee)
                .IsRequired()
                .HasDefaultValue(0m);

            builder
                .HasOne(p => p.Loan)
                .WithMany(l => l.Payments)
                .HasForeignKey(p => p.LoanId);
        }
    }
}
