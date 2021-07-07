using Autofac;
using Texo.Domain.Api.Factory;
using Texo.Domain.Api.Provider;
using Texo.Domain.Api.Repository;
using Texo.Infrastructure.InMemory.Service;
using Texo.Infrastructure.InMemory.Store;

namespace Texo.Infrastructure.InMemory.Module
{
    public class InMemoryModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryTransactionProvider>().As<ITransactionProvider>().SingleInstance();
            builder.RegisterType<InMemoryProjectStore>().As<IProjectFactory>().As<IProjectRepository>()
                .SingleInstance();
        }
    }
}