using FileTransferManager.Api.DTOs;
using MediatR;

namespace FileTransferManager.Api.Queries
{
    public sealed class ListFilesQuery : IRequest<List<FileItemDto>>
    {
        public string FolderType { get; set; }
        public string FileName { get; set; }

        public ListFilesQuery(string folderType, string fileName)
        {
            FolderType = folderType;
            FileName = fileName;
        }
    }
    public sealed class ListFilesQueryHandler : IRequestHandler<ListFilesQuery, List<FileItemDto>>
    {
        private readonly IWebHostEnvironment _environment;

        public ListFilesQueryHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public async Task<List<FileItemDto>> Handle(ListFilesQuery request, CancellationToken cancellationToken)
        {
            var safeFolderName = GetSafeFolderName(request.FolderType);
            if (string.IsNullOrEmpty(safeFolderName))
            {
                throw new ArgumentException("Geçersiz klasör tipi");
            }

            string folderPath = Path.Combine(_environment.WebRootPath, safeFolderName);
            if (!Directory.Exists(folderPath))
            {
                return new List<FileItemDto>();
            }

            var fileInfos = Directory.GetFiles(folderPath)
                                     .Where(filePath => string.IsNullOrEmpty(request.FileName) || Path.GetFileName(filePath).Equals(request.FileName, StringComparison.OrdinalIgnoreCase))
                                     .Select(filePath => new FileInfo(filePath))
                                     .Select(fileInfo => new FileItemDto
                                     {
                                         FileName = fileInfo.Name,
                                         Size = fileInfo.Length
                                     }).ToList();

            return fileInfos;
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



/*
         * Tüm listeyi döndür
         * public async Task<List<FileItemDto>> Handle(ListFilesQuery request, CancellationToken cancellationToken)
        {
            var safeFolderName = GetSafeFolderName(request.FolderType);
            if (string.IsNullOrEmpty(safeFolderName))
            {
                throw new ArgumentException("Geçersiz klasör tipi");
            }
            string folderPath = Path.Combine(_environment.WebRootPath, safeFolderName);
            if (!Directory.Exists(folderPath))
            {
                return new List<FileItemDto>(); // Boş liste döndür
            }
            var fileInfos = Directory.GetFiles(folderPath)
                                     .Select(filePath => new FileInfo(filePath))
                                     .Select(fileInfo => new FileItemDto
                                     {
                                         FileName = fileInfo.Name,
                                         Size = fileInfo.Length
                                     }).ToList();
            return fileInfos;
        }*/