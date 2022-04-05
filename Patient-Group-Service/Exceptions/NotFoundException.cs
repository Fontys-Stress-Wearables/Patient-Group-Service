using System.Net;

namespace Patient_Group_Service.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
}
