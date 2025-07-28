using Shared.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.Identity.Users
{
    public class UserListFilter : PaginationFilter
    {
        public bool? IsActive { get; set; }
    }
}
