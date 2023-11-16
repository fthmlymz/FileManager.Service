using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FileTransferManager.Api.Commands
{


    public class DownloadFileQuery : IRequest<DownloadFileResult>
    {
        public string FileName { get; }
        public string FolderType { get; }

        public DownloadFileQuery(string fileName, string folderType)
        {
            FileName = fileName;
            FolderType = folderType;
        }
    }


    public class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, DownloadFileResult>
    {
        private readonly IWebHostEnvironment _environment;
        public DownloadFileQueryHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<DownloadFileResult> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
        {
            var safeFolderName = GetSafeFolderName(request.FolderType);
            if (string.IsNullOrEmpty(safeFolderName))
            {
                return null; // Geçersiz klasör tipi
            }

            string folderPath = Path.Combine(_environment.WebRootPath, safeFolderName);
            string filePath = Path.Combine(folderPath, request.FileName);

            if (!File.Exists(filePath))
            {
                return null; // Dosya yoksa null döndür
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

            return new DownloadFileResult
            {
                FileContent = fileBytes,
                ContentType = "application/octet-stream"
            };
        }

        // ... GetSafeFolderName metodu ...
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
    public class DownloadFileResult
    {
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
    }





    /*public class DownloadFileQuery : IRequest<DownloadFileResult>
    {
        public string FileName { get; }
        public string FolderType { get; }

        public DownloadFileQuery(string fileName, string folderType)
        {
            FileName = fileName;
            FolderType = folderType;
        }
    }

    public class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, DownloadFileResult>
    {
        private readonly IWebHostEnvironment _environment;

        public DownloadFileQueryHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<DownloadFileResult> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
        {
            var safeFolderName = GetSafeFolderName(request.FolderType);
            if (string.IsNullOrEmpty(safeFolderName))
            {
                return null; // veya uygun bir hata mesajı ile bir hata sonucu döndürün
            }

            string folderPath = Path.Combine(_environment.WebRootPath, safeFolderName);
            string filePath = Path.Combine(folderPath, request.FileName);

            if (!File.Exists(filePath))
            {
                return null; // Dosya yoksa null döndür
            }

            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;

            return new DownloadFileResult
            {
                Stream = memoryStream,
                ContentType = "application/octet-stream"
            };
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

    public class DownloadFileResult
    {
        public MemoryStream Stream { get; set; }
        public string ContentType { get; set; }
    }*/
}
