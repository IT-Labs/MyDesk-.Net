using AutoMapper;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class OfficeService : IOfficeService
    {
        private readonly IOfficeRepository _officeRepository;
        private readonly IMapper _mapper;

        public OfficeService(IOfficeRepository officeRepository, IMapper mapper)
        {
            _officeRepository = officeRepository;
            _mapper = mapper;
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

            Office office = _officeRepository.Get(request.Id);

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

            Office office = _officeRepository.Get(id);

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
            List<Office> offices = _officeRepository.GetAll(take: take, skip: skip);

            return new OfficeListResponse()
            {
                Offices = _mapper.Map<List<OfficeDto>>(offices),
                Success = true
            };
        }

        public Office GetDetailsForOffice(int id)
        {
            return _officeRepository.Get(id);
        }
    }
}
