using System.Net;

namespace web_programming_lab_2.Exceptions;

public abstract class BaseException : Exception
{
    public string Code { get; }
    public abstract HttpStatusCode StatusCode { get; }


    protected BaseException(string message, string code) : base(message)
    {
        Code = code;
    }
}