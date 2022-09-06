using inOffice.Repository.Implementation;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Entities;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Repository
{
    public class EmployeeRepositoryTests : TestBase
    {
        private IEmployeeRepository _employeeRepository;

        [SetUp]
        public void Setup()
        {
            _employeeRepository = new EmployeeRepository(base._dbContext);
        }

        [TearDown]
        public void CleanUp()
        {
            base.CleanDbContext();
        }

        [Test]
        [Order(1)]
        public void Get_Success()
        {
            // Arrange
            int id = 1;

            // Act
            Employee employee = _employeeRepository.Get(id);

            // Assert
            Assert.NotNull(employee, "Employee should exist.");
            Assert.IsTrue(employee.Id == id);
        }

        [Test]
        [Order(2)]
        public void Get_Failure()
        {
            // Arrange + Act
            Employee employee = _employeeRepository.Get(3);

            // Assert
            Assert.IsNull(employee, "Employee shouldn't exist.");
        }

        [Test]
        [Order(3)]
        public void GetByEmail_Success()
        {
            // Arrange
            string email = "john.doe@it-labs.com";

            // Act
            Employee employee = _employeeRepository.GetByEmail(email);

            // Assert
            Assert.NotNull(employee, "Employee should exist.");
            Assert.IsTrue(employee.Email == email);
        }

        [Test]
        [Order(4)]
        public void GetByEmail_Failure()
        {
            // Arrange
            string email = "test@email.com";

            // Act
            Employee employee = _employeeRepository.GetByEmail(email);

            // Assert
            Assert.IsNull(employee, "Employee shouldn't exist.");
        }

        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(1, 1)]
        [Order(5)]
        public void GetAll_Success(int? take, int? skip)
        {
            // Arrange + Act
            List<Employee> employees = _employeeRepository.GetAll(take: take, skip: skip);

            // Assert
            if (skip.HasValue)
            {
                Assert.IsTrue(employees.Count == take);
            }
            else
            {
                Assert.IsTrue(employees.Count == 3);
            }

            employees.ForEach(x => Assert.IsTrue(x.IsDeleted == false));
        }

        [Test]
        [Order(6)]
        public void Create_Success()
        {
            // Arrange
            Employee employee = new Employee()
            {
                FirstName = "Jon",
                LastName = "Jones",
                Email = "jon.jones@it-labs.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Passvord!23"),
                JobTitle = "Bones",
                IsAdmin = false,
                IsDeleted = false
            };

            // Act
            _employeeRepository.Create(employee);

            // Assert
            Employee createdEmployee = _employeeRepository.Get(employee.Id);

            Assert.NotNull(createdEmployee, "Employee should not be null.");
            Assert.IsTrue(createdEmployee.Id == employee.Id);
        }

        [Test]
        [Order(7)]
        public void Update_Success()
        {
            // Arrange
            int id = 2;
            string firstName = "Jon";
            string lastName = "Jones";

            Employee employee = _employeeRepository.Get(id);
            employee.FirstName = firstName;
            employee.LastName = lastName;

            // Act
            _employeeRepository.Update(employee);

            // Assert
            Employee updatedEmployee = _employeeRepository.Get(id);

            Assert.NotNull(updatedEmployee, "Employee should not be null.");
            Assert.IsTrue(updatedEmployee.Id == employee.Id && updatedEmployee.FirstName == firstName && updatedEmployee.LastName == lastName);
        }
    }
}
