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

        public async Task<FileInfo> GetFileInfoAsync(string name, FileTag tag)
        {
            return await _context.FileInfos.FirstOrDefaultAsync(f => f.Name.ToLower() == name.ToLower() && f.Tag == tag);
        }

        public Task<List<FileInfo>> GetNotUsedAppImagesAsync(FileTag tag)
        {
            return _context.FileInfos.FromSqlRaw("EXECUTE dbo.GetNotUsedAppImages {0}", tag).ToListAsync();
        }

        public Task<List<FileInfo>> GetNotUsedAppVideosAsync(FileTag tag)
        {
            return _context.FileInfos.FromSqlRaw("EXECUTE dbo.GetNotUsedAppVideos {0}", tag).ToListAsync();
        }

        public async Task<List<FileInfo>> GetAllChatFileInfosAsync(DateTime oldestTime)
        {
            return await _context.FileInfos.Where(f => f.CreatedTime < oldestTime && (f.Tag == FileTag.Chat || f.Tag == FileTag.ChatThumbnail || f.Tag == FileTag.ChatVideo)).ToListAsync();
        }
    }
}
