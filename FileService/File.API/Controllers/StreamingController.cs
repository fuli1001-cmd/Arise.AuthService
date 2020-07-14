using Arise.DDD.API.Response;
using Arise.DDD.Domain.Exceptions;
using BrunoZell.ModelBinding;
using FileService.File.API.Application.Commands.CreateFileInfo;
using FileService.File.API.Dtos;
using FileService.File.API.Filters;
using FileService.File.API.Settings;
using FileService.File.API.Utilities;
using FileService.File.Domain.AggregatesModel.FileInfoAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FileService.File.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class StreamingController : ControllerBase
    {
        private readonly ILogger<StreamingController> _logger;
        private readonly IOptionsSnapshot<StreamingSettings> _streamingSettings;
        private readonly IMediator _mediator;
        private readonly IFileInfoRepository _fileInfoRepository;

        private readonly string[] _permittedExtensions = { ".txt", ".png", ".jpeg", ".jpg", ".gif", ".zip" };

        // Get the default form options so that we can use them to set the default 
        // limits for request body data.
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public StreamingController(
            IFileInfoRepository fileInfoRepository,
            IMediator mediator, 
            ILogger<StreamingController> logger, 
            IOptionsSnapshot<StreamingSettings> streamingSettings)
        {
            _fileInfoRepository = fileInfoRepository ?? throw new ArgumentNullException(nameof(fileInfoRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _streamingSettings = streamingSettings ?? throw new ArgumentNullException(nameof(streamingSettings));
        }

        // use multipart content with json data:
        // https://stackoverflow.com/questions/41367602/upload-files-and-json-in-asp-net-core-web-api
        // https://github.com/BrunoZell/JsonModelBinder
        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadPhysical([ModelBinder(BinderType = typeof(JsonModelBinder))] List<FileInfoDto> fileInfos)
        {
            var i = 0;
            var successUploads = new List<UploadInfoDto>();
            var failedUploads = new List<UploadInfoDto>();

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                throw new ClientException("上传失败", new List<string> { "Not multipart content type." });

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    // This check assumes that there's a file
                    // present without form data. If form data
                    // is present, this method immediately fails
                    // and returns the model error.
                    if (!MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        // 记录上传失败的文件
                        failedUploads.Add(new UploadInfoDto { Name = contentDisposition.FileName.Value, Index = i });
                    }
                    else
                    {
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                        //var trustedFileNameForFileStorage = Path.GetRandomFileName();
                        var trustedFileNameForFileStorage = trustedFileNameForDisplay;

                        var fileInfo = fileInfos.FirstOrDefault(fi => fi.Name == trustedFileNameForFileStorage);

                        if ((await _fileInfoRepository.GetFileInfoByNameAsync(trustedFileNameForFileStorage)) == null && fileInfo != null)
                        {
                            // 文件不存在，保存文件并记录到数据库
                            // **WARNING!**
                            // In the following example, the file is saved without
                            // scanning the file's contents. In most production
                            // scenarios, an anti-virus/anti-malware scanner API
                            // is used on the file before making the file available
                            // for download or for use by other systems. 
                            // For more information, see the topic that accompanies 
                            // this sample.

                            var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                                section, contentDisposition, ModelState,
                                _permittedExtensions, _streamingSettings.Value.FileSizeLimit);

                            if (!ModelState.IsValid)
                            {
                                // 记录上传失败的文件
                                failedUploads.Add(new UploadInfoDto { Name = trustedFileNameForFileStorage, Index = i });
                                ModelState.Clear();
                            }
                            else
                            {
                                var folder = Path.Combine(_streamingSettings.Value.StoredFilesPath, fileInfo.Tag.ToString().ToLower());
                                Directory.CreateDirectory(folder);

                                using (var targetStream = System.IO.File.Create(Path.Combine(folder, trustedFileNameForFileStorage)))
                                {
                                    // 文件写入文件系统
                                    await targetStream.WriteAsync(streamedFileContent);

                                    // 文件信息存入到数据库中
                                    var command = new CreateFileInfoCommand { Name = trustedFileNameForFileStorage };
                                    await _mediator.Send(command);

                                    // 记录上传成功的文件
                                    successUploads.Add(new UploadInfoDto { Name = trustedFileNameForFileStorage, Index = i });

                                    _logger.LogInformation(
                                        "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                                        "'{TargetFilePath}' as {TrustedFileNameForFileStorage}",
                                        trustedFileNameForDisplay, _streamingSettings.Value.StoredFilesPath,
                                        trustedFileNameForFileStorage);
                                }
                            }
                        }
                        else
                        {
                            if (fileInfo == null)
                                failedUploads.Add(new UploadInfoDto { Name = trustedFileNameForFileStorage, Index = i });
                            else
                                // 文件已存在，直接记录为上传成功
                                successUploads.Add(new UploadInfoDto { Name = trustedFileNameForFileStorage, Index = i });
                        }
                    }
                }

                i++;
                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            //return Created(nameof(StreamingController), null);
            var uploadStatus = new UploadStatusDto { SuccessUploads = successUploads, FailedUploads = failedUploads };
            if (failedUploads.Count > 0)
                return StatusCode((int)HttpStatusCode.BadRequest, 
                    ResponseWrapper.CreateErrorResponseWrapper(Arise.DDD.API.Response.StatusCode.ClientError, "Upload Failed.", null, uploadStatus));
            else
                return Ok(ResponseWrapper.CreateOkResponseWrapper(uploadStatus));
        }
    }
}
