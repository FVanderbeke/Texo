using Autofac;
using Texo.Domain.Api.Provider;
using Texo.Domain.Api.Service;
using Texo.Domain.Default.SemVer;

namespace Texo.Domain.Module
{
    public class DomainModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GlobalTransactionProvider>().SingleInstance();
            builder.RegisterType<TransactionManager>().SingleInstance();
            builder.RegisterType<ProjectManager>().SingleInstance();
            builder.RegisterType<Versions>().SingleInstance();
            builder.RegisterType<SemVerVersionProvider>().As<IVersionProvider>().SingleInstance();
        }
    }
}