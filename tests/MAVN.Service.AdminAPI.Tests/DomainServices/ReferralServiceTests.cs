using System.Threading.Tasks;
using Lykke.Service.Referral.Client;
using Lykke.Service.Referral.Client.Enums;
using Lykke.Service.Referral.Client.Models.Requests;
using Lykke.Service.Referral.Client.Models.Responses;
using MAVN.Service.AdminAPI.DomainServices;
using Moq;
using Xunit;

namespace MAVN.Service.AdminAPI.Tests.DomainServices
{
    public class ReferralServiceTests
    {
        [Fact]
        public async Task ShouldGetReferralCode_WhenReferralCodeExits()
        {
            // Arrange
            var referralClient = new Mock<IReferralClient>();
            var referralCode = "refcod";
            referralClient.Setup(c => c.ReferralApi.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new ReferralResultResponse()
                {
                    ReferralCode = referralCode
                });

            var service = new ReferralService(referralClient.Object);

            // Act
            var result = await service.GetOrCreateReferralCodeAsync("123");

            // Assert
            Assert.Equal(referralCode, result);
        }

        [Fact]
        public async Task ShouldCreateReferralCode_WhenReferralCodeDoesNotExists()
        {
            // Arrange
            var referralClient = new Mock<IReferralClient>();
            var referralCode = "refcod";

            referralClient.Setup(c => c.ReferralApi.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new ReferralResultResponse()
                {
                    ReferralCode = null,
                    ErrorCode = ReferralErrorCodes.ReferralNotFound
                });

            referralClient.Setup(c => c.ReferralApi.PostAsync(It.IsAny<ReferralCreateRequest>()))
                .ReturnsAsync(new ReferralCreateResponse()
                {
                    ReferralCode = referralCode
                });

            var service = new ReferralService(referralClient.Object);

            // Act
            var result = await service.GetOrCreateReferralCodeAsync("123");

            // Assert
            Assert.Equal(referralCode, result);
        }

        [Fact]
        public async Task ShouldReturnNull_WhenReferralCreateFails()
        {
            // Arrange
            var referralClient = new Mock<IReferralClient>();

            referralClient.Setup(c => c.ReferralApi.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new ReferralResultResponse()
                {
                    ReferralCode = null,
                    ErrorCode = ReferralErrorCodes.ReferralNotFound
                });

            referralClient.Setup(c => c.ReferralApi.PostAsync(It.IsAny<ReferralCreateRequest>()))
                .ReturnsAsync(new ReferralCreateResponse()
                {
                    ReferralCode = null
                });

            var service = new ReferralService(referralClient.Object);

            // Act
            var result = await service.GetOrCreateReferralCodeAsync("123");

            // Assert
            Assert.Null(result);
        }
    }
}
