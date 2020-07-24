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

        /// <summary>
        /// 胺类别获取未使用的程序内图片
        /// </summary>
        /// <param name="tag">类别</param>
        /// <returns></returns>
        Task<List<FileInfo>> GetNotUsedAppImagesAsync(FileTag tag);

        /// <summary>
        /// 按类别获取未使用的程序内视频
        /// </summary>
        /// <param name="tag">类别</param>
        /// <returns></returns>
        Task<List<FileInfo>> GetNotUsedAppVideosAsync(FileTag tag);

        /// <summary>
        /// 获取聊天相关的文件数据
        /// </summary>
        /// <returns></returns>
        Task<List<FileInfo>> GetAllChatFileInfosAsync();
    }
}
