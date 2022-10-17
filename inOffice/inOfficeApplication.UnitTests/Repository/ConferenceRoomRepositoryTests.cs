using inOffice.Repository;
using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data.Entities;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Repository
{
    public class ConferenceRoomRepositoryTests : RepositoryBaseTest
    {
        private IConferenceRoomRepository _conferenceRoomRepository;

        [SetUp]
        public void Setup()
        {
            _conferenceRoomRepository = new ConferenceRoomRepository(base._dbContext);
        }

        [TearDown]
        public void CleanUp()
        {
            base.CleanDbContext();
        }

        [TestCase(null, null)]
        [TestCase(true, null)]
        [TestCase(null, true)]
        [TestCase(true, true)]
        [Order(1)]
        public void Get_Success(bool? includeReservations, bool? includeReviews)
        {
            // Arrange
            int id = 1;

            // Act
            ConferenceRoom conferenceRoom = _conferenceRoomRepository.Get(id, includeReservations: includeReservations, includeReviews: includeReviews);

            //Assert
            Assert.NotNull(conferenceRoom, "ConferenceRoom should exist.");
            Assert.IsTrue(conferenceRoom.Id == id);

            if (includeReservations == true)
            {
                Assert.IsTrue(conferenceRoom.Reservations.Count == 1);
                if (includeReviews == true)
                {
                    Assert.IsTrue(conferenceRoom.Reservations.Single().Reviews.Count == 1);
                    Assert.IsTrue(conferenceRoom.Reservations.Single().Reviews.Single().Reviews == "Test review 3");
                }
            }
            else
            {
                Assert.IsTrue(conferenceRoom.Reservations.Count == 0);
            }
        }

        [Test]
        [Order(2)]
        public void Get_Failure()
        {
            // Arrange + Act
            ConferenceRoom conferenceRoom = _conferenceRoomRepository.Get(2);

            // Assert
            Assert.IsNull(conferenceRoom, "ConferenceRoom shouldn't exist.");
        }

        [TestCase(null, null, null)]
        [TestCase(true, null, null)]
        [TestCase(true, 1, 0)]
        [Order(3)]
        public void GetOfficeConferenceRooms(bool? includeReservations, int? take, int? skip)
        {
            // Arrange + Act
            List<ConferenceRoom> conferenceRooms = _conferenceRoomRepository.GetOfficeConferenceRooms(1, 
                includeReservations: includeReservations, take: take, skip: skip);

            // Assert
            if (take.HasValue && skip.HasValue)
            {
                Assert.IsTrue(conferenceRooms.Count == take);
            }
            else
            {
                Assert.IsTrue(conferenceRooms.Count == 2);
            }

            if (includeReservations == true)
            {
                ConferenceRoom conferenceRoom = conferenceRooms.Single(x => x.Id == 1);
                Assert.NotNull(conferenceRoom);
                Assert.IsTrue(conferenceRoom.Reservations.Count == 1);
            }
        }

        [Test]
        [Order(4)]
        public void SoftDelete_Success()
        {
            // Arrange
            int id = 3;
            ConferenceRoom conferenceRoom = _conferenceRoomRepository.Get(id);

            // Act
            _conferenceRoomRepository.SoftDelete(conferenceRoom);

            // Assert
            ConferenceRoom deletedConferenceRoom = _conferenceRoomRepository.Get(id);
            Assert.IsNull(deletedConferenceRoom, "ConferenceRoom shouldn't exist.");
        }
    }
}
