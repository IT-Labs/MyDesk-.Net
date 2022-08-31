using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Interface
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
