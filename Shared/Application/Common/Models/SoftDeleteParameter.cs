using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.Common.Models
{
    public class SoftDeleteParameter : UserCredentials
    {
        public Guid EntityId { get; set; }
    }

    public class UserCredentials
    {
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

}
