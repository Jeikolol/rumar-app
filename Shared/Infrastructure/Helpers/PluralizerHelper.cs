using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Helpers
{
    public class PluralizerHelper
    {
        public static string Pluralize<TEntity>()
        {
            return typeof(TEntity).Name.Pluralize(inputIsKnownToBeSingular: false);
        }
    }
}
