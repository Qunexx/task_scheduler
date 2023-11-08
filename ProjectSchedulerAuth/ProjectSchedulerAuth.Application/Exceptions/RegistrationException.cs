namespace ProjectSchedulerAuth.Application.Exceptions
{
    public class RegistrationException : Exception
    {
        public RegistrationException(string message): base(message) { }
    }
}