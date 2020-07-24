using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.File.Domain.AggregatesModel.FileInfoAggregate
{
    public class FileInfo : Entity, IAggregateRoot
    {
        public string Name { get; private set; }

        public FileTag Tag { get; private set; }

        public DateTime CreatedTime { get; private set; }

        public FileInfo()
        {
            CreatedTime = DateTime.UtcNow;
        }

        public FileInfo(string name, FileTag tag) : this()
        {
            Name = name;
            Tag = tag;
        }
    }

    public enum FileTag
    {
        App, // app使用的中等大小图片
        AppThumbnail, // app使用的缩略图
        AppVideo, // app使用的视频
        AppOriginal, // app使用的原图
        Chat, // 聊天产生的中等大小图片
        ChatThumbnail, // 聊天产生的缩略图
        ChatVideo // 聊天视频
    }
}
