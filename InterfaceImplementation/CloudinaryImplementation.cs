using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Identity.Client;
using MunicipleComplaintMgmtSys.API.Interfaces;

namespace MunicipleComplaintMgmtSys.API.InterfaceImplementation
{
    public class CloudinaryImplementation : IImageStorage
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImplementation(IConfiguration configuration)
        {
            var account = new Account(
                configuration["CloudinarySettings:CloudName"],
                configuration["CloudinarySettings:ApiKey"],
                configuration["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file.Length == 0)
                throw new ArgumentException("File is empty");

            await using var fileStream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, fileStream),
                Folder = "MunicipleComplaint",
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = true,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return uploadResult.SecureUrl.ToString();

        }
    }
}
