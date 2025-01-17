﻿using Arise.DDD.API.Response;
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
using Microsoft.AspNetCore.Hosting;
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
        [DisableRequestSizeLimit]
        //[RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        //[RequestSizeLimit(209715200)]
        public async Task<IActionResult> UploadPhysical([ModelBinder(BinderType = typeof(JsonModelBinder))] List<FileTag> tags)
        {
            Program.CleanAppMre.WaitOne();
            Program.CleanChatMre.WaitOne();
            Program.StreamingMre.Reset();

            try
            {
                _logger.LogInformation("File tags: {FileTags}", tags);

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
                        // 1. This check assumes that there's a file
                        // present without form data. If form data
                        // is present, this method immediately fails
                        // and returns the model error.
                        // 2. also check there is a tag correspond to the file
                        if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition) || (tags != null && i >= tags.Count))
                        {
                            if (tags != null && i >= tags.Count)
                                _logger.LogError($"Upload file {contentDisposition.FileName.Value} failed, no conresponding tag.");
                            else
                                _logger.LogError($"Upload file {contentDisposition.FileName.Value} failed, no file content disposition.");

                            // 记录上传失败的文件
                            failedUploads.Add(new UploadInfoDto { Name = contentDisposition.FileName.Value, Index = i });
                        }
                        else
                        {
                            _logger.LogInformation($"Uploading file {contentDisposition.FileName.Value} with tag {tags?[i].ToString() ?? "N/A"}.");

                            // Don't trust the file name sent by the client. To display
                            // the file name, HTML-encode the value.
                            var trustedFileNameForDisplay = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                            //var trustedFileNameForFileStorage = Path.GetRandomFileName();
                            var trustedFileNameForFileStorage = trustedFileNameForDisplay;

                            var fileInfo = tags == null ? null : await _fileInfoRepository.GetFileInfoAsync(trustedFileNameForFileStorage, tags[i]);

                            if (fileInfo == null)
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

                                    _logger.LogError($"Upload file {contentDisposition.FileName.Value} failed.");
                                }
                                else
                                {
                                    var folder = tags == null ? _streamingSettings.Value.StoredFilesPath : GetFolderByTag(tags[i]);
                                    //var folder = Path.Combine(_streamingSettings.Value.StoredFilesPath, tags?[i].ToString().ToLower() ?? string.Empty);

                                    _logger.LogInformation("folder: {folder}", folder);

                                    //Directory.CreateDirectory(folder);
                                    var filePath = Path.Combine(folder, trustedFileNameForFileStorage);

                                    _logger.LogInformation("Save file at path: {filePath}", filePath);

                                    // 文件写入文件系统
                                    using (var targetStream = System.IO.File.Create(filePath))
                                    {
                                        await targetStream.WriteAsync(streamedFileContent);
                                    }

                                    // 文件信息存入到数据库中
                                    var command = new CreateFileInfoCommand { Name = trustedFileNameForFileStorage, FileTag = tags[i] };
                                    await _mediator.Send(command);

                                    // 记录上传成功的文件
                                    successUploads.Add(new UploadInfoDto { Name = trustedFileNameForFileStorage, Index = i });

                                    // 向前兼容：拷贝文件
                                    if (tags == null)
                                    {
                                        //var distFolder = Path.Combine(_streamingSettings.Value.StoredFilesPath, FileTag.App.ToString().ToLower());
                                        var distFolder = GetFolderByTag(FileTag.App);
                                        System.IO.File.Copy(filePath, Path.Combine(distFolder, trustedFileNameForFileStorage), true);
                                        _logger.LogInformation($"Copied file {filePath} to {distFolder}");

                                        //distFolder = Path.Combine(_streamingSettings.Value.StoredFilesPath, FileTag.AppOriginal.ToString().ToLower());
                                        distFolder = GetFolderByTag(FileTag.AppOriginal);
                                        System.IO.File.Copy(filePath, Path.Combine(distFolder, trustedFileNameForFileStorage), true);
                                        _logger.LogInformation($"Copied file {filePath} to {distFolder}");

                                        //distFolder = Path.Combine(_streamingSettings.Value.StoredFilesPath, FileTag.AppThumbnail.ToString().ToLower());
                                        distFolder = GetFolderByTag(FileTag.AppThumbnail);
                                        System.IO.File.Copy(filePath, Path.Combine(distFolder, trustedFileNameForFileStorage), true);
                                        _logger.LogInformation($"Copied file {filePath} to {distFolder}");

                                        //distFolder = Path.Combine(_streamingSettings.Value.StoredFilesPath, FileTag.AppVideo.ToString().ToLower());
                                        distFolder = GetFolderByTag(FileTag.AppVideo);
                                        System.IO.File.Copy(filePath, Path.Combine(distFolder, trustedFileNameForFileStorage), true);
                                        _logger.LogInformation($"Copied file {filePath} to {distFolder}");

                                        //distFolder = Path.Combine(_streamingSettings.Value.StoredFilesPath, FileTag.Chat.ToString().ToLower());
                                        distFolder = GetFolderByTag(FileTag.Chat);
                                        System.IO.File.Copy(filePath, Path.Combine(distFolder, trustedFileNameForFileStorage), true);
                                        _logger.LogInformation($"Copied file {filePath} to {distFolder}");

                                        //distFolder = Path.Combine(_streamingSettings.Value.StoredFilesPath, FileTag.ChatThumbnail.ToString().ToLower());
                                        distFolder = GetFolderByTag(FileTag.ChatThumbnail);
                                        System.IO.File.Copy(filePath, Path.Combine(distFolder, trustedFileNameForFileStorage), true);
                                        _logger.LogInformation($"Copied file {filePath} to {distFolder}");

                                        //distFolder = Path.Combine(_streamingSettings.Value.StoredFilesPath, FileTag.ChatVideo.ToString().ToLower());
                                        distFolder = GetFolderByTag(FileTag.ChatVideo);
                                        System.IO.File.Copy(filePath, Path.Combine(distFolder, trustedFileNameForFileStorage), true);
                                        _logger.LogInformation($"Copied file {filePath} to {distFolder}");
                                    }
                                    else
                                    {
                                        var distFolder = _streamingSettings.Value.StoredFilesPath;
                                        System.IO.File.Copy(filePath, Path.Combine(distFolder, trustedFileNameForFileStorage), true);
                                        _logger.LogInformation($"Copied file {filePath} to {distFolder}");
                                    }

                                    _logger.LogInformation(
                                        "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                                        "'{TargetFilePath}' as {TrustedFileNameForFileStorage}",
                                        trustedFileNameForDisplay, folder,
                                        trustedFileNameForFileStorage);
                                }
                            }
                            else
                            {
                                // 文件已存在，直接记录为上传成功
                                successUploads.Add(new UploadInfoDto { Name = trustedFileNameForFileStorage, Index = i });
                                _logger.LogInformation($"File {trustedFileNameForFileStorage} already exists in database, skip.");
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
            catch
            {
                throw;
            }
            finally
            {
                Program.StreamingMre.Set();
            }
        }

        private string GetFolderByTag(FileTag tag)
        {
            return _streamingSettings.Value.GetType().GetProperty(tag.ToString()).GetValue(_streamingSettings.Value, null).ToString();
        }
    }
}
