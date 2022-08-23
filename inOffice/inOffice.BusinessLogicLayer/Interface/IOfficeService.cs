using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Entities;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IOfficeService
    {
        OfficeListResponse GetAllOffices(int? take = null, int? skip = null);
        Office GetDetailsForOffice(int id);
        OfficeResponse CreateNewOffice(NewOfficeRequest request);
        OfficeResponse DeleteOffice(int id);
        OfficeResponse UpdateOffice(OfficeRequest request);

    }
}
