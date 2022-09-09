using inOfficeApplication.Data;
using inOfficeApplication.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests
{
    public class RepositoryBaseTest
    {
        protected ApplicationDbContext _dbContext;

        [OneTimeSetUp]
        public void Setup()
        {
            DbContextOptions<ApplicationDbContext> options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("testDB")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureCreated();

            SeedDbData();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            _dbContext.Database.EnsureDeleted();
        }

        public void CleanDbContext()
        {
            foreach (EntityEntry entityEntry in _dbContext.ChangeTracker.Entries())
            {
                entityEntry.State = EntityState.Detached;
            }
        }

        #region Private methods
        private void SeedDbData()
        {
            _dbContext.Categories.AddRange(GetCategories());
            _dbContext.Employees.AddRange(GetEmployees());
            _dbContext.Offices.AddRange(GetOffices());
            _dbContext.Desks.AddRange(GetDesks());
            _dbContext.ConferenceRooms.AddRange(GetConferenceRooms());
            _dbContext.Reservations.AddRange(GetReservations());
            _dbContext.Reviews.AddRange(GetReviews());

            _dbContext.SaveChanges();

            CleanDbContext();
        }

        private List<Category> GetCategories()
        {
            return new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    SingleMonitor = true,
                    NearWindow = true,
                    DoubleMonitor = false,
                    Unavailable = false,
                    IsDeleted = true
                },
                new Category()
                {
                    Id = 2,
                    SingleMonitor = true,
                    NearWindow = true,
                    DoubleMonitor = false,
                    Unavailable = false,
                    IsDeleted = false
                },
                new Category()
                {
                    Id = 3,
                    SingleMonitor = false,
                    NearWindow = true,
                    DoubleMonitor = true,
                    Unavailable = false,
                    IsDeleted = false
                }
            };
        }

        private List<Employee> GetEmployees()
        {
            return new List<Employee>()
            {
                new Employee()
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@it-labs.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Passvord!23"),
                    JobTitle = "Employee",
                    IsAdmin = false,
                    IsDeleted = false
                },
                new Employee()
                {
                    Id = 3,
                    FirstName = "John",
                    LastName = "Doe 2",
                    Email = "john.doe2@it-labs.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Passvord!23"),
                    JobTitle = "Employee",
                    IsAdmin = false,
                    IsDeleted = true
                },
                new Employee()
                {
                    Id = 4,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane.doe@it-labs.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Passvord!23"),
                    JobTitle = "Employee",
                    IsAdmin = false,
                    IsDeleted = false
                }
            };
        }

        private List<Office> GetOffices()
        {
            return new List<Office>()
            {
                new Office()
                {
                    Id = 1,
                    Name = "Main office",
                    OfficeImage = string.Empty,
                    IsDeleted = false
                },
                new Office()
                {
                    Id = 2,
                    Name = "Deleted office",
                    OfficeImage = string.Empty,
                    IsDeleted = true
                },
                new Office()
                {
                    Id = 3,
                    Name = "Side office",
                    OfficeImage = string.Empty,
                    IsDeleted = false
                }
            };
        }

        private List<Desk> GetDesks()
        {
            return new List<Desk>()
            {
                new Desk()
                {
                    Id = 1,
                    OfficeId = 1,
                    CategorieId = 1,
                    IndexForOffice = 1,
                    Categories = "regular",
                    IsDeleted = false
                },
                new Desk()
                {
                    Id = 2,
                    OfficeId = 1,
                    CategorieId = 1,
                    IndexForOffice = 2,
                    Categories = "regular",
                    IsDeleted = true
                },new Desk()
                {
                    Id = 3,
                    OfficeId = 1,
                    CategorieId = 1,
                    IndexForOffice = 3,
                    Categories = "regular",
                    IsDeleted = false
                }
            };
        }

        private List<ConferenceRoom> GetConferenceRooms()
        {
            return new List<ConferenceRoom>()
            {
                new ConferenceRoom()
                {
                    Id = 1,
                    OfficeId = 1,
                    IndexForOffice = 1,
                    Capacity = 10,
                    IsDeleted = false
                },
                new ConferenceRoom()
                {
                    Id = 2,
                    OfficeId = 1,
                    IndexForOffice = 2,
                    Capacity = 20,
                    IsDeleted = true
                },
                new ConferenceRoom()
                {
                    Id = 3,
                    OfficeId = 1,
                    IndexForOffice = 3,
                    Capacity = 30,
                    IsDeleted = false
                }
            };
        }

        private List<Reservation> GetReservations()
        {
            return new List<Reservation>()
            {
                new Reservation()
                {
                    Id = 1,
                    StartDate = DateTime.Now.AddDays(-2),
                    EndDate = DateTime.Now.AddDays(-1),
                    DeskId = 1,
                    EmployeeId = 2,
                    IsDeleted = false
                },
                new Reservation()
                {
                    Id = 2,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    DeskId = 1,
                    EmployeeId = 2,
                    IsDeleted = true
                },
                new Reservation()
                {
                    Id = 3,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    DeskId = 1,
                    EmployeeId = 2,
                    IsDeleted = false
                },
                new Reservation()
                {
                    Id = 4,
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(3),
                    ConferenceRoomId = 1,
                    EmployeeId = 2,
                    IsDeleted = false
                }
            };
        }

        private List<Review> GetReviews()
        {
            return new List<Review>()
            {
                new Review()
                {
                    Id = 1,
                    Reviews = "Test review",
                    ReviewOutput = "Neutral",
                    ReservationId = 1,
                    IsDeleted = true
                },
                new Review()
                {
                    Id = 2,
                    Reviews = "Test review 2",
                    ReviewOutput = "Neutral",
                    ReservationId = 1,
                    IsDeleted = false
                },
                new Review()
                {
                    Id = 3,
                    Reviews = "Test review 3",
                    ReviewOutput = "Neutral",
                    ReservationId = 4,
                    IsDeleted = false
                }
            };
        }
        #endregion
    }
}
