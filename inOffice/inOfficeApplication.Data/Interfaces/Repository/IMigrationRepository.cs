using inOfficeApplication.Data.Utils;

namespace inOfficeApplication.Data.Interfaces.Repository
{
    public interface IMigrationRepository
    {
        List<ApplicationDbContext> ExecuteMigrations(DbType dbType);
    }
}
