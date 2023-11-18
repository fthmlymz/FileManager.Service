using FileTransferManager.Api.Paginated;
using MediatR;

namespace FileTransferManager.Api.Commands
{
    public sealed class DownloadFileCommand : IRequest<Result<DownloadFileResult>>
    {
        public string FileName { get; }
        public string FolderType { get; }

        public DownloadFileCommand(string fileName, string folderType)
        {
            FileName = fileName;
            FolderType = folderType;
        }
    }
    public sealed class DownloadFileQueryHandler : IRequestHandler<DownloadFileCommand, Result<DownloadFileResult>>
    {
        private readonly IWebHostEnvironment? _environment;
        public DownloadFileQueryHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<Result<DownloadFileResult>> Handle(DownloadFileCommand request, CancellationToken cancellationToken)
        {
            var safeFolderName = GetSafeFolderName(request.FolderType);
            if (string.IsNullOrEmpty(safeFolderName))
            {
                return Result<DownloadFileResult>.Failure("Klasör adı boş olamaz.");
            }

            string folderPath = Path.Combine(_environment.WebRootPath, safeFolderName);
            var matchingFiles = Directory.EnumerateFiles(folderPath)
                                         .Where(file => Path.GetFileNameWithoutExtension(file) == request.FileName)
                                         .FirstOrDefault();

            if (string.IsNullOrEmpty(matchingFiles))
            {
                return Result<DownloadFileResult>.Failure("Dosya bulunamadı.");
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(matchingFiles);
            return Result<DownloadFileResult>.Success(new DownloadFileResult
            {
                FileContent = fileBytes,
                ContentType = "application/octet-stream"
            });
        }


        private string ? GetSafeFolderName(string folderType)
        {
            if (!string.IsNullOrEmpty(folderType) &&
                folderType.All(char.IsLetterOrDigit))
            {
                return folderType;
            }
            return null;
        }
    }
    public class DownloadFileResult
    {
        public byte[] ? FileContent { get; set; }
        public string ? ContentType { get; set; }
    }
}
