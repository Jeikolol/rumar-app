using Api.Data;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using DataAccess.Context;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Shared.Application.Common.Caching;
using Shared.Application.Common.Exceptions;
using Shared.Application.Common.Models;
using Shared.Application.Identity.Users;
using Shared.Application.Specification;
using Shared.Entities.Identity;
using Shared.Infrastructure.Auth;
using Shared.Services.Interfaces;
using Shared.Shared.Constants;

namespace Api.Identity.Users
{
    internal partial class UserService : IUserService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IStringLocalizer _t;
        private readonly SecuritySettings _securitySettings;
        private readonly IEventPublisher _events;
        private readonly ICacheService _cache;

        public UserService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext db,
            IStringLocalizer<UserService> localizer,
            IOptions<SecuritySettings> securitySettings,
            ICacheService cache)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _t = localizer;
            _securitySettings = securitySettings.Value;
            _cache = cache;
        }

        public async Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken)
        {
            var spec = new EntitiesByPaginationFilterSpec<ApplicationUser>(filter);

            var users = await _userManager.Users
                .WithSpecification(spec)
                .ProjectToType<UserDetailsDto>()
                .ToListAsync(cancellationToken);
            int count = await _userManager.Users
                .CountAsync(cancellationToken);

            return new PaginationResponse<UserDetailsDto>(users, count, filter.PageNumber, filter.PageSize);
        }

        public async Task<bool> ExistsWithNameAsync(string name)
        {
            return await _userManager.FindByNameAsync(name) is not null;
        }

        public async Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null)
        {
            return await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user && user.Id != exceptId;
        }

        public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null)
        {
            return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is ApplicationUser user && user.Id != exceptId;
        }

        public async Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken) =>
            (await _userManager.Users
                    .AsNoTracking()
                    .ToListAsync(cancellationToken))
                .Adapt<List<UserDetailsDto>>();

        public Task<int> GetCountAsync(CancellationToken cancellationToken) =>
            _userManager.Users.AsNoTracking().CountAsync(cancellationToken);

        public async Task<UserDetailsDto> GetAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync(cancellationToken);

            _ = user ?? throw new NotFoundException(_t["User Not Found."]);

            return user.Adapt<UserDetailsDto>();
        }

        public async Task<UserDetailsDto> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync(cancellationToken);

            _ = user ?? throw new NotFoundException(_t["User Not Found."]);

            return user.Adapt<UserDetailsDto>();
        }

        public async Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

            _ = user ?? throw new NotFoundException(_t["User Not Found."]);

            bool isAdmin = await _userManager.IsInRoleAsync(user, RumarRoles.Admin);
            if (isAdmin)
            {
                throw new ConflictException(_t["Administrators Profile's Status cannot be toggled"]);
            }

            user.IsActive = request.ActivateUser;

            await _userManager.UpdateAsync(user);

            await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
        }
    }
}