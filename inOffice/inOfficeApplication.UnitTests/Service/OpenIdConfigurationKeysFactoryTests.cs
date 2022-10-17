using inOffice.BusinessLogicLayer;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System.Text;

namespace inOfficeApplication.UnitTests.Service
{
    public class OpenIdConfigurationKeysFactoryTests
    {
        private IApplicationParmeters _applicationParmeters;
        private IMemoryCache _cache;
        private IHttpClientFactory _httpClientFactory;
        private IOpenIdConfigurationKeysFactory _openIdConfigurationKeysFactory;


        [OneTimeSetUp]
        public void Setup()
        {
            _applicationParmeters = Substitute.For<IApplicationParmeters>();
            _cache = Substitute.For<IMemoryCache>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();

            _httpClientFactory.CreateClient().Returns(new GoogleMockedHttpClient());

            _openIdConfigurationKeysFactory = new OpenIdConfigurationKeysFactory(_applicationParmeters, _cache, _httpClientFactory);
        }

        [Test]
        [Order(1)]
        public void GetKeys_GoogleAuthType_NotCached_Success()
        {
            // Arrange + Act
            IEnumerable<SecurityKey> securityKeys = _openIdConfigurationKeysFactory.GetKeys(AuthTypes.Google);

            // Assert
            Assert.IsTrue(securityKeys.Count() == 2);
        }

        [Test]
        [Order(2)]
        public void GetKeys_GoogleAuthType_Cached_Success()
        {
            // Arrange
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("test"));
            _cache.TryGetValue("Google security keys", out Arg.Any<IEnumerable<SecurityKey>>()).Returns(x => 
            { 
                x[1] = new List<SecurityKey>() { securityKey };
                return true;
            });

            // Act
            IEnumerable<SecurityKey> securityKeys = _openIdConfigurationKeysFactory.GetKeys(AuthTypes.Google);

            // Assert
            Assert.IsTrue(securityKeys.Count() == 1);
            Assert.IsTrue(securityKeys.Single() == securityKey);
        }

        [Test]
        [Order(3)]
        public void GetKeys_AzureAuthType_Cached_Success()
        {
            // Arrange
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("test321"));
            _cache.TryGetValue("Azure security keys", out Arg.Any<IEnumerable<SecurityKey>>()).Returns(x =>
            {
                x[1] = new List<SecurityKey>() { securityKey };
                return true;
            });

            // Act
            IEnumerable<SecurityKey> securityKeys = _openIdConfigurationKeysFactory.GetKeys(AuthTypes.Azure);

