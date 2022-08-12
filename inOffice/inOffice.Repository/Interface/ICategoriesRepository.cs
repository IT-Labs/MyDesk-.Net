using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Interface
{
    public interface ICategoriesRepository
    {
        Categories GetDeskCategories(int deskId);
        void Insert(Categories categories);
        void Update(Categories categories);
    }
}
