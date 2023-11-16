namespace FileTransferManager.Api.DTOs
{
    public sealed class FileItemDto
    {
        public string FileName { get; set; }
        public long Size { get; set; } 
        public string FolderName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string FileType { get; set; } 
    }
}
