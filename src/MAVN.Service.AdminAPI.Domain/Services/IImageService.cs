using System;
using Microsoft.AspNetCore.Http;

namespace MAVN.Service.AdminAPI.Domain.Services
{
    public interface IImageService
    {
        byte[] HandleFile(IFormFile file, Guid RuleContentId);
    }
}
