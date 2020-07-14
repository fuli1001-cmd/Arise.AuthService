using FileService.File.Domain.AggregatesModel.FileInfoAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.File.Infrastructure.EntityConfigurations
{
    class FileInfoEntityTypeConfiguration : IEntityTypeConfiguration<FileInfo>
    {
        public void Configure(EntityTypeBuilder<FileInfo> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(f => f.Name).IsRequired();
        }
    }
}
