using FileTransferManager.Service.Commands;
using FileTransferManager.Service.DTOs;
using FileTransferManager.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FileTransferManager.Service.Controllers
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

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<FileItemDto>>> ListFiles()
        {
            var files = await _mediator.Send(new ListFilesQuery());
            return Ok(files);
        }



        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var result = await _mediator.Send(new DownloadFileQuery(fileName));
            if (result == null)
            {
                return NotFound();
            }

            return File(result.Stream, result.ContentType, fileName);
        }


        [HttpDelete("delete/{folderType}/{fileName}")]
        public async Task<IActionResult> DeleteFile(string folderType, string fileName)
        {
            var result = await _mediator.Send(new DeleteFileCommand(fileName, folderType));
            if (result)
            {
                return Ok("Dosya silindi");
            }

            return NotFound("Dosya bulunamadı");
        }




        //[HttpDelete("{fileName}")]
        //public async Task<IActionResult> DeleteFile(string fileName)
        //{
        //    var command = new DeleteFileCommand { FileName = fileName };
        //    var result = await _mediator.Send(command);

        //    if (result)
        //    {
        //        return Ok();
        //    }
        //    return NotFound();
        //}


    }
}
