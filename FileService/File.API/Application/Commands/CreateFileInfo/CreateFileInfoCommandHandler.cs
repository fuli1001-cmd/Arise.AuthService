using FileService.File.Domain.AggregatesModel.FileInfoAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileService.File.API.Application.Commands.CreateFileInfo
{
    public class CreateFileInfoCommandHandler : IRequestHandler<CreateFileInfoCommand, bool>
    {
        private readonly IFileInfoRepository _fileInfoRepository;
        private readonly ILogger<CreateFileInfoCommandHandler> _logger;

        public CreateFileInfoCommandHandler(
            IFileInfoRepository fileInfoRepository,
            ILogger<CreateFileInfoCommandHandler> logger)
        {
            _fileInfoRepository = fileInfoRepository ?? throw new ArgumentNullException(nameof(fileInfoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateFileInfoCommand request, CancellationToken cancellationToken)
        {
            var fi = new FileInfo(request.Name, request.FileTag);
            _fileInfoRepository.Add(fi);
            return await _fileInfoRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
