using AutoMapper;
using inOffice.BusinessLogicLayer;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Service
{
    public class EmployeeServiceTests
    {
        private IEmployeeService _employeeService;
        private IEmployeeRepository _employeeRepository;
        private IMapper _mapper;

        [OneTimeSetUp]
        public void Setup()
        {
            _employeeRepository = Substitute.For<IEmployeeRepository>();
            _mapper = Substitute.For<IMapper>();

            _employeeService = new EmployeeService(_employeeRepository, _mapper);
        }

        [Test]
        [Order(1)]
        public void Create_Success()
        {
            // Arrange
            EmployeeDto employeeDto = new EmployeeDto()
            {
                Email = "test@test.com",
                FirstName = "John",
                Surname = "Doe",
                JobTitle = "Admin"
            };

            Employee employee = new Employee()
            {
                Email = "test@test.com",
                FirstName = "John",
                LastName = "Doe",
                IsDeleted = false,
                JobTitle = "Admin"
            };

            _mapper.Map<Employee>(employeeDto).Returns(employee);

            // Act
            _employeeService.Create(employeeDto);

            // Assert
            _employeeRepository.Received(1).Create(employee);
        }

        [Test]
        [Order(2)]
        public void SetEmployeeAsAdmin_Success()
        {
            // Arrange
            int employeeId = 11;
            Employee employee = new Employee()
            {
                Id = employeeId,
                Email = "test@test.com",
                FirstName = "John",
                LastName = "Doe",
                IsDeleted = false,
                JobTitle = "Admin"
            };

            _employeeRepository.Get(employeeId).Returns(employee);

            // Act
            _employeeService.SetEmployeeAsAdmin(employeeId);

            // Assert
            _employeeRepository.Received(1).Update(Arg.Is<Employee>(x => x.Id == employeeId && x.IsAdmin == true));
        }

        [Test]
        [Order(3)]
        public void SetEmployeeAsAdmin_ThrowsNotFoundException()
        {
            // Arrange
            int id = 13;

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _employeeService.SetEmployeeAsAdmin(id));
            Assert.IsTrue(exception.Message == $"Employee with ID:{id} not found.");
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(4)]
        public void GetAll_Success(int? take, int? skip)
        {
            // Arrange
            List<Employee> employees = new List<Employee>()
            {
                new Employee()
                {
                    Id = 1,
                    Email = "john.doe@test.com",
                    Password = "pass1",
                    FirstName = "John",
                    LastName = "Doe",
                    IsDeleted = false,
                    JobTitle = "Admin"
                },
                new Employee()
                {
                    Id = 2,
                    Email = "jane.doe@test.com",
                    Password = "pass2",
                    FirstName = "Jane",
                    LastName = "Doe",
                    IsDeleted = false,
                    JobTitle = "Admin"
                },
                new Employee()
                {
                    Id = 3,
                    Email = "jane.doe@test.com",
                    Password = "pass3",
                    FirstName = "Jane",
                    LastName = "Doe",
                    IsDeleted = false,
                    JobTitle = "Admin"
                }
            };

            List<EmployeeDto> employeesDtos = new List<EmployeeDto>()
            {
                new EmployeeDto()
                {
                    Id = 1,
                    Email = "john.doe@test.com",
                    FirstName = "John",
                    Surname = "Doe",
                    JobTitle = "Admin"
                },
                new EmployeeDto()
                {
                    Id = 2,
                    Email = "jane.doe@test.com",
                    FirstName = "Jane",
                    Surname = "Doe",
                    JobTitle = "Admin"
                },
                new EmployeeDto()
                {
                    Id = 3,
                    Email = "jane.doe@test.com",
                    FirstName = "Jane",
                    Surname = "Doe",
                    JobTitle = "Admin"
                }
            };

            _employeeRepository.GetAll(take: take, skip: skip).Returns(employees);
            _mapper.Map<List<EmployeeDto>>(employees).Returns(employeesDtos);

            // Act
            List<EmployeeDto> result = _employeeService.GetAll(take: take, skip: skip);

            // Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.All(x => string.IsNullOrEmpty(x.Password)));
            _employeeRepository.Received(1).GetAll(take, skip);
        }

        [Test]
        [Order(5)]
        public void GetByEmail_Success()
        {
            // Arrange
            string email = "test@it-labs.com";
            Employee employee = new Employee()
            {
                Id = 1,
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                IsDeleted = false,
                JobTitle = "Admin"
            };

            EmployeeDto employeeDto = new EmployeeDto()
            {
                Id = 1,
                Email = email,
                FirstName = "John",
                Surname = "Doe",
                JobTitle = "Admin"
            };

            _employeeRepository.GetByEmail(email).Returns(employee);
            _mapper.Map<EmployeeDto>(employee).Returns(employeeDto);

            // Act
            EmployeeDto result = _employeeService.GetByEmail(email);

            // Assert
            Assert.NotNull(result);
            _employeeRepository.Received(1).GetByEmail(email);
        }

        [Test]
        [Order(6)]
        public void GetByEmail_Failure()
        {
            // Arrange
            string email = "test123@it-labs.com";

            // Act + Assert
            EmployeeDto employeeDto = _employeeService.GetByEmail(email);

            // Assert
            Assert.IsNull(employeeDto);
            _employeeRepository.Received(1).GetByEmail(email);
        }

        [Test]
        [Order(7)]
        public void GetByEmailAndPassword_Success()
        {
            // Arrange
            string email = "test@it-labs.com";
            string password = "test123";

            Employee employee = new Employee()
            {
                Id = 1,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = "John",
                LastName = "Doe",
                IsDeleted = false,
                JobTitle = "Admin"
            };

            EmployeeDto employeeDto = new EmployeeDto()
            {
                Id = 1,
                Email = email,
                FirstName = "John",
                Surname = "Doe",
                JobTitle = "Admin"
            };

            _employeeRepository.GetByEmail(email).Returns(employee);
            _mapper.Map<EmployeeDto>(employee).Returns(employeeDto);

            // Act
            EmployeeDto result = _employeeService.GetByEmailAndPassword(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Email == email);
        }

        [Test]
        [Order(8)]
        public void GetByEmailAndPassword_WrongEmail_ThrowsNotFoundException()
        {
            // Arrange
            string email = "test@it-labs.com";

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _employeeService.GetByEmailAndPassword(email, "pass"));
            Assert.IsTrue(exception.Message == $"Employee with email: {email} not found.");
        }

        [Test]
        [Order(9)]
        public void GetByEmailAndPassword_WrongPassword_ThrowsNotFoundException()
        {
            // Arrange
            string email = "test@it-labs.com";

            Employee employee = new Employee()
            {
                Id = 1,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword("test123"),
                FirstName = "John",
                LastName = "Doe",
                IsDeleted = false,
                JobTitle = "Admin"
            };

            _employeeRepository.GetByEmail(email).Returns(employee);

            // Act + Assert
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _employeeService.GetByEmailAndPassword(email, "test321"));
            Assert.IsTrue(exception.Message == $"Employee with email: {email} not found.");
        }
    }
}
