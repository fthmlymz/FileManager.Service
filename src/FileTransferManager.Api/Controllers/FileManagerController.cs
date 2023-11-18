using FileTransferManager.Api.Commands;
using FileTransferManager.Api.DTOs;
using FileTransferManager.Api.Paginated;
using FileTransferManager.Api.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileTransferManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FileManagerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize("FileTransferReadRole")]
        [HttpGet("search/{fileName}")]
        public async Task<ActionResult<Result<List<FileItemDto>>>> SearchFile(string fileName)
        {
            return await _mediator.Send(new SearchFileQuery(fileName));
        }

        [Authorize("FileTransferCreateRole")]
        [HttpPost("upload")]
        public async Task<ActionResult<Result<bool>>> UploadFile([FromForm] UploadFileCommand command)
        {
            return await _mediator.Send(command);
        }

        [Authorize("FileTransferReadRole")]
        [HttpGet("download/{folderType}/{fileName}")]
        public async Task<IActionResult> DownloadFile(string folderType, string fileName)
        {
            var result = await _mediator.Send(new DownloadFileCommand(fileName, folderType));

            if (result.Succeeded && result.Data != null)
            {
                return File(result.Data.FileContent, result.Data.ContentType, fileName);
            }

            return NotFound(result.Messages.FirstOrDefault() ?? "Dosya bulunamadı.");
        }

        [Authorize("FileTransferDeleteRole")]
        [HttpDelete("delete/{folderType}/{fileName}")]
        public async Task<ActionResult<Result<bool>>> DeleteFile(string folderType, string fileName)
        {
            return await _mediator.Send(new DeleteFileCommand(fileName, folderType));
        }
    }
}
