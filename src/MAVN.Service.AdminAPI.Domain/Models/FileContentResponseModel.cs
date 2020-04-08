namespace MAVN.Service.AdminAPI.Domain.Models
{
    public class FileContentResponseModel
    {
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
