namespace MunicipleComplaintMgmtSys.API.Interfaces
{
    public interface IImageStorage
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
