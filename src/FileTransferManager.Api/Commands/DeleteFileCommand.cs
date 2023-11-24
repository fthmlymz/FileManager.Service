using FileTransferManager.Api.Paginated;
using MediatR;

namespace FileTransferManager.Api.Commands
{
    public sealed class DeleteFileCommand : IRequest<Result<bool>>

    {
        public string FileName { get; set; }
        public string FolderType { get; set; }

        public DeleteFileCommand(string fileName, string folderType)
        {
            FileName = fileName;
            FolderType = folderType;
        }
    }
    public sealed class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, Result<bool>>
    {
        private readonly IWebHostEnvironment _environment;

        public DeleteFileCommandHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<Result<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var safeFolderName = GetSafeFolderName(request.FolderType);
            if (string.IsNullOrEmpty(safeFolderName))
            {
                return Result<bool>.Failure("Geçersiz klasör tipi");
            }

            string folderPath = Path.Combine(_environment.WebRootPath, safeFolderName);
            string historyFolderPath = Path.Combine(_environment.WebRootPath, "HistoryFiles");

            Directory.CreateDirectory(historyFolderPath);

            var matchingFiles = Directory.EnumerateFiles(folderPath, request.FileName + ".*");

            if (!matchingFiles.Any())
            {
                return Result<bool>.Failure("Dosya bulunamadı.");
            }

            try
            {
                foreach (var file in matchingFiles)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(historyFolderPath, fileName);

                    if (File.Exists(destFile))
                    {
                        string newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-DeletedDate-{DateTime.Now:dd-MM-yyyy-HH-mm-ss}{Path.GetExtension(fileName)}";
                        destFile = Path.Combine(historyFolderPath, newFileName);
                    }
                    File.Move(file, destFile);
                }

                return Result<bool>.Success(true, "Dosya başarıyla silindi");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure("Dosya silme işlemi sırasında bir hata oluştu: " + ex.Message);
            }
        }



        private string GetSafeFolderName(string folderType)
        {
            if (!string.IsNullOrEmpty(folderType) && folderType.All(char.IsLetterOrDigit))
            {
                return folderType;
            }
            return null;
        }
    }
}
/*public sealed class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, Result<bool>>
   {
       private readonly IWebHostEnvironment ? _environment;

       public DeleteFileCommandHandler(IWebHostEnvironment environment)
       {
           _environment = environment;
       }
       public async Task<Result<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
       {
           var safeFolderName = GetSafeFolderName(request.FolderType);
           if (string.IsNullOrEmpty(safeFolderName))
           {
               return Result<bool>.Failure("Geçersiz klasör tipi");
           }

           string folderPath = Path.Combine(_environment.WebRootPath, safeFolderName);

           var matchingFiles = Directory.EnumerateFiles(folderPath, request.FileName + ".*");

           if (!matchingFiles.Any())
           {
               return Result<bool>.Failure("Dosya bulunamadı.");
           }

           foreach (var file in matchingFiles)
           {
               File.Delete(file);
           }

           return Result<bool>.Success(true);
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
   }*/