using Arise.DDD.Infrastructure.Data;
using Arise.DDD.Infrastructure.Extensions;
using FileService.File.Domain.AggregatesModel.FileInfoAggregate;
using FileService.File.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace File.Infrastructure
{
    public class FileContext : BaseContext
    {
        public DbSet<FileInfo> FileInfos { get; set; }

        public FileContext(DbContextOptions<FileContext> options, IMediator mediator) : base(options, mediator) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FileInfoEntityTypeConfiguration());
        }
    }
}
