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
    public class EntitiesService : IEntitiesService
    {
        private readonly IRepository<Desk> _deskRepository;
        private readonly IRepository<ConferenceRoom> _conferenceRoomRepository;

        public EntitiesService(IRepository<Desk> deskRepository, IRepository<ConferenceRoom> conferenceRoomRepository)
        {
            _deskRepository = deskRepository;
            _conferenceRoomRepository = conferenceRoomRepository;

        }

        public EntitiesResponse CreateNewEntities(EntitiesRequest o)
        {

            EntitiesResponse response = new EntitiesResponse();
            var indexForDesk = this._deskRepository.GetAll().Where(x => x.OfficeId == o.Id).ToList().Count();
            var indexForConferenceRoom = this._conferenceRoomRepository.GetAll().Where(x => x.OfficeId == o.Id).ToList().Count();

            try
            {
                for (int i = 0; i < o.NumberOfDesks; i++)
                {
                    Desk desk = new Desk();

                    desk.OfficeId = o.Id;
                    desk.IsDeleted = false;
                    desk.Categories = "regular";
                    desk.IndexForOffice = indexForDesk + i + 1;

                    this._deskRepository.Insert(desk);

                }

                for (int i = 0; i < o.NumberOfConferenceRooms; i++)
                {
                    ConferenceRoom conferenceRoom = new ConferenceRoom();

                    conferenceRoom.OfficeId = o.Id;
                    conferenceRoom.IsDeleted = false;
                    conferenceRoom.IndexForOffice = indexForConferenceRoom + i + 1;

                    this._conferenceRoomRepository.Insert(conferenceRoom);

                }

                response.Success = true;
            }
            catch (Exception _)
            {
                response.Success = false;
            }

            return response;
        }

        public DesksResponse ListAllDesks(int id)
        {
            DesksResponse responseDeskList = new DesksResponse();

            try
            {

                responseDeskList.DeskList = this._deskRepository.GetAll().Where(x => x.OfficeId == id).ToList();

                responseDeskList.sucess = true;

                return responseDeskList;
            }
            
            catch(Exception _)
            {
                responseDeskList.sucess= false;

                return responseDeskList;
            }
        }

        public DeleteResponse DeleteEntity(DeleteRequest o)
        {
            DeleteResponse deleteResponse = new DeleteResponse();

            try {

                if(o.TypeOfEntity == "D")
                {
                    var desk = _deskRepository.Get(o.IdOfEntity);
                    this._deskRepository.Delete(desk);
                    deleteResponse.Success = true;

                }
                else
                {
                    var conferenceRoomToDelete = _conferenceRoomRepository.Get(o.IdOfEntity);
                    this._conferenceRoomRepository.Delete(conferenceRoomToDelete);
                    deleteResponse.Success = true;

                }

                return deleteResponse;
                
            }
            catch(Exception _)
            {   
                deleteResponse.Success = false;
                return deleteResponse;

            }

        }
        public ConferenceRoomsResponse ListAllConferenceRooms(int id)
        {
            ConferenceRoomsResponse responseConferenceRoom = new ConferenceRoomsResponse();

            try
            {

                responseConferenceRoom.ConferenceRoomsList = this._conferenceRoomRepository.GetAll().Where(x => x.OfficeId == id).ToList();

                responseConferenceRoom.Sucess = true;

                return responseConferenceRoom;
            }
            catch (Exception _)
            {
                responseConferenceRoom.Sucess= false;  

                return responseConferenceRoom;
            }

            
        }

        public EntitiesResponse UpdateEntities(UpdateRequest o)
        {

            var desks = o.CheckedDesks;
            var conferenceRooms = o.ConferenceRoomCapacity;
            var uncheckedDesks = o.UncheckedDesks;
            EntitiesResponse entitiesResponse = new EntitiesResponse();

            try
            {
                for (int i = 0; i < desks.Count; i++)
                {
                    var desk = _deskRepository.Get(desks[i]);
                    desk.Categories = "silent";
                    this._deskRepository.Update(desk);
                }
                for (int i = 0; i < uncheckedDesks.Count; i++)
                {
                    var desk = _deskRepository.Get(uncheckedDesks[i]);
                    desk.Categories = "regular";
                    this._deskRepository.Update(desk);
                }
                foreach (var room in conferenceRooms)
                {
                    var roomToUpdate = _conferenceRoomRepository.Get(room.confId);
                    roomToUpdate.Capacity = room.confCap;
                    _conferenceRoomRepository.Update(roomToUpdate);
                }

                entitiesResponse.Success = true;

                return entitiesResponse;
            }

            catch(Exception _)
            {
                entitiesResponse.Success = false;

                return entitiesResponse;
            }       

        }
    }
}
