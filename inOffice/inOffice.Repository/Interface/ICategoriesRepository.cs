using inOfficeApplication.Data.Entities;

namespace inOffice.Repository.Interface
{
    public interface ICategoriesRepository
    {
        Category Get(bool? doubleMonitor, bool? nearWindow, bool? singleMonitor, bool? unavailable);
        void Insert(Category categories);
        void Update(Category categories);
    }
}
