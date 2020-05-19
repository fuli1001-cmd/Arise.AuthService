using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Arise.FileUploadService.Models
{
    public class UploadStatus
    {
        public List<UploadInfo> SuccessUploads { get; set; }
        public List<UploadInfo> FailedUploads { get; set; }
    }

    public class UploadInfo
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public int Index { get; set; }
    }
}
