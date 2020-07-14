using Arise.DDD.Domain.SeedWork;
using Autofac;
using FileService.File.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FileService.File.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // register repositories
            builder.RegisterAssemblyTypes(typeof(FileInfoRepository).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRepository<>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
