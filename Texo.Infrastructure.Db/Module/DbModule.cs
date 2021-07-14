using Autofac;
using Microsoft.EntityFrameworkCore;
using Texo.Domain.Model.Factory;
using Texo.Domain.Model.Repository;
using Texo.Domain.Model.Service;
using Texo.Infrastructure.Db.Dao;
using Texo.Infrastructure.Db.Service;
using DbContext = Texo.Infrastructure.Db.Service.DbContext;

namespace Texo.Infrastructure.Db.Module
{
    public class DbModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DbContextFactory>().OnActivating(e => e.Instance.Activate()).AsSelf().SingleInstance();
            builder.RegisterType<DbTransactionService>().As<ITransactionService>().AsSelf().SingleInstance();
            builder.RegisterType<DbProjectDao>().As<IProjectFactory>().As<IProjectRepository>().SingleInstance();
        }
    }
}