namespace ProjectSchedulerAuth.Application.Exceptions
{
    public class LoginizationException : Exception
    {
        public LoginizationException(string message) : base(message) { }
    }
}