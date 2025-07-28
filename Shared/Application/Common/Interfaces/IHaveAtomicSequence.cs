using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.Common.Interfaces
{
    public interface IHaveAtomicSequence
    {
        long Sequence { get; set; }
    }
}
