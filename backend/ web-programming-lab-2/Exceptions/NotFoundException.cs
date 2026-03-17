using System.Net;

namespace web_programming_lab_2.Exceptions;

public class NotFoundException<T> : BaseException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public NotFoundException(int id)
        : base($"{typeof(T).Name} with ID {id} not found", $"{typeof(T).Name.ToUpper()}_NOT_FOUND")
    {
    }
    
    public NotFoundException(string message)
        : base(message, "NOT_FOUND")
    { }
}