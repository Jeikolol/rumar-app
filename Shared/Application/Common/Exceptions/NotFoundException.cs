using Shared.Application.Common.Exceptions;
using System.Net;

namespace Shared.Application.Common.Exceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(string message)
        : base(message, null, HttpStatusCode.NotFound)
    {
    }
}