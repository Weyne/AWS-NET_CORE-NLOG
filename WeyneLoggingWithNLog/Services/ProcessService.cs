using System.Threading.Tasks;
using WeyneLoggingWithNLog.Interfaces;
using Microsoft.Extensions.Logging;


namespace WeyneLoggingWithNLog.Services
{
    public class ProcessService : IProcessService
    {
        private readonly ILogger<ProcessService> _logger;

        public ProcessService(ILogger<ProcessService> logger)
        {
            _logger = logger;
        }

        public async Task<string> Invoke(string input)
        {
            _logger.LogInformation("Hola mundo: {0}", input);
            _logger.LogError("Error: {0}", input);
            _logger.LogTrace("Trace: {0}", input);
            return input;
        }
    }
}
