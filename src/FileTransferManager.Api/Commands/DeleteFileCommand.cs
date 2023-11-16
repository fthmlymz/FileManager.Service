using MediatR;

namespace FileTransferManager.Api.Commands
{
    public class DeleteFileCommand : IRequest<bool>
    {
        public string FileName { get; set; }
        public string FolderType { get; set; }

        public DeleteFileCommand(string fileName, string folderType)
        {
            FileName = fileName;
            FolderType = folderType;
        }
    }


    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, bool>
    {
        private readonly IWebHostEnvironment _environment;

        public DeleteFileCommandHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<bool> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var safeFolderName = GetSafeFolderName(request.FolderType);
            if (string.IsNullOrEmpty(safeFolderName))
            {
                throw new ArgumentException("Geçersiz klasör tipi");
            }

            string folderPath = Path.Combine(_environment.WebRootPath, safeFolderName);
            string filePath = Path.Combine(folderPath, request.FileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
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
