using Domain.Hangfire;
using Domain.MassTransit;
using Serilog;
using System.Threading.Tasks;

namespace Infrastructure.Hangfire
{
    public class HangfireService: IHangfireService
    {
        private readonly ILogger _logger;
        public HangfireService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task EnqueueAsync(Message message)
        {
            //Hangfire queue logic here
        }
    }
}
