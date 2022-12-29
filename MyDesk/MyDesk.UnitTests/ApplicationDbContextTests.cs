using MyDesk.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;

namespace MyDesk.UnitTests
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

            DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbContextOptionsBuilder.UseSqlServer(defaultConnectionString);

            DefaultHttpContext httpContext = new ();

            IHttpContextAccessor httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(httpContext);

            // Act
            ApplicationDbContext applicationDbContext = new ApplicationDbContext(dbContextOptionsBuilder.Options, httpContextAccessor);

            // Assert
            Assert.That(applicationDbContext.Database.GetConnectionString(), Is.EqualTo(defaultConnectionString));
        }
    }
}
