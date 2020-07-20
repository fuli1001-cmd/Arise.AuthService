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
        App,
        AppThumbnail,
        AppVideo,
        AppOriginal,
        Chat,
        ChatThumbnail,
        ChatVideo
    }
}
