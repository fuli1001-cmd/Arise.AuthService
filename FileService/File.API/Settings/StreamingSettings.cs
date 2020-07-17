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

        public string App { get; set; }

        public string AppThumbnail { get; set; }

        public string AppVideo { get; set; }

        public string AppOriginal { get; set; }

        public string Chat { get; set; }

        public string ChatThumbnail { get; set; }

        public string ChatVideo { get; set; }
    }
}
