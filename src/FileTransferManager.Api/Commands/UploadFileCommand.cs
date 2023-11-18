using FileTransferManager.Api.Paginated;
using MediatR;

namespace FileTransferManager.Api.Commands
{
    public sealed record UploadFileCommand(IFormFile File, string FolderType) : IRequest<Result<bool>>;

    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Result<bool>>
    {
        private readonly IWebHostEnvironment ? _environment;

        public UploadFileCommandHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<Result<bool>> Handle(UploadFileCommand request, CancellationToken cancellationToken)

        {
            if (request.File == null || request.File.Length == 0)
            {
                return Result<bool>.Success(false);
            }

            var safeFolderName = GetSafeFolderName(request.FolderType);
            if (string.IsNullOrEmpty(safeFolderName))
            {
                throw new ArgumentException("Geçersiz klasör tipi");
            }

            string uploadPath = Path.Combine(_environment.WebRootPath, safeFolderName);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string formattedFileName = $"{Path.GetFileNameWithoutExtension(request.File.FileName)} - {DateTime.Now:dd.MM.yyyy}{Path.GetExtension(request.File.FileName)}";
            string filePath = Path.Combine(uploadPath, formattedFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }

            return Result<bool>.Success(true);
        }

        private string GetSafeFolderName(string folderType)
        {
            if (!string.IsNullOrEmpty(folderType) &&
                folderType.All(char.IsLetterOrDigit))
            {
                return folderType;
            }
            return null;
        }
    }
}
