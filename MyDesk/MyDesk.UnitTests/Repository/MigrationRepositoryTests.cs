using MyDesk.BusinessLogicLayer;
using MyDesk.Repository;
using MyDesk.Data;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.Interfaces.Repository;
using MyDesk.Data.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using System.Configuration;

namespace MyDesk.UnitTests.Repository
{
    public class MigrationRepositoryTests
    {
        private IApplicationParmeters _applicationParmeters;
        private IMigrationRepository _migrationRepository;

        [SetUp]
        public void Setup()
        {
            _applicationParmeters = Substitute.For<IApplicationParmeters>();
            _migrationRepository = new MigrationRepository(_applicationParmeters);
        }

        [Test]
        public void ExecuteMigrations_Success()
        {
            // Arrange
            string defaultConnectionString = "default tenant";
            Dictionary<string, string> tenants = new Dictionary<string, string>() { { "tenant name", "other tenant" } };

            _applicationParmeters.GetConnectionString().Returns(defaultConnectionString);
            _applicationParmeters.GetTenants().Returns(tenants);

            // Act
            List<ApplicationDbContext> applicationDbContexts = _migrationRepository.ExecuteMigrations(DbType.InMemory);

            // Assert
            foreach (ApplicationDbContext applicationDbContext in applicationDbContexts)
            {
                applicationDbContext.Database.EnsureCreated();
                applicationDbContext.Database.CanConnect();
                Assert.IsTrue(applicationDbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory");
            }
        }
    }
}
