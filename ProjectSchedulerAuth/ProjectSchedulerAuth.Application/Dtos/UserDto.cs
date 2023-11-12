namespace ProjectSchedulerAuth.Application.Dtos
{
    /// <summary>
    /// Dto пользователя
    /// </summary>
    public class UserDto
    {
        public string SpecialName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}