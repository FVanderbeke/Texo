using Autofac;
using Texo.Domain.Model.Factory;
using Texo.Domain.Model.Repository;
using Texo.Domain.Model.Service;
using Texo.Infrastructure.InMemory.Service;
using Texo.Infrastructure.InMemory.Store;

namespace Texo.Infrastructure.InMemory.Module
{
    public class InMemoryModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryTransactionService>().As<ITransactionService>().SingleInstance();
            builder.RegisterType<InMemoryProjectStore>().As<IProjectFactory>().As<IProjectRepository>()
                .SingleInstance();
        }
    }
}