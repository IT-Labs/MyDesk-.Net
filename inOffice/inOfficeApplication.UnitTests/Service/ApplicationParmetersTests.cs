using inOffice.BusinessLogicLayer;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Service
{
    public class ApplicationParmetersTests
    {
        private IApplicationParmeters _applicationParmeters;
        private IMemoryCache _memoryCache;
        private IConfiguration _configuration;

        [OneTimeSetUp]
        public void Setup()
        {
            _configuration = Substitute.For<IConfiguration>();
            _memoryCache = Substitute.For<IMemoryCache>();

            _applicationParmeters = new ApplicationParmeters(_configuration, _memoryCache);
        }

        [TestCase(false)]
        [TestCase(true)]
        [Order(1)]
        public void GetJwtIssuer_Success(bool isCached)
        {
            // Arrange
            string issuer = "issuer";
            string cachedIssuer = "cached issuer";

            _configuration[Arg.Is<string>(x => x == "JwtIssuer")].Returns(issuer);
            if (isCached)
            {
                _memoryCache.TryGetValue("GetJwtIssuer cache key", out Arg.Any<string>()).Returns(x =>
                {
                    x[1] = cachedIssuer;
                    return true;
                });
            }

            // Act
            string result = _applicationParmeters.GetJwtIssuer();

            // Assert
            if (isCached)
            {
                Assert.IsTrue(result == cachedIssuer);
            }
            else
            {
                Assert.IsTrue(result == issuer);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        [Order(2)]
        public void GetJwtAudience_Success(bool isCached)
        {
            // Arrange
            string audience = "audience";
            string cachedAudience = "cached audience";

            _configuration[Arg.Is<string>(x => x == "JwtAudience")].Returns(audience);
            if (isCached)
            {
                _memoryCache.TryGetValue("GetJwtAudience cache key", out Arg.Any<string>()).Returns(x =>
                {
                    x[1] = cachedAudience;
                    return true;
                });
            }

            // Act
            string result = _applicationParmeters.GetJwtAudience();

            // Assert
            if (isCached)
            {
                Assert.IsTrue(result == cachedAudience);
            }
            else
            {
                Assert.IsTrue(result == audience);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        [Order(3)]
        public void GetSettingsMetadataAddress_Success(bool isCached)
        {
            // Arrange
            string metadataAddress = "metadata address";
            string cachedMetadataAddress = "cached metadata address";

            _configuration[Arg.Is<string>(x => x == "MetadataAddress")].Returns(metadataAddress);
            if (isCached)
            {
                _memoryCache.TryGetValue("GetMetadataAddress cache key", out Arg.Any<string>()).Returns(x =>
                {
                    x[1] = cachedMetadataAddress;
                    return true;
                });
            }

            // Act
            string result = _applicationParmeters.GetMetadataAddress();

            // Assert
            if (isCached)
            {
                Assert.IsTrue(result == cachedMetadataAddress);
            }
            else
            {
                Assert.IsTrue(result == metadataAddress);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        [Order(4)]
        public void GetSettingsSentimentEndpoint_Success(bool isCached)
        {
            // Arrange
            string sentimentEndpoint = "sentiment endpoint";
            string cachedSentimentEndpoint = "cached sentiment endpoint";

            _configuration[Arg.Is<string>(x => x == "SentimentEndpoint")].Returns(sentimentEndpoint);
            if (isCached)
            {
                _memoryCache.TryGetValue("GetSentimentEndpoint cache key", out Arg.Any<string>()).Returns(x =>
                {
                    x[1] = cachedSentimentEndpoint;
                    return true;
                });
            }

            // Act
            string result = _applicationParmeters.GetSentimentEndpoint();

            // Assert
            if (isCached)
            {
                Assert.IsTrue(result == cachedSentimentEndpoint);
            }
            else
            {
                Assert.IsTrue(result == sentimentEndpoint);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        [Order(5)]
        public void GetSettingsCustomBearerTokenSigningKey_Success(bool isCached)
        {
            // Arrange
            string signingKey = "key";
            string cachedSigningKey = "cached key";

            _configuration[Arg.Is<string>(x => x == "CustomBearerTokenSigningKey")].Returns(signingKey);
            if (isCached)
            {
                _memoryCache.TryGetValue("GetCustomBearerTokenSigningKey cache key", out Arg.Any<string>()).Returns(x =>
                {
                    x[1] = cachedSigningKey;
                    return true;
                });
            }

            // Act
            string result = _applicationParmeters.GetCustomBearerTokenSigningKey();

            // Assert
            if (isCached)
            {
                Assert.IsTrue(result == cachedSigningKey);
            }
            else
            {
                Assert.IsTrue(result == signingKey);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        [Order(6)]
        public void GetTenantClaimKey_Success(bool isCached)
        {
            // Arrange
            string claimsKey = "claims key";
            string cachedClaimsKey = "cached claims key";

            _configuration[Arg.Is<string>(x => x == "TenantClaimKey")].Returns(claimsKey);
            if (isCached)
            {
                _memoryCache.TryGetValue("GetTenantClaimKey cache key", out Arg.Any<string>()).Returns(x =>
                {
                    x[1] = cachedClaimsKey;
                    return true;
                });
            }

            // Act
            string result = _applicationParmeters.GetTenantClaimKey();

            // Assert
            if (isCached)
            {
                Assert.IsTrue(result == cachedClaimsKey);
            }
            else
            {
                Assert.IsTrue(result == claimsKey);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        [Order(7)]
        public void GetTenants_Success(bool isCached)
        {
            // Arrange
            Dictionary<string, string> tenants = new Dictionary<string, string>()
            {
                { "tenant1", "tenant 1 connection string" },
                { "tenant2", "tenant 2 connection string" }
            };
            Dictionary<string, string> cachedTenants = new Dictionary<string, string>() { { "cached tenant", "cached tenant connection string" } };

            string serializedTenants = JsonConvert.SerializeObject(tenants);
            _configuration[Arg.Is<string>(x => x == "Tenants")].Returns(serializedTenants);
            if (isCached)
            {
                _memoryCache.TryGetValue("GetTenants cache key", out Arg.Any<Dictionary<string, string>>()).Returns(x =>
                {
                    x[1] = cachedTenants;
                    return true;
                });
            }

            // Act
            Dictionary<string, string> result = _applicationParmeters.GetTenants();

            // Assert
            if (isCached)
            {
                Assert.IsTrue(result.Count == 1);
                Assert.IsTrue(result.First().Key == cachedTenants.First().Key);
                Assert.IsTrue(result.First().Value == cachedTenants.First().Value);
            }
            else
            {
                Assert.IsTrue(result.Count == tenants.Count);
                Assert.IsTrue(result.First().Key == tenants.First().Key);
                Assert.IsTrue(result.Last().Key == tenants.Last().Key);
                Assert.IsTrue(result.First().Value == tenants.First().Value);
                Assert.IsTrue(result.Last().Value == tenants.Last().Value);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [Order(8)]
        public void GetTenants_Empty(string tenants)
        {
            // Arrange
            _configuration[Arg.Is<string>(x => x == "Tenants")].Returns(tenants);
            _memoryCache.TryGetValue("GetTenants cache key", out Arg.Any<Dictionary<string, string>>()).Returns(x =>
            {
                return false;
            });

            // Act
            Dictionary<string, string> result = _applicationParmeters.GetTenants();

            // Assert
            Assert.IsTrue(result.Count == 0);
        }
    }
}
