using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class OfficeService : IOfficeService
    {
        private readonly IOfficeRepository _officeRepository;

        public OfficeService(IOfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        public OfficeResponse CreateNewOffice(NewOfficeRequest request)
        {
            OfficeResponse response = new OfficeResponse();
            Office office = new Office()
            {
                Name = request.OfficeName,
                OfficeImage = string.Empty
            };

            Office existingOffice = _officeRepository.GetByName(office.Name);

            if (existingOffice != null)
            {
                response.Success = false;
            }
            else
            {
                _officeRepository.Insert(office);
                response.Success = true;
            }

            return response;
        }

        public OfficeResponse UpdateOffice(OfficeRequest request)
        {
            OfficeResponse response = new OfficeResponse();

            Office office = GetDetailsForOffice(request.Id);

            if (office == null)
            {
                response.Success = false;
                return response;
            }

            office.Name = request.OfficeName;
            office.OfficeImage = request.OfficePlan;

            _officeRepository.Update(office);
            response.Success = true;

            return response;
        }

        public OfficeResponse DeleteOffice(int id)
        {
            OfficeResponse response = new OfficeResponse();

            Office office = GetDetailsForOffice(id);

            if (office == null)
            {
                response.Success = false;
                return response;
            }

            _officeRepository.SoftDelete(office);
            response.Success = true;

            return response;
        }

        public OfficeListResponse GetAllOffices(int? take = null, int? skip = null)
        {
            OfficeListResponse officeListResponse = new OfficeListResponse();

            officeListResponse.Offices = _officeRepository.GetAll(take: take, skip: skip);
            officeListResponse.Success = true;

            return officeListResponse;
        }

        public Office GetDetailsForOffice(int id)
        {
            return _officeRepository.Get(id);
        }
    }
}
