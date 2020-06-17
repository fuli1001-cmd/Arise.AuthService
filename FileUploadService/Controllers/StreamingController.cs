using Arise.DDD.API.Response;
using Arise.DDD.Domain.Exceptions;
using Arise.FileUploadService.Filters;
using Arise.FileUploadService.Models;
using Arise.FileUploadService.Settings;
using Arise.FileUploadService.Utilities;
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

namespace Arise.FileUploadService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class StreamingController : ControllerBase
    {
        private readonly ILogger<StreamingController> _logger;
        private readonly IOptionsSnapshot<StreamingSettings> _streamingSettings;

        private readonly string[] _permittedExtensions = { ".txt", ".png", ".jpeg", ".jpg", ".gif", ".zip" };

        // Get the default form options so that we can use them to set the default 
        // limits for request body data.
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public StreamingController(ILogger<StreamingController> logger, IOptionsSnapshot<StreamingSettings> streamingSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _streamingSettings = streamingSettings ?? throw new ArgumentNullException(nameof(streamingSettings));
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadPhysical()
        {
            var i = 0;
            var successUploads = new List<UploadInfo>();
            var failedUploads = new List<UploadInfo>();

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                //ModelState.AddModelError("File",
                //    $"The request couldn't be processed (Error 1).");
                // Log error

                //return BadRequest(ModelState);
                throw new ClientException("上传失败", new List<string> { "Not multipart content type." });
            }

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
                        //ModelState.AddModelError("File",
                        //    $"The request couldn't be processed (Error 2).");
                        // Log error

                        //return BadRequest(ModelState);
                        //return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper(2, new string[] { "The request couldn't be processed." }));
                        failedUploads.Add(new UploadInfo { Name = contentDisposition.FileName.Value, Index = i });
                    }
                    else
                    {
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                        //var trustedFileNameForFileStorage = Path.GetRandomFileName();
                        var trustedFileNameForFileStorage = trustedFileNameForDisplay;

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
                            //return BadRequest(ModelState);
                            failedUploads.Add(new UploadInfo { Name = contentDisposition.FileName.Value, Index = i });
                            ModelState.Clear();
                        }
                        else
                        {
                            using (var targetStream = System.IO.File.Create(
                                Path.Combine(_streamingSettings.Value.StoredFilesPath, trustedFileNameForFileStorage)))
                            {
                                await targetStream.WriteAsync(streamedFileContent);

                                successUploads.Add(new UploadInfo { Name = trustedFileNameForFileStorage, Index = i });

                                _logger.LogInformation(
                                    "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                                    "'{TargetFilePath}' as {TrustedFileNameForFileStorage}",
                                    trustedFileNameForDisplay, _streamingSettings.Value.StoredFilesPath,
                                    trustedFileNameForFileStorage);
                            }
                        }
                    }
                }

                i++;
                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            //return Created(nameof(StreamingController), null);
            var uploadStatus = new UploadStatus { SuccessUploads = successUploads, FailedUploads = failedUploads };
            if (failedUploads.Count > 0)
                return StatusCode((int)HttpStatusCode.BadRequest, 
                    ResponseWrapper.CreateErrorResponseWrapper(DDD.API.Response.StatusCode.ClientError, "Upload Failed.", null, uploadStatus));
            else
                return Ok(ResponseWrapper.CreateOkResponseWrapper(uploadStatus));
        }
    }
}
