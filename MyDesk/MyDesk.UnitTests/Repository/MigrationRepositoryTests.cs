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
        private IConfiguration _config;
        private IMigrationRepository _migrationRepository;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                  {"ConnectionString", "default tenant"},
                  {"Tenants:tenant name", "other tenant" }
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _migrationRepository = new MigrationRepository(_config);
        }

        [Test]
        public void ExecuteMigrations_Success()
        {
            // Act
            List<ApplicationDbContext> applicationDbContexts = _migrationRepository.ExecuteMigrations(DbType.InMemory);

            // Assert
            foreach (ApplicationDbContext applicationDbContext in applicationDbContexts)
            {
                applicationDbContext.Database.EnsureCreated();
                applicationDbContext.Database.CanConnect();
                Assert.That(applicationDbContext.Database.ProviderName, Is.EqualTo("Microsoft.EntityFrameworkCore.InMemory"));
            }
        }
    }
}
