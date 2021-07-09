using Autofac;
using Texo.Domain.Model.Service;

namespace Texo.Domain.Module
{
    public class DomainModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GlobalTransactionService>().SingleInstance();
            builder.RegisterType<TransactionManager>().SingleInstance();
            builder.RegisterType<ProjectManager>().SingleInstance();
        }
    }
}