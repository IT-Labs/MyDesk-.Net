using inOfficeApplication.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests
{
    public class ApplicationDbContextTests
    {
        [Test]
        [Order(1)]
        public void Constructor_Default_Success()
        {
            // Arrange
            string connectionString = "connection string";

            DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbContextOptionsBuilder.UseSqlServer(connectionString);

            // Act
            ApplicationDbContext applicationDbContext = new ApplicationDbContext(dbContextOptionsBuilder.Options, null);

            // Assert
            Assert.IsTrue(applicationDbContext.Database.GetConnectionString() == connectionString);
        }

        [Test]
        [Order(2)]
        public void Constructor_Tenant_Success()
        {
            // Arrange
            string defaultConnectionString = "connection string";
            string tenantConnectionString = "tenant connection string";

            DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbContextOptionsBuilder.UseSqlServer(defaultConnectionString);

            DefaultHttpContext httpContext = new DefaultHttpContext();
            httpContext.Items["tenant"] = tenantConnectionString;

            IHttpContextAccessor httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(httpContext);

            // Act
            ApplicationDbContext applicationDbContext = new ApplicationDbContext(dbContextOptionsBuilder.Options, httpContextAccessor);

            // Assert
            Assert.IsTrue(applicationDbContext.Database.GetConnectionString() == tenantConnectionString);
        }
    }
}
