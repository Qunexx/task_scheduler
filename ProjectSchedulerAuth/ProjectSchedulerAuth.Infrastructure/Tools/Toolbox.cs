using Microsoft.Extensions.Configuration;

namespace ProjectSchedulerAuth.Infrastructure.Tools
{
    /// <summary>
    /// Класс в котором находятся вспомогательные методы
    /// </summary>
    public class Toolbox
    {
        private readonly IConfiguration _config;

        public Toolbox(IConfiguration config)
        {
            _config = config;
        }

        public string GetSectionFromConfig(string sectionPath)
        {
            try
            {
                return _config.GetSection(sectionPath).Value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}