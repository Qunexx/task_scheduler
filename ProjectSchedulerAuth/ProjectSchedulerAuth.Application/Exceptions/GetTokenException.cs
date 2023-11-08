namespace ProjectSchedulerAuth.Application.Exceptions
{
    public class GetTokenException : Exception
    {
        GetTokenException(string message) : base(message) { }
    }
}