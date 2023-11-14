using MediatR;

namespace FileTransferManager.Service.Commands
{
    public class UploadFileCommand : IRequest<bool>
    {
        public IFormFile File { get; set; }
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

            string uploadPath = Path.Combine(_environment.WebRootPath, "Zimmet");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string filePath = Path.Combine(uploadPath, request.File.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }

            return true; // İşlem başarılıysa
        }
    }
}
