using Autofac;
using Domain.Hangfire;

namespace Infrastructure.Hangfire
{
    public class HangfireModule: Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HangfireService>().As<IHangfireService>().InstancePerLifetimeScope();
        }

    }
}
