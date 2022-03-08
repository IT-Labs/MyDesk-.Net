using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IOfficeService
    {
        OfficeListResponse GetAllOffices();
        Office GetDetailsForOffice(int id);
        OfficeResponse CreateNewOffice(OfficeRequest o);
        OfficeResponse DeleteOffice(int id);
        OfficeResponse UpdateOffice(OfficeRequest o);

    }
}
