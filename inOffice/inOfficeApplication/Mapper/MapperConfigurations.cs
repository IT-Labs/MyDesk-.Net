using AutoMapper;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;

namespace inOfficeApplication.Mapper
{
    public class MapperConfigurations
    {
        public static IMapper CreateMapper()
        {
            MapperConfiguration configuration = new MapperConfiguration(config =>
            {
                config.CreateMap<Office, OfficeDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.Name, opt => opt.MapFrom(y => y.Name))
                    .ForMember(x => x.OfficeImage, opt => opt.MapFrom(y => y.OfficeImage));

                config.CreateMap<ConferenceRoom, ConferenceRoomDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.Capacity, opt => opt.MapFrom(y => y.Capacity))
                    .ForMember(x => x.IndexForOffice, opt => opt.MapFrom(y => y.IndexForOffice))
                    .ForMember(x => x.Office, opt => opt.MapFrom(y => y.Office))
                    .ForMember(x => x.Reservations, opt => opt.MapFrom(y => y.Reservations));

                config.CreateMap<Desk, DeskDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.IndexForOffice, opt => opt.MapFrom(y => y.IndexForOffice))
                    .ForMember(x => x.Category, opt => opt.MapFrom(y => y.Categorie))
                    .ForMember(x => x.Reservations, opt => opt.MapFrom(y => y.Reservations))
                    .ForMember(x => x.Office, opt => opt.MapFrom(y => y.Office));

                config.CreateMap<Category, CategoryDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.DoubleMonitor, opt => opt.MapFrom(y => y.DoubleMonitor))
                    .ForMember(x => x.NearWindow, opt => opt.MapFrom(y => y.NearWindow))
                    .ForMember(x => x.SingleMonitor, opt => opt.MapFrom(y => y.SingleMonitor))
                    .ForMember(x => x.Unavailable, opt => opt.MapFrom(y => y.Unavailable))
                    .ForMember(x => x.Desks, opt => opt.Ignore());

                config.CreateMap<Employee, EmployeeDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.FirstName, opt => opt.MapFrom(y => y.FirstName))
                    .ForMember(x => x.Surname, opt => opt.MapFrom(y => y.LastName))
                    .ForMember(x => x.Email, opt => opt.MapFrom(y => y.Email))
                    .ForMember(x => x.JobTitle, opt => opt.MapFrom(y => y.JobTitle))
                    .ForMember(x => x.Password, opt => opt.Ignore())
                    .ReverseMap();

                config.CreateMap<Review, ReviewDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.Reviews, opt => opt.MapFrom(y => y.Reviews))
                    .ForMember(x => x.ReviewOutput, opt => opt.MapFrom(y => y.ReviewOutput))
                    .ForMember(x => x.Reservation, opt => opt.MapFrom(y => y.Reservation));

                config.CreateMap<Reservation, ReservationDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.StartDate, opt => opt.MapFrom(y => y.StartDate))
                    .ForMember(x => x.EndDate, opt => opt.MapFrom(y => y.EndDate))
                    .ForMember(x => x.Reviews, opt => opt.MapFrom(y => y.Reviews))
                    .ForMember(x => x.Employee, opt => opt.MapFrom(y => y.Employee))
                    .ForMember(x => x.Desk, opt => opt.MapFrom(y => y.Desk))
                    .ForMember(x => x.ConferenceRoom, opt => opt.MapFrom(y => y.ConferenceRoom))
                    .BeforeMap((entity, dto) =>
                    {
                        if (entity.Desk != null && entity.Desk.Reservations != null)
                        {
                            entity.Desk.Reservations.Clear();
                        }
                    });
            });

            return configuration.CreateMapper();
        }
    }
}
