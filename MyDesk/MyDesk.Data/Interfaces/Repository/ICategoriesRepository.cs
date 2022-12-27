using MyDesk.Data.Entities;

namespace MyDesk.Data.Interfaces.Repository
{
    public interface ICategoriesRepository
    {
        Category Get(bool? doubleMonitor, bool? nearWindow, bool? singleMonitor, bool? unavailable);
        void Insert(Category categories);
        void Update(Category categories);
    }
}
