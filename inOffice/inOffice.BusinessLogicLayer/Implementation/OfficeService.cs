using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class OfficeService : IOfficeService
    {

        private readonly IRepository<Office> _officeRepository;

        public OfficeService(IRepository<Office> officeRepository)
        {
            _officeRepository = officeRepository;  
        }

        public OfficeResponse CreateNewOffice(NewOfficeRequest o)
        {
            Office office = new Office();
            office.Name = o.OfficeName;
            office.OfficeImage = "";
           
            OfficeResponse response = new OfficeResponse();
            try
            {
                this._officeRepository.Insert(office);
                response.Success=true;
            }
            catch(Exception _)
            {
                response.Success = false;
            }

            return response;
        }

        public OfficeResponse UpdateOffice(OfficeRequest o)
        {
            OfficeResponse response = new OfficeResponse();
            var office = GetDetailsForOffice(o.Id);
            office.Name = o.OfficeName;
            office.OfficeImage=o.OfficePlan;
            try
            {
                this._officeRepository.Update(office);
                response.Success = true;
                
            }
            catch(Exception _)
            {
                response.Success = false;
            }
            return response;
        }

        public OfficeResponse DeleteOffice(int id)
        {
            var office = this.GetDetailsForOffice(id);
            OfficeResponse response = new OfficeResponse();
            try
            {
                this._officeRepository.SoftDelete(office);
                response.Success = true;
            }
            catch (Exception _)
            {
                response.Success = false;
            }
            return response;
        }

        public OfficeListResponse GetAllOffices()
        { 
            OfficeListResponse officeListResponse = new OfficeListResponse();
            try
            {
                officeListResponse.Offices = this._officeRepository.GetAll().ToList();
                officeListResponse.Success = true;
                return officeListResponse;
            }
            catch (Exception _)
            {
                officeListResponse.Success = false;
            }
            return officeListResponse;
        }

        public Office GetDetailsForOffice(int id)
        {
            return this._officeRepository.Get(id);
        }

        
    }
}
