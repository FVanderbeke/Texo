using Autofac;
using Microsoft.EntityFrameworkCore;
using Texo.Domain.Model.Factory;
using Texo.Domain.Model.Repository;
using Texo.Domain.Model.Service;
using Texo.Infrastructure.Db.Dao;
using Texo.Infrastructure.Db.Service;
using Texo.Infrastructure.Db.Internal;
using DbContext = Texo.Infrastructure.Db.Internal.DbContext;

namespace Texo.Infrastructure.Db.Module
{
    public class DbModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DbContext>().AsSelf().SingleInstance();
            builder.RegisterType<DbTransactionService>().As<ITransactionService>().SingleInstance();
            builder.RegisterType<DbProjectDao>().As<IProjectFactory>().As<IProjectRepository>().SingleInstance();
        }
    }
}