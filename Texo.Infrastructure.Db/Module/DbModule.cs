using Autofac;
using Microsoft.EntityFrameworkCore;
using Texo.Domain.Api.Factory;
using Texo.Domain.Api.Provider;
using Texo.Domain.Api.Repository;
using Texo.Infrastructure.Db.Service;
using Texo.Infrastructure.Db.Internal;
using Texo.Infrastructure.Db.Store;
using DbContext = Texo.Infrastructure.Db.Internal.DbContext;

namespace Texo.Infrastructure.Db.Module
{
    public class DbModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DbContext>().AsSelf().SingleInstance();
            builder.RegisterType<DbTransactionProvider>().As<ITransactionProvider>().SingleInstance();
            builder.RegisterType<DbProjectStore>().As<IProjectFactory>().As<IProjectRepository>().SingleInstance();
        }
    }
}