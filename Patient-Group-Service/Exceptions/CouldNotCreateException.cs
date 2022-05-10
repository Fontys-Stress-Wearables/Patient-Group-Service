using System.Net;

namespace Patient_Group_Service.Exceptions;

public class CouldNotCreateException : AppException
{
    public CouldNotCreateException(string message) : base(HttpStatusCode.InternalServerError, message)
    {
    }
}
