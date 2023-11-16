using MediatR;

namespace FileTransferManager.Api.Commands
{
    public class UploadFileCommand : IRequest<bool>
    {
        public IFormFile File { get; set; }
        public string FolderType { get; set; }
    }
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, bool>
    {
        private readonly IWebHostEnvironment _environment;

        public UploadFileCommandHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<bool> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return false;
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

            return true;
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
