using Arise.DDD.Infrastructure;
using File.Infrastructure;
using FileService.File.Domain.AggregatesModel.FileInfoAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.File.Infrastructure.Repositories
{
    public class FileInfoRepository : EfRepository<FileInfo, FileContext>, IFileInfoRepository
    {
        public FileInfoRepository(FileContext context) : base(context)
        {

        }

        public async Task<FileInfo> GetFileInfoByNameAsync(string name)
        {
            return await _context.FileInfos.FirstOrDefaultAsync(f => f.Name.ToLower() == name.ToLower());
        }
    }
}
