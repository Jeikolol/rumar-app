using Mapster;
using Shared.Application.Common.Exceptions;
using Shared.Entities;
using Shared.Infrastructure;
using Shared.Services.Interfaces;
using Shared.Shared.Dto;

namespace Shared.Services
{
    public class LoanService : ILoanService
    {
        private readonly IRumarRepository<Loans> _service;

        public LoanService(IRumarRepository<Loans> service)
        {
            _service = service;
        }

        public async Task<Loans> CreateAsync(CreateLoanRequest request, CancellationToken cancellation)
        {
           var entity = await _service.InsertAndSaveAsync(request.Adapt<Loans>(), cancellation);

            return entity;
        }

        public async Task<Loans> GetLoanByIdAsync(Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
            {
                throw new NotFoundException($"El préstamo de Id = [{id}] no fue encontrado");
            }

            return result;
        }
    }
}
