using FileService.File.Domain.AggregatesModel.FileInfoAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.File.API.Dtos
{
    public class FileInfoDto
    {
        public string Name { get; set; }

        public FileTag Tag { get; set; }
    }
}
