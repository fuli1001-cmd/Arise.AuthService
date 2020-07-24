using FileService.File.API.Settings;
using FileService.File.Domain.AggregatesModel.FileInfoAggregate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileService.File.API.Application.Services
{
    public class CleanService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly StreamingSettings _streamingSettings;
        private readonly AppCleanSettings _appCleanSettings;
        private readonly ChatCleanSettings _chatCleanSettings;
        private readonly ILogger<CleanService> _logger;

        private Timer _timer;

        // can not inject IRepository<T> directly, because IRepository<T> is scoped, 
        // while BackgroundService is singleton, have to use IServiceScopeFactory to generate a scope
        public CleanService(
            IOptionsSnapshot<StreamingSettings> streamingOptions,
            IOptionsSnapshot<AppCleanSettings> appCleanOptions,
            IOptionsSnapshot<ChatCleanSettings> chatCleanOptions,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<CleanService> logger)
        {
            _streamingSettings = streamingOptions?.Value ?? throw new ArgumentNullException(nameof(streamingOptions));
            _appCleanSettings = appCleanOptions?.Value ?? throw new ArgumentNullException(nameof(appCleanOptions));
            _chatCleanSettings = chatCleanOptions?.Value ?? throw new ArgumentNullException(nameof(chatCleanOptions));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 清理程序内文件
            ScheduleTask(_appCleanSettings.StartHour, _appCleanSettings.StartMinute, _appCleanSettings.IntervalHours, async () =>
            {
                Program.StreamingMre.WaitOne();
                Program.CleanAppMre.Reset();

                _logger.LogInformation("start clean app files");

                try { await CleanAppAsync(); }
                catch { throw; }
                finally 
                { 
                    Program.CleanAppMre.Set();
                    _logger.LogInformation("end clean app files");
                }
            });

            // 清理聊天文件
            ScheduleTask(_chatCleanSettings.StartHour, _chatCleanSettings.StartMinute, _chatCleanSettings.IntervalHours, async () =>
            {
                Program.StreamingMre.WaitOne();
                Program.CleanChatMre.Reset();

                _logger.LogInformation("start clean chat files");

                try { await CleanChatAsync(); }
                catch { throw; }
                finally
                {
                    Program.CleanChatMre.Set();
                    _logger.LogInformation("end clean chat files");
                }
            });
        }

        // 清理程序内文件
        private async Task CleanAppAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IFileInfoRepository>();

                await CleanFilesWithTagAsync(FileTag.App, repository);
                await CleanFilesWithTagAsync(FileTag.AppOriginal, repository);
                await CleanFilesWithTagAsync(FileTag.AppThumbnail, repository);
                await CleanFilesWithTagAsync(FileTag.AppVideo, repository);

                await repository.UnitOfWork.SaveEntitiesAsync();
            }
        }

        // 清理聊天文件
        private async Task CleanChatAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IFileInfoRepository>();

                // 类别传任意一种聊天类别即可删除所有聊天文件
                await CleanFilesWithTagAsync(FileTag.Chat, repository);

                await repository.UnitOfWork.SaveEntitiesAsync();
            }
        }

        /// <summary>
        /// 按类别清理文件
        /// </summary>
        /// <param name="tag">文件类别</param>
        /// <param name="repository"></param>
        /// <returns></returns>
        private async Task CleanFilesWithTagAsync(FileTag tag, IFileInfoRepository repository)
        {
            List<Domain.AggregatesModel.FileInfoAggregate.FileInfo> notUsedFileInfos;

            switch(tag)
            {
                case FileTag.App:
                case FileTag.AppOriginal:
                case FileTag.AppThumbnail:
                    notUsedFileInfos = await repository.GetNotUsedAppImagesAsync(tag);
                    break;

                case FileTag.AppVideo:
                    notUsedFileInfos = await repository.GetNotUsedAppVideosAsync(tag);
                    break;

                case FileTag.Chat:
                case FileTag.ChatThumbnail:
                case FileTag.ChatVideo:
                    // 删除早于距当前时间超过任务执行间隔的聊天文件
                    var oldestTime = DateTime.UtcNow.AddHours(-_chatCleanSettings.IntervalHours);
                    notUsedFileInfos = await repository.GetAllChatFileInfosAsync(oldestTime);
                    break;

                default:
                    return;
            }

            notUsedFileInfos.ForEach(fi =>
            {
                var delResult = false;

                // 由于聊天文件是统一删除，因此如果类别是聊天中的任意一种，则删除所有聊天文件
                // app内文件按传入类别删除
                if (tag == FileTag.Chat || tag == FileTag.ChatThumbnail || tag == FileTag.ChatVideo)
                    delResult = DeleteFile(fi.Name, FileTag.Chat) && DeleteFile(fi.Name, FileTag.ChatThumbnail) && DeleteFile(fi.Name, FileTag.ChatVideo);
                else 
                    delResult = DeleteFile(fi.Name, tag);

                // 如果删除文件成功则删除数据库数据
                if (delResult)
                    repository.Remove(fi);
            });
        }

        /// <summary>
        /// 从磁盘删除文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="tag">文件类别</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        private bool DeleteFile(string name, FileTag tag)
        {
            var folder = GetFolderByTag(tag);
            var filePath = Path.Combine(folder, name);

            try
            {
                System.IO.File.Delete(filePath);
                _logger.LogError("deleted {File}", filePath);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError("delete {File} failed: {@DeleteFileException}", filePath, ex);
                return false;
            }
        }

        // 根据类别获取文件存储路径
        private string GetFolderByTag(FileTag tag)
        {
            return _streamingSettings.GetType().GetProperty(tag.ToString()).GetValue(_streamingSettings, null).ToString();
        }

        /// <summary>
        /// 在指定的UTC时间以指定间隔重复执行一个任务
        /// </summary>
        /// <param name="hour">任务开始执行的时间点（小时）</param>
        /// <param name="min">任务开始执行的时间点（分钟）</param>
        /// <param name="intervalHours">重复执行间隔</param>
        /// <param name="task">要执行的任务</param>
        private void ScheduleTask(int hour, int min, double intervalHours, Action task)
        {
            DateTime now = DateTime.UtcNow;
            DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, hour, min, 0, 0);

            if (now > firstRun)
                firstRun = firstRun.AddDays(1);

            TimeSpan timeToGo = firstRun - now;

            _timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours(intervalHours));
        }
    }
}
