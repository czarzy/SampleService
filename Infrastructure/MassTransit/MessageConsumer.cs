using Domain.MassTransit;
using MassTransit;
using Serilog;
using System.Threading.Tasks;

namespace Infrastructure.MassTransit
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly ILogger _logger;

        public MessageConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            //put consumer logic here
        }
    }
}
