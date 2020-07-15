using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileService.File.Domain.AggregatesModel.FileInfoAggregate
{
    public interface IFileInfoRepository : IRepository<FileInfo>
    {
        Task<FileInfo> GetFileInfoAsync(string name, FileTag tag);
    }
}
