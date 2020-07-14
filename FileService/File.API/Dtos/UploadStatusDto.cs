using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.File.API.Dtos
{
    public class UploadStatusDto
    {
        public List<UploadInfoDto> SuccessUploads { get; set; }
        public List<UploadInfoDto> FailedUploads { get; set; }
    }

    public class UploadInfoDto
    {
        public string Name { get; set; }
        public int Index { get; set; }
    }
}
