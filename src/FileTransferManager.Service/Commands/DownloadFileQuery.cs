using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FileTransferManager.Service.Commands
{
    public class DownloadFileQuery : IRequest<DownloadFileResult>
    {
        public string FileName { get; }

        public DownloadFileQuery(string fileName)
        {
            FileName = fileName;
        }
    }

    public class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, DownloadFileResult>
    {
        public async Task<DownloadFileResult> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
        {
            // Burada dosyayı okuyup MemoryStream olarak döndürün
            // Örneğin: sunucunuzda bir dosyayı okuyup MemoryStream'e yazma

            return new DownloadFileResult
            {
                Stream = new MemoryStream(), // Dosya içeriği
                ContentType = "application/octet-stream" // Dosya türü
            };
        }
    }

    public class DownloadFileResult
    {
        public MemoryStream ? Stream { get; set; }
        public string ? ContentType { get; set; }
    }
}
