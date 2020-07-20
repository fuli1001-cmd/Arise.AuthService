using FileService.File.Domain.AggregatesModel.FileInfoAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.File.API.Application.Commands.CreateFileInfo
{
    public class CreateFileInfoCommand : IRequest<bool>
    {
        public string Name { get; set; }

        public FileTag FileTag { get; set; }
    }
}
