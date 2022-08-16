﻿using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using System.Transactions;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class EntitiesService : IEntitiesService
    {
        private readonly IDeskRepository _deskRepository;
        private readonly IConferenceRoomRepository _conferenceRoomRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ICategoriesRepository _categoriesRepository;

        public EntitiesService(IDeskRepository deskRepository,
            IConferenceRoomRepository conferenceRoomRepository,
            IReservationRepository reservationRepository,
            ICategoriesRepository categoriesRepository)
        {
            _deskRepository = deskRepository;
            _conferenceRoomRepository = conferenceRoomRepository;
            _reservationRepository = reservationRepository;
            _categoriesRepository = categoriesRepository;
        }

        public AllReviewsForEntity AllReviewsForEntity(int id)
        {
            AllReviewsForEntity response = new AllReviewsForEntity();

            List<Reservation> deskReservations = _reservationRepository.GetDeskReservations(id, includeReview: true);

            response.AllReviews = deskReservations.Where(x => x.Review != null).Select(x => x.Review.Reviews).ToList();
            response.Success = response.AllReviews.Count > 0;

            return response;
        }

        public EntitiesResponse CreateNewDesks(EntitiesRequest request)
        {
            EntitiesResponse response = new EntitiesResponse();

            int deskCount = _deskRepository.GetOfficeDesks(request.Id).Count();

            List<Desk> desksToInsert = new List<Desk>();

            for (int i = 0; i < request.NumberOfDesks; i++)
            {
                Desk desk = new Desk()
                {
                    OfficeId = request.Id,
                    IsDeleted = false,
                    Categories = "regular",
                    IndexForOffice = deskCount + i + 1
                };

                desksToInsert.Add(desk);
            }

            _deskRepository.BulkInsert(desksToInsert);

            response.Success = true;

            return response;
        }

        public DesksResponse ListAllDesks(int id, int? take = null, int? skip = null)
        {
            DesksResponse responseDeskList = new DesksResponse();
            List<DeskCustom> list = new List<DeskCustom>();

            List<Desk> desks = _deskRepository.GetOfficeDesks(id, take: take, skip: skip);

            foreach (Desk desk in desks)
            {
                List<Reservation> deskReservations = _reservationRepository.GetDeskReservations(desk.Id, includeEmployee: true).Where(x => x.StartDate > DateTime.Today).ToList();

                deskReservations.ForEach(x =>
                {
                    if (x.Employee != null)
                    {
                        x.Employee.Password = null;
                    }
                });

                DeskCustom custom = new DeskCustom()
                {
                    Id = desk.Id,
                    IndexForOffice = desk.IndexForOffice,
                    Reservations = deskReservations
                };

                Categories? findCategories = _categoriesRepository.GetDeskCategories(desk.Id);
                custom.Categories = findCategories;
                list.Add(custom);
            }

            list.ForEach(x => x.Reservations?.ForEach(y => y.Employee?.Reservations?.Clear()));

            responseDeskList.sucess = true;
            responseDeskList.DeskList = list;

            return responseDeskList;
        }

        public DeleteResponse DeleteEntity(DeleteRequest request)
        {
            DeleteResponse deleteResponse = new DeleteResponse();

            if (request.TypeOfEntity == "D")
            {
                Desk desk = _deskRepository.Get(request.IdOfEntity);

                if (desk == null)
                {
                    deleteResponse.Success = false;
                    return deleteResponse;
                }

                _deskRepository.SoftDelete(desk);

                deleteResponse.Success = true;
            }
            else
            {
                ConferenceRoom conferenceRoom = _conferenceRoomRepository.Get(request.IdOfEntity, includeReservation: true);

                if (conferenceRoom == null)
                {
                    deleteResponse.Success = false;
                    return deleteResponse;
                }

                using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (conferenceRoom.Reservation != null)
                    {
                        _reservationRepository.Delete(conferenceRoom.Reservation);
                    }

                    _conferenceRoomRepository.Delete(conferenceRoom);

                    transaction.Complete();
                }

                deleteResponse.Success = true;
            }

            return deleteResponse;
        }

        public ConferenceRoomsResponse ListAllConferenceRooms(int id, int? take = null, int? skip = null)
        {
            ConferenceRoomsResponse responseConferenceRoom = new ConferenceRoomsResponse();

            responseConferenceRoom.ConferenceRoomsList = _conferenceRoomRepository.GetOfficeConferenceRooms(id, take: take, skip: skip);

            List<ConferenceRoom> roomsToUpdate = new List<ConferenceRoom>();
            foreach (ConferenceRoom conferenceRoom in responseConferenceRoom.ConferenceRoomsList)
            {
                if (conferenceRoom.ReservationId.HasValue)
                {
                    Reservation reservation = _reservationRepository.Get(conferenceRoom.ReservationId.Value);
                    if (DateTime.Compare(reservation.StartDate, DateTime.Now) < 0 && DateTime.Compare(reservation.EndDate, DateTime.Now) < 0)
                    {
                        conferenceRoom.ReservationId = null;
                        roomsToUpdate.Add(conferenceRoom);
                    }
                }
            }
            _conferenceRoomRepository.BulkUpdate(roomsToUpdate);

            responseConferenceRoom.Sucess = true;

            return responseConferenceRoom;
        }

        public EntitiesResponse UpdateDesks(UpdateRequest request)
        {
            EntitiesResponse entitiesResponse = new EntitiesResponse();

            using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (DeskToUpdate deskToUpdate in request.ListOfDesksToUpdate)
                {
                    Categories existingDeskCategories = _categoriesRepository.GetDeskCategories(deskToUpdate.DeskId);
                    if (existingDeskCategories != null)
                    {
                        existingDeskCategories.DoubleMonitor = deskToUpdate.DualMonitor;
                        existingDeskCategories.SingleMonitor = deskToUpdate.SingleMonitor;
                        existingDeskCategories.NearWindow = deskToUpdate.NearWindow;
                        existingDeskCategories.Unavailable = deskToUpdate.Unavailable;

                        _categoriesRepository.Update(existingDeskCategories);
                    }
                    else
                    {
                        Categories categories = new Categories()
                        {
                            DeskId = deskToUpdate.DeskId,
                            DoubleMonitor = deskToUpdate.DualMonitor,
                            SingleMonitor = deskToUpdate.SingleMonitor,
                            NearWindow = deskToUpdate.NearWindow,
                            Unavailable = deskToUpdate.Unavailable
                        };

                        Desk desk = _deskRepository.Get(deskToUpdate.DeskId);

                        if (desk == null)
                        {
                            continue;
                        }

                        _categoriesRepository.Insert(categories);
                        desk.CategorieId = categories.Id;

                        _deskRepository.Update(desk);
                    }
                }

                transaction.Complete();
            }

            entitiesResponse.Success = true;

            return entitiesResponse;
        }
    }
}
