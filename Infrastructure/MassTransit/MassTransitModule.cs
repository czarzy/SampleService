using Autofac;
using GreenPipes;
using MassTransit;
using System;

namespace Infrastructure.MassTransit
{
    public class MassTransitModule: Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.AddMassTransit(mt =>
            {
                mt.AddConsumer<MessageConsumer>();
                mt.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URI")), h =>
                    {
                        h.Username("RABBITMQ_USER");
                        h.Password("RABBITMQ_PASS");
                    });
                    cfg.ReceiveEndpoint(Environment.GetEnvironmentVariable("QUEUE_NAME"), ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(5, 100));
                        ep.ConfigureConsumer<MessageConsumer>(provider);
                    });
                }));
            });
        }
    }
}
