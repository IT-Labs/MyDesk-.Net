using inOffice.BusinessLogicLayer;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Service
{
    public class ApplicationParmetersTests
    {
        private IApplicationParmeters _applicationParmeters;
        private IConfiguration _configuration;

        [OneTimeSetUp]
        public void Setup()
        {
            _configuration = Substitute.For<IConfiguration>();
            _applicationParmeters = new ApplicationParmeters(_configuration);
        }

        [Test]
        [Order(1)]
        public void GetJwtIssuer_Success()
        {
            // Arrange
            string issuer = "issuer";
            _configuration[Arg.Is<string>(x => x == "JwtIssuer")].Returns(issuer);

            // Act
            string result = _applicationParmeters.GetJwtIssuer();

            // Assert
            Assert.IsTrue(result == issuer);
        }

        [Test]
        [Order(2)]
        public void GetJwtAudience_Success()
        {
            // Arrange
            string audience = "audience";
            _configuration[Arg.Is<string>(x => x == "JwtAudience")].Returns(audience);

            // Act
            string result = _applicationParmeters.GetJwtAudience();

            // Assert
            Assert.IsTrue(result == audience);
        }

        [Test]
        [Order(3)]
        public void GetSettingsMetadataAddress_Success()
        {
            // Arrange
            string metadataAddress = "metadata address";
            _configuration[Arg.Is<string>(x => x == "MetadataAddress")].Returns(metadataAddress);

            // Act
            string result = _applicationParmeters.GetMetadataAddress();

            // Assert
            Assert.IsTrue(result == metadataAddress);
        }

        [Test]
        [Order(4)]
        public void GetSettingsSentimentEndpoint_Success()
        {
            // Arrange
            string sentimentEndpoint = "sentiment endpoint";
            _configuration[Arg.Is<string>(x => x == "SentimentEndpoint")].Returns(sentimentEndpoint);

            // Act
            string result = _applicationParmeters.GetSentimentEndpoint();

            // Assert
            Assert.IsTrue(result == sentimentEndpoint);
        }

        [Test]
        [Order(5)]
        public void GetSettingsCustomBearerTokenSigningKey_Success()
        {
            // Arrange
            string signingKey = "key";
            _configuration[Arg.Is<string>(x => x == "CustomBearerTokenSigningKey")].Returns(signingKey);

            // Act
            string result = _applicationParmeters.GetCustomBearerTokenSigningKey();

            // Assert
            Assert.IsTrue(result == signingKey);
        }
    }
}
