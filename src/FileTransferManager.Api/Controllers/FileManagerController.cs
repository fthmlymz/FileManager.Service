using FileTransferManager.Api.Commands;
using FileTransferManager.Api.Queries;
using MediatR;
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

        [HttpGet("searchFile/{fileName}")]
        public async Task<IActionResult> SearchFile(string fileName)
        {
            var result = await _mediator.Send(new SearchFileQuery(fileName));
            if (result != null && result.Any())
            {
                return Ok(result);
            }

            return NotFound();
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileCommand command)
        {
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("download/{folderType}/{fileName}")]
        public async Task<IActionResult> DownloadFile(string folderType, string fileName)
        {
            var result = await _mediator.Send(new DownloadFileQuery(fileName, folderType));
            if (result != null)
            {
                return File(result.FileContent, result.ContentType, fileName);
            }

            return NotFound("Dosya bulunamadı.");
        }


        [HttpDelete("delete/{folderType}/{fileName}")]
        public async Task<IActionResult> DeleteFile(string folderType, string fileName)
        {
            var result = await _mediator.Send(new DeleteFileCommand(fileName, folderType));
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }





        //[HttpGet("getFile/{folderType}/{fileName?}")]
        //public async Task<IActionResult> ListFiles(string folderType, string fileName = null)
        //{
        //    var result = await _mediator.Send(new ListFilesQuery(folderType, fileName));
        //    if (result != null)
        //    {
        //        return Ok(result);
        //    }

        //    return NotFound();
        //}

        //[HttpGet("list/{folderType}")]
        //public async Task<IActionResult> ListFiles(string folderType)
        //{
        //    var result = await _mediator.Send(new ListFilesQuery(folderType));
        //    if (result != null)
        //    {
        //        return Ok(result);
        //    }
        //    return NotFound();
        //}

        //[HttpGet("download/{folderType}/{fileName}")]
        //public async Task<IActionResult> DownloadFile(string folderType, string fileName)
        //{
        //    var result = await _mediator.Send(new DownloadFileQuery(fileName, folderType));
        //    if (result != null)
        //    {
        //        return File(result.Stream, result.ContentType, fileName);
        //    }
        //    return NotFound();
        //}

        //[HttpGet("download/{folderType}/{fileName}")]
        //public async Task<IActionResult> DownloadFile(string folderType, string fileName)
        //{
        //    var result = await _mediator.Send(new DownloadFileQuery(fileName, folderType));
        //    if (result?.Stream == null)
        //    {
        //        return NotFound("Dosya bulunamadı.");
        //    }
        //    return File(result.Stream, result.ContentType, fileName);
        //}
    }
}
