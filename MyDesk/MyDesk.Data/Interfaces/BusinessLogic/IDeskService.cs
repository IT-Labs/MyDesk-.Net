using MyDesk.Data.DTO;

namespace MyDesk.Data.Interfaces.BusinessLogic
{
    public interface IDeskService
    {
        void Create(int officeId, int numberOfInstancesToCreate);
        List<DeskDto> GetOfficeDesks(int id, int? take = null, int? skip = null);
        void Update(List<DeskDto> desks);
        void Delete(int id);
    }
}
