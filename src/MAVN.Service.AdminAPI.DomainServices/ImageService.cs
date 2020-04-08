using System;
using System.Linq;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.AdminAPI.Domain.Services;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class ImageService : IImageService
    {
        private const string PngContentType = "image/png";
        private const string JpegContentType = "image/jpeg";
        private static readonly string[] ImageContentTypes = { PngContentType, JpegContentType };
        private readonly bool _mobileAppImageDoOptimization;
        private readonly int _mobileAppImageMinWidth;
        private readonly int _mobileAppImageWarningFileSizeInKB;
        private readonly ILog _log;

        public ImageService(
            bool mobileAppImageDoOptimization,
            int mobileAppImageMinWidth,
            int mobileAppImageWarningFileSizeInKB,
            ILogFactory logFactory
        )
        {
            _mobileAppImageDoOptimization = mobileAppImageDoOptimization;
            _mobileAppImageMinWidth = mobileAppImageMinWidth;
            _mobileAppImageWarningFileSizeInKB = mobileAppImageWarningFileSizeInKB;
            _log = logFactory.CreateLog(this);
        }

        public byte[] HandleFile(IFormFile file, Guid RuleContentId)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                file.CopyTo(ms);

                if (ImageContentTypes.Contains(file.ContentType) && _mobileAppImageDoOptimization)
                {
                    using (var image = Image.Load(ms.ToArray()))
                    {
                        var originalWidth = image.Width;
                        var originalHeight = image.Height;
                        var heightToWidthCoefficient = image.Height * 1.0 / image.Width;

                        if (originalWidth > _mobileAppImageMinWidth)
                        {
                            var newHeight = Math.Round(_mobileAppImageMinWidth * heightToWidthCoefficient, MidpointRounding.AwayFromZero);

                            image.Mutate(x => x.Resize(
                                new ResizeOptions
                                {
                                    Mode = heightToWidthCoefficient < 1 ? ResizeMode.Max : ResizeMode.Min,
                                    Size = new Size(
                                        _mobileAppImageMinWidth,
                                        newHeight <= int.MaxValue ? (int) newHeight : int.MaxValue
                                    )
                                }
                            ));
                        }

                        using (var outStream = new System.IO.MemoryStream())
                        {
                            image.Save(outStream, new JpegEncoder()
                            {
                                Quality = 80
                            });

                            var byteArray = outStream.ToArray();

                            var logData = new
                            {
                                RuleContentId,
                                file.ContentType,
                                originalFileLength = file.Length,
                                resizedFileLength = byteArray.Length,
                                originalWidth,
                                originalHeight,
                                heightToWidthCoefficient,
                                resizedWidth = image.Width,
                                resizedHeight = image.Height
                            };

                            _log.Info($"Image handled. Data = '{logData}'", context: new { RuleContentId });

                            if (byteArray.Length / 1024 > _mobileAppImageWarningFileSizeInKB)
                            {
                                _log.Warning($"Handled image file size ({byteArray.Length}) exceeds MobileAppImageWarningFileSizeInKB: {_mobileAppImageWarningFileSizeInKB} KB.", context: new { RuleContentId });
                            }

                            return byteArray;
                        }
                    }
                }
                else
                {
                    return ms.ToArray();
                }
            }
        }
    }
}
