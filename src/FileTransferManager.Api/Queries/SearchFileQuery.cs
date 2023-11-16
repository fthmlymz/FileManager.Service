using FileTransferManager.Api.DTOs;
using MediatR;
using System.Text.RegularExpressions;

namespace FileTransferManager.Api.Queries
{
    public sealed class SearchFileQuery : IRequest<List<FileItemDto>>
    {
        public string FileName { get; set; }

        public SearchFileQuery(string fileName)
        {
            FileName = fileName;
        }
    }



public sealed class SearchFileQueryHandler : IRequestHandler<SearchFileQuery, List<FileItemDto>>
    {
        private readonly IWebHostEnvironment _environment;

        public SearchFileQueryHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<List<FileItemDto>> Handle(SearchFileQuery request, CancellationToken cancellationToken)
        {
            var fileItems = new List<FileItemDto>();
            var webRootPath = _environment.WebRootPath;

            var allFolders = Directory.GetDirectories(webRootPath, "*", SearchOption.AllDirectories);

            // Dosya adı deseni (örneğin "149 - 16.11.2023.pdf")
            var regexPattern = $"^{Regex.Escape(request.FileName)} - \\d{{2}}\\.\\d{{2}}\\.\\d{{4}}\\.pdf$";

            foreach (var folder in allFolders)
            {
                var relativeFolderName = Path.GetRelativePath(webRootPath, folder);
                var fileInfos = Directory.GetFiles(folder)
                                         .Select(filePath => new FileInfo(filePath))
                                         .Where(fileInfo => fileInfo.Name.Equals(request.FileName, StringComparison.OrdinalIgnoreCase) ||
                                                            Regex.IsMatch(fileInfo.Name, regexPattern, RegexOptions.IgnoreCase))
                                         .Select(fileInfo => new FileItemDto
                                         {
                                             FileName = fileInfo.Name,
                                             Size = fileInfo.Length / 1024,
                                             FolderName = relativeFolderName,
                                             CreationTime = fileInfo.CreationTime,
                                             LastModifiedTime = Convert.ToDateTime(fileInfo.LastWriteTime),
                                             FileType = Path.GetExtension(fileInfo.Name)
                                         });
                fileItems.AddRange(fileInfos);
            }

            return fileItems;
        }
    }

    /*
    public sealed class SearchFileQueryHandler : IRequestHandler<SearchFileQuery, List<FileItemDto>>
        {
            private readonly IWebHostEnvironment _environment;

            public SearchFileQueryHandler(IWebHostEnvironment environment)
            {
                _environment = environment;
            }

            public async Task<List<FileItemDto>> Handle(SearchFileQuery request, CancellationToken cancellationToken)
            {
                var fileItems = new List<FileItemDto>();
                var webRootPath = _environment.WebRootPath;

                var allFolders = Directory.GetDirectories(webRootPath, "*", SearchOption.AllDirectories);

                // Dosya adı deseni (örneğin "149 - 16.11.2023.pdf")
                var regexPattern = $"^{Regex.Escape(request.FileName)} - \\d{{2}}\\.\\d{{2}}\\.\\d{{4}}\\.pdf$";

                foreach (var folder in allFolders)
                {
                    var relativeFolderName = Path.GetRelativePath(webRootPath, folder);
                    var fileInfos = Directory.GetFiles(folder)
                                             .Select(filePath => new FileInfo(filePath))
                                             .Where(fileInfo => fileInfo.Name.Equals(request.FileName, StringComparison.OrdinalIgnoreCase) ||
                                                                (relativeFolderName.Contains("HistoryFiles") &&
                                                                 Regex.IsMatch(fileInfo.Name, regexPattern, RegexOptions.IgnoreCase)))
                                             .Select(fileInfo => new FileItemDto
                                             {
                                                 FileName = fileInfo.Name,
                                                 Size = fileInfo.Length / 1024,
                                                 FolderName = relativeFolderName,
                                                 CreationTime = fileInfo.CreationTime,
                                                 LastModifiedTime = Convert.ToDateTime(fileInfo.LastWriteTime),
                                                 FileType = Path.GetExtension(fileInfo.Name)
                                             });
                    fileItems.AddRange(fileInfos);
                }

                return fileItems;
            }
        }
    */








    /*public sealed class SearchFileQueryHandler : IRequestHandler<SearchFileQuery, List<FileItemDto>>
    {
        private readonly IWebHostEnvironment _environment;

        public SearchFileQueryHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<List<FileItemDto>> Handle(SearchFileQuery request, CancellationToken cancellationToken)
        {
            var fileItems = new List<FileItemDto>();
            var webRootPath = _environment.WebRootPath;

            foreach (var folder in Directory.GetDirectories(webRootPath))
            {
                var folderName = Path.GetFileName(folder);
                var fileInfos = Directory.GetFiles(folder)
                                         .Where(filePath => Path.GetFileName(filePath).Equals(request.FileName, StringComparison.OrdinalIgnoreCase))
                                         .Select(filePath => new FileInfo(filePath))
                                         .Select(fileInfo => new FileItemDto
                                         {
                                             FileName = fileInfo.Name,
                                             Size = fileInfo.Length / 1024,
                                             FolderName = folderName,
                                             CreationTime = fileInfo.CreationTime,
                                             LastModifiedTime = Convert.ToDateTime(fileInfo.LastWriteTime),
                                             FileType = Path.GetExtension(fileInfo.Name)
                                         });
                fileItems.AddRange(fileInfos);
            }
            return fileItems;
        }
    }*/
}
