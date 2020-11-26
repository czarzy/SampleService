using Domain.MassTransit;
using System.Threading.Tasks;

namespace Domain.Hangfire
{
    public interface IHangfireService
    {
        Task EnqueueAsync(Message message);
    }
}