            // Assert
            Assert.IsTrue(securityKeys.Count() == 1);
            Assert.IsTrue(securityKeys.Single() == securityKey);
        }

        [Test]
        [Order(4)]
        public void GetKeys_CustomAuthType_Success()
        {
            // Arrange + Act
            IEnumerable<SecurityKey> securityKeys = _openIdConfigurationKeysFactory.GetKeys(AuthTypes.Custom);

            // Assert
            Assert.IsNull(securityKeys);
        }
    }

    public class GoogleMockedHttpClient : HttpClient
    {
        public override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Dictionary<string, string> dictionaryCertificates = new Dictionary<string, string>()
            {
                { "ba079b4202641e54abced8fb1354ce03919fb294", "-----BEGIN CERTIFICATE-----\nMIIDJzCCAg+gAwIBAgIJAPwYV5EwH+tgMA0GCSqGSIb3DQEBBQUAMDYxNDAyBgNV\nBAMMK2ZlZGVyYXRlZC1zaWdub24uc3lzdGVtLmdzZXJ2aWNlYWNjb3VudC5jb20w\nHhcNMjIwOTIwMTUyMjEyWhcNMjIxMDA3MDMzNzEyWjA2MTQwMgYDVQQDDCtmZWRl\ncmF0ZWQtc2lnbm9uLnN5c3RlbS5nc2VydmljZWFjY291bnQuY29tMIIBIjANBgkq\nhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAx9/VQBtJLyLBirWCPrRict7M4jCcYZSH\nZA6o435ELwdziueRVqN2Q7gXTorNaePRx2np0hPegmMrXgb4eCW4fGZ9ibeKZ7ey\nvK1+fCwezPxf74CMvzqz/Sk7wta+64Zo9fci9JfWEt3xvVYIT7o+KgzR9WLEdyse\nNDhZV1zSrzC71/R+nL4SVq6dt70AWBf6wJix6ZJTNtIe5rIiqGS8VO2b6E6f0BHb\n942XEwA5ZlUBfo4TCZo2s1uZ2FrpT1QzupARsK5iRJ/FZNUFbOOdQ0zVm7rf9558\n4lswqihIBN2TNECoSLlZxq5YTo6R+NplH9a8mPb1WSZq7ZL3Wf8A1wIDAQABozgw\nNjAMBgNVHRMBAf8EAjAAMA4GA1UdDwEB/wQEAwIHgDAWBgNVHSUBAf8EDDAKBggr\nBgEFBQcDAjANBgkqhkiG9w0BAQUFAAOCAQEAuRTGJ7Zq+zMw8viGda8qq+Xj8LEO\nt8313nb3+QNTmc86+fR43AfDDiXH6BTR0zGg9JnnWIhGXceefMAj+2w/N3lIwpVI\n9AMGNs0e4JOJbwfH9s3p6CM7MtXI/x9xvJ94rK7wh3toSyK99/IJ0DCBkIkcbqvZ\nGN5Xql0tLO80pZlCGkoEAA9pB4v/RQdC/s1vioASlT2ajPlrnuEGn/lJ6Uy1HDPD\nPqeU5KDB7uXtnd1E5uRCS6VlLRDLw/zdNAB3smiy8OPxAKljpaNLK7bh0JmLpsVE\nsqM5vTWtN/5wvnM3HKJOJo+1yeJC70tz5LmeKWHpx2KvqHbfGIimLudBSA==\n-----END CERTIFICATE-----\n" },
                { "ed36c257c47ebabb47b456828a858aa5fcda12dd", "-----BEGIN CERTIFICATE-----\nMIIDJzCCAg+gAwIBAgIJAMsUsfyYfIxgMA0GCSqGSIb3DQEBBQUAMDYxNDAyBgNV\nBAMMK2ZlZGVyYXRlZC1zaWdub24uc3lzdGVtLmdzZXJ2aWNlYWNjb3VudC5jb20w\nHhcNMjIwOTI4MTUyMjEzWhcNMjIxMDE1MDMzNzEzWjA2MTQwMgYDVQQDDCtmZWRl\ncmF0ZWQtc2lnbm9uLnN5c3RlbS5nc2VydmljZWFjY291bnQuY29tMIIBIjANBgkq\nhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA+aCIh5BgnG/83z6njWPVVzlJvLdZvLoF\nIsMcN6lkuj+GwY9Z0MA86vL5XiH1hbYm0yMLizBYL3CM5Pplrb54o/EKY5uKxPtA\nWckceQJnZBNq9YFsbOI61Jf2iPhNt08IKrJ8sOq8aTqM8UUWPmKJByo8fvzBDbmZ\nwNyyb0CLtB+jVvNURu1f+FVZwboAgKJIh6+XCL//KkPNgfW7ODaXXrk1cvm2GpgC\nNr7x+Ht5IJZwjx/TLwo9xdRPfUiEQtpUvVUghOUM/0JCfHHg95IDyz9Eo27GLvBL\ntyJK9qpm4/hhyWElXGSawvgr5ybovuoq1IUGshkQHkHX9ZK6NvBaNwIDAQABozgw\nNjAMBgNVHRMBAf8EAjAAMA4GA1UdDwEB/wQEAwIHgDAWBgNVHSUBAf8EDDAKBggr\nBgEFBQcDAjANBgkqhkiG9w0BAQUFAAOCAQEAWsihC1WlduqEnJG1q1ZbVcHtwtSA\nzeyOmZywZUqqiiSy3Bw4K6n2r40zRCD8Pddu23V9tCi+b4jYstBQg0jSCgijaTvC\nmfrDFEq51BsDSzpwEhfHKx3wia/EJ1Zk8PUOfebi2ypGrPOTwq516GqRCI1crN3k\n977w/KQXeDmGc+VUVF1CJLRW4RBwo15W7uA1SwaC+//oimF+tmVgjeHwbHwfajY7\nqbuZhc8Awqbo0MqwZhxk6p9dhAhWf1y98KnOr7wNgDvbGmG59Y/Ygf8sOUZ/FVQz\n3WUPCWbILSRabjXba2KdDAck0/Ap7d9v6A14IbWpEcsSzR9aHE15xEUPOw==\n-----END CERTIFICATE-----\n" }
            };
            string data = JsonConvert.SerializeObject(dictionaryCertificates);

            return new HttpResponseMessage() { Content = new StringContent(data, Encoding.UTF8, "application/json") };
        }
    }
}
