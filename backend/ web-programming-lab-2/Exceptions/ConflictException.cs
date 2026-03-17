using System.Net;

namespace web_programming_lab_2.Exceptions;

public class ConflictException : BaseException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

    public ConflictException(string message, string code = "CONFLICT_ERROR")
        : base(message, code)
    {
    }
}