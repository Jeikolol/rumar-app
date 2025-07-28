using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.Entities;
using Shared.Infrastructure;
using Shared.Models;
using Shared.Shared.Dto;

namespace Shared.Application.Common.Validation.Transaction
{
    public class CreateLoansValidation : CustomValidator<CreateLoanRequest>
    {
        public CreateLoansValidation(IRumarRepository<Loans> repo, IStringLocalizer<CreateLoanRequest> t) 
        {
            RuleFor(x => x.CustomerIdentification)
                .NotEmpty()
                .MustAsync(async (ide, ct) => await repo.Table().AnyAsync(e => e.CustomerIdentification == ide && !e.IsFullyPaid, cancellationToken: ct))
                .WithMessage((_, ide) => t["Ya existe un préstamo sin terminar con esta cédula"]);
        }
    }

    public class UpdateLoansValidation : CustomValidator<UpdateLoanRequest>
    {
        public UpdateLoansValidation(IRumarRepository<Loans> repo, IStringLocalizer<UpdateLoanRequest> t)
        {
            RuleFor(x => x.CustomerIdentification)
                .NotEmpty()
                .CustomAsync(async (ide, cont, ct) =>
                {
                    try
                    {
                        var exists = await repo.Table().AnyAsync(e => e.Id != cont.InstanceToValidate.Id && e.CustomerIdentification == ide && !e.IsFullyPaid, cancellationToken: ct);

                        if (exists)
                        {
                            var fail = new FluentValidation.Results.ValidationFailure(
                                cont.DisplayName,
                                t["Ya existe un préstamo sin terminar con esta cédula"]);

                            cont.AddFailure(fail);
                        }
                    }
                    catch (Exception ex)
                    {

                        var fail = new FluentValidation.Results.ValidationFailure(
                            cont.DisplayName,
                            ex.ToString());

                        cont.AddFailure(fail);
                    }
                });
        }
    }
}
