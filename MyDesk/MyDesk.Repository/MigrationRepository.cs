using MyDesk.Data;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.Interfaces.Repository;
using MyDesk.Data.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MyDesk.Repository
{
    public class MigrationRepository : IMigrationRepository
    {
        private readonly IConfiguration _config;

        public MigrationRepository(IConfiguration config)
        {
            _config = config;
        }

        public List<ApplicationDbContext> ExecuteMigrations(DbType dbType)
        {
            List<ApplicationDbContext> applicationDbContexts = new ();

            DbContextOptionsBuilder<ApplicationDbContext> defaultDbContextOptionsBuilder = GetDbOptions(dbType, _config["ConnectionString"]);
            ApplicationDbContext defaultDbContext = new (defaultDbContextOptionsBuilder.Options, null);
            if (defaultDbContext.Database.IsRelational())
            {
                defaultDbContext.Database.Migrate();
            }
            applicationDbContexts.Add(defaultDbContext);

            // Migrate each Tenant Db
            Dictionary<string, string> tenantsData;
            string tenants = _config["Tenants"];

            if (!string.IsNullOrEmpty(tenants))
            {
                tenantsData = JsonConvert.DeserializeObject<Dictionary<string, string>>(tenants);
            }
            else
            {
                tenantsData = new Dictionary<string, string>();
            }

            foreach (KeyValuePair<string, string> tenantData in tenantsData)
            {
                DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = GetDbOptions(dbType, tenantData.Value);
                ApplicationDbContext dbContext = new (dbContextOptionsBuilder.Options, null);
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
