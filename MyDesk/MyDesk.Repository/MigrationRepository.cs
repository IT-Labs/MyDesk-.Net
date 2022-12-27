using MyDesk.Data;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.Interfaces.Repository;
using MyDesk.Data.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MyDesk.Repository
{
    public class MigrationRepository : IMigrationRepository
    {
        private readonly IApplicationParmeters _applicationParmeters;

        public MigrationRepository(IApplicationParmeters applicationParmeters)
        {
            _applicationParmeters = applicationParmeters;
        }

        public List<ApplicationDbContext> ExecuteMigrations(DbType dbType)
        {
            List<ApplicationDbContext> applicationDbContexts = new List<ApplicationDbContext>();

            DbContextOptionsBuilder<ApplicationDbContext> defaultDbContextOptionsBuilder = GetDbOptions(dbType, _applicationParmeters.GetConnectionString());
            ApplicationDbContext defaultDbContext = new ApplicationDbContext(defaultDbContextOptionsBuilder.Options, null);
            if (defaultDbContext.Database.IsRelational())
            {
                defaultDbContext.Database.Migrate();
            }
            applicationDbContexts.Add(defaultDbContext);

            foreach (KeyValuePair<string, string> tenantData in _applicationParmeters.GetTenants())
            {
                DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = GetDbOptions(dbType, tenantData.Value);
                ApplicationDbContext dbContext = new ApplicationDbContext(dbContextOptionsBuilder.Options, null);
                if (dbContext.Database.IsRelational())
                {
                    dbContext.Database.Migrate();
                }
                applicationDbContexts.Add(dbContext);
            }

            return applicationDbContexts;
        }

        private DbContextOptionsBuilder<ApplicationDbContext> GetDbOptions(DbType dbType, string connectionString)
        {
            DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            if (dbType == DbType.SQL)
            {
                dbContextOptionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                dbContextOptionsBuilder.UseInMemoryDatabase(connectionString);
            }

            return dbContextOptionsBuilder;
        }
    }
}
