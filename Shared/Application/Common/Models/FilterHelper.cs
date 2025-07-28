using Shared.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.Common.Models
{
    public static class FilterHelper
    {
        public static string GetLogicOpt(LogicOperator opt)
        {
            switch (opt)
            {
                case LogicOperator.EqualTo: return "=";
                case LogicOperator.GreaterThan: return ">";
                case LogicOperator.LessThan: return "<";
                case LogicOperator.GreaterOrEqual: return ">=";
                case LogicOperator.LessOrEqual: return "<=";

                default:
                    throw new ArgumentException("Operador lógico no válido", nameof(opt));
            }
        }
    }
}
