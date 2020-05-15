using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Arise.FileUploadService.Models
{
    public class UploadStatus
    {
        public List<SuccessUploadInfo> SuccessUploads { get; set; }
        public List<FailedUploadInfo> FailedUploads { get; set; }
    }

    public class SuccessUploadInfo
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
    }

    public class FailedUploadInfo
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
