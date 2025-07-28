using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Shared.Application.Common.Exceptions;
using Shared.Application.Common.Models;
using Shared.Services.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace Api.Auth
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User?.GetUserId() is { } userId &&
                await _userService.HasPermissionAsync(userId, requirement.Permission))
            {
                context.Succeed(requirement);
            }

            string? uri = _httpContextAccessor?.HttpContext?.Request.GetDisplayUrl();
            string requestBody = string.Empty;
            if (!string.IsNullOrEmpty(context.User?.GetUserId())
                && uri != null && Regex.IsMatch(uri, "delete.*with.*credentials", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))
            {
                HttpRequest? req = _httpContextAccessor?.HttpContext?.Request;

                _httpContextAccessor?.HttpContext?.Request.EnableBuffering();

                if (_httpContextAccessor.HttpContext.Request.Body.CanSeek)
                {
                    _httpContextAccessor.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                    using StreamReader reader = new(_httpContextAccessor.HttpContext.Request.Body, Encoding.UTF8, false, 1024, true);
                    requestBody = await reader.ReadToEndAsync();
                }

                req.Body.Position = 0;

                var softDeleteParameter = JsonConvert.DeserializeObject<SoftDeleteParameter>(requestBody);

                if (softDeleteParameter != null)
                {
                    bool userResult = await _userService.HasPermissionWithCredentialsAsync(softDeleteParameter, requirement.Permission, new CancellationToken());

                    if (!userResult)
                    {
                        throw new NotFoundException("El Usuario no tiene el permiso requerido.");
                    }
                }
            }

            context.Succeed(requirement);

        }
    }
}
