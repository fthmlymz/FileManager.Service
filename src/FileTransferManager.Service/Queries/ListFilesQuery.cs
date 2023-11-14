using FileTransferManager.Service.DTOs;
using MediatR;

namespace FileTransferManager.Service.Queries
{
    public class ListFilesQuery : IRequest<IEnumerable<FileItemDto>>
    {
    }

    public class ListFilesQueryHandler : IRequestHandler<ListFilesQuery, IEnumerable<FileItemDto>>
    {
        public async Task<IEnumerable<FileItemDto>> Handle(ListFilesQuery request, CancellationToken cancellationToken)
        {
            // Burada dosya listesini döndürün
            // Örneğin: belirli bir dizindeki tüm dosyaları listeleme

            var files = new List<FileItemDto>();
            // Dosya listesi doldurma işlemleri...

            return files;
        }
    }
}
