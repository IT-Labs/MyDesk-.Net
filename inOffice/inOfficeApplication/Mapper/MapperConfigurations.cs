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
                    .ForMember(x => x.OfficeId, opt => opt.MapFrom(y => y.OfficeId))
                    .ForMember(x => x.Reservation, opt => opt.MapFrom(y => y.Reservations.FirstOrDefault()));

                config.CreateMap<Desk, DeskDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.Categories, opt => opt.MapFrom(y => y.Categorie));

                config.CreateMap<Category, CategoryDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.DoubleMonitor, opt => opt.MapFrom(y => y.DoubleMonitor))
                    .ForMember(x => x.NearWindow, opt => opt.MapFrom(y => y.NearWindow))
                    .ForMember(x => x.SingleMonitor, opt => opt.MapFrom(y => y.SingleMonitor))
                    .ForMember(x => x.Unavailable, opt => opt.MapFrom(y => y.Unavailable))
                    .ForMember(x => x.DeskId, opt => opt.MapFrom(y => GetFirstDeskId(y.Desks)));

                config.CreateMap<Employee, EmployeeDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.FirstName, opt => opt.MapFrom(y => y.FirstName))
                    .ForMember(x => x.LastName, opt => opt.MapFrom(y => y.LastName))
                    .ForMember(x => x.Email, opt => opt.MapFrom(y => y.Email))
                    .ForMember(x => x.JobTitle, opt => opt.MapFrom(y => y.JobTitle));

                config.CreateMap<Review, ReviewDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.Reviews, opt => opt.MapFrom(y => y.Reviews))
                    .ForMember(x => x.ReviewOutput, opt => opt.MapFrom(y => y.ReviewOutput))
                    .ForMember(x => x.ReservationId, opt => opt.MapFrom(y => y.ReservationId));

                config.CreateMap<Reservation, ReservationDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.DeskId, opt => opt.MapFrom(y => y.DeskId))
                    .ForMember(x => x.ConfId, opt => opt.MapFrom(y => y.ConferenceRoomId))
                    .ForMember(x => x.StartDate, opt => opt.MapFrom(y => y.StartDate))
                    .ForMember(x => x.EndDate, opt => opt.MapFrom(y => y.EndDate))
                    .ForMember(x => x.ReviewId, opt => opt.MapFrom(y => GetFirstReviewId(y.Reviews)))
                    .ForMember(x => x.EmployeeId, opt => opt.MapFrom(y => y.EmployeeId))
                    .ForMember(x => x.Employee, opt => opt.MapFrom(y => y.Employee));
            });

            return configuration.CreateMapper();
        }

        private static int? GetFirstDeskId(ICollection<Desk> desks)
        {
            if (desks != null && desks.Count > 0)
            {
                return desks.First().Id;
            }
            else
            {
                return null;
            }
        }

        private static int? GetFirstReviewId(ICollection<Review> reviews)
        {
            if (reviews != null && reviews.Count > 0)
            {
                return reviews.First().Id;
            }
            else
            {
                return null;
            }
        }
    }
}
