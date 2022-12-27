using MyDesk.Data.DTO;

namespace MyDesk.Data.Interfaces.BusinessLogic
{
    public interface IOfficeService
    {
        List<OfficeDto> GetAllOffices(int? take = null, int? skip = null);
        OfficeDto GetDetailsForOffice(int id);
        void CreateNewOffice(OfficeDto officeDto);
        void DeleteOffice(int id);
        void UpdateOffice(OfficeDto officeDto);

    }
}
