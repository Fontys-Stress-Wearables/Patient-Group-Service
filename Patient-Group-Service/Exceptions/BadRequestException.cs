using System.Net;

namespace Patient_Group_Service.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }
}
