using Shared.Application.Common.Interfaces;
using Shared.Entities;
using Shared.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services.Interfaces
{
    public interface ILoanService : ITransientService
    {
        Task<Loans> CreateAsync(CreateLoanRequest request, CancellationToken cancellation);
        Task<Loans> GetLoanByIdAsync(Guid id);
    }
}
