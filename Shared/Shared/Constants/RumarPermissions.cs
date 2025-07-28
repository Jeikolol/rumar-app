using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Shared.Constants
{
    public static class RumarResource
    {
        public const string Dashboard = nameof(Dashboard);
        public const string Hangfire = nameof(Hangfire);
        public const string Users = nameof(Users);
        public const string UserRoles = nameof(UserRoles);
        public const string Roles = nameof(Roles);
        public const string RoleClaims = nameof(RoleClaims);
    }

    public class RumarPermissions
    {
        private static RumarPermission[] _all
        {
            get
            {
                List<RumarPermission> permissions = new();

                var permissionClasses = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => typeof(IRumarModulePermissions).IsAssignableFrom(t) && t.IsClass)
                    .ToList();

                foreach (var permissionClass in permissionClasses)
                {
                    var field = permissionClass.GetField("All", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    var data = field?.GetValue(null) as RumarPermission[] ?? Array.Empty<RumarPermission>();

                    permissions.AddRange(data);
                }

                return permissions.ToArray();
            }
        }

        public static IReadOnlyList<RumarPermission> All { get; } = new ReadOnlyCollection<RumarPermission>(_all);
        public static IReadOnlyList<RumarPermission> Root { get; } = new ReadOnlyCollection<RumarPermission>(_all.Where(p => p.IsRoot).ToArray());
        public static IReadOnlyList<RumarPermission> Admin { get; } = new ReadOnlyCollection<RumarPermission>(_all.Where(p => !p.IsRoot).ToArray());
        public static IReadOnlyList<RumarPermission> Basic { get; } = new ReadOnlyCollection<RumarPermission>(_all.Where(p => p.IsBasic).ToArray());
        public static IReadOnlyList<RumarPermission> ByModule(string moduleName)
        {
            return new ReadOnlyCollection<RumarPermission>(_all.Where(p => p.ModuleName == moduleName).ToArray());
        }
    }

    public record RumarPermission(string Description, string Action, string Controller, string? ModuleName, string? SubmoduleName = null, bool IsBasic = false, bool IsRoot = false)
    {
        public string Name => NameFor(Action, Controller, ModuleName, SubmoduleName);
        public static string NameFor(string action, string resource, string? module = null, string? submodule = null) =>
            $"Permissions.{(string.IsNullOrEmpty(module) ? string.Empty : module + ".")}{(string.IsNullOrEmpty(submodule) ? string.Empty : submodule + ".")}{resource}.{action}";
    }

}
