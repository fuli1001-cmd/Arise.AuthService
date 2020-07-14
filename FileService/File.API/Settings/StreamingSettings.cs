using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.File.API.Settings
{
    public class StreamingSettings
    {
        public long FileSizeLimit { get; set; }
        public string StoredFilesPath { get; set; }
    }
}
