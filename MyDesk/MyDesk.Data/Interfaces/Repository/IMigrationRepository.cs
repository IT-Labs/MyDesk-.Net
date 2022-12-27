using MyDesk.Data.Utils;

namespace MyDesk.Data.Interfaces.Repository
{
    public interface IMigrationRepository
    {
        List<ApplicationDbContext> ExecuteMigrations(DbType dbType);
    }
}
