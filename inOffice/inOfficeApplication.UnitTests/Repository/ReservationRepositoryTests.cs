﻿using inOffice.Repository.Implementation;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Entities;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Repository
{
    public class ReservationRepositoryTests : RepositoryBaseTest
    {
        private IReservationRepository _reservationRepository;

        [SetUp]
        public void Setup()
        {
            _reservationRepository = new ReservationRepository(base._dbContext);
        }

        [TearDown]
        public void CleanUp()
        {
            base.CleanDbContext();
        }

        [TestCase(1, null, null, null, null)]
        [TestCase(1, true, null, null, null)]
        [TestCase(1, true, true, null, null)]
        [TestCase(1, true, true, null, true)]
        [TestCase(4, null, null, null, null)]
        [TestCase(4, null, null, true, null)]
        [TestCase(4, null, true, true, null)]
        [TestCase(4, null, true, true, true)]
        [Order(1)]
        public void Get_Success(int id, bool? includeDesk, bool? includeOffice, bool? includeonferenceRoom, bool? includeReviews)
        {
            // Arrange + Act
            Reservation reservation = _reservationRepository.Get(id, includeDesk: includeDesk, includeOffice: includeOffice, 
                includeonferenceRoom: includeonferenceRoom, includeReviews: includeReviews);

            // Assert
            Assert.NotNull(reservation, "Reservation should exist.");

            if (includeReviews == true)
            {
                Assert.IsTrue(reservation.Reviews.Count == 1);
            }
            else
            {
                Assert.IsTrue(reservation.Reviews.Count == 0);
            }

            if (includeDesk == true)
            {
                Assert.NotNull(reservation.Desk);
                if (includeOffice == true)
                {
                    Assert.NotNull(reservation.Desk.Office);
                }
                else
                {
                    Assert.IsNull(reservation.Desk.Office);
                }
            }
            else
            {
                Assert.IsNull(reservation.Desk);
            }

            if (includeonferenceRoom == true)
            {
                Assert.NotNull(reservation.ConferenceRoom);
                if (includeOffice == true)
                {
                    Assert.NotNull(reservation.ConferenceRoom.Office);
                }
                else
                {
                    Assert.IsNull(reservation.ConferenceRoom.Office);
                }
            }
            else
            {
                Assert.IsNull(reservation.ConferenceRoom);
            }
        }

        [Test]
        [Order(2)]
        public void Get_Failure()
        {
            // Arrange + Act
            Reservation reservation = _reservationRepository.Get(2);

            // Assert
            Assert.IsNull(reservation, "Reservation shouldn't exist.");
        }

        [TestCase(null, null, null, null, null)]
        [TestCase(true, null, null, null, null)]
        [TestCase(true, true, null, null, null)]
        [TestCase(true, true, true, null, null)]
        [TestCase(true, true, true, 1, 0)]
        [Order(3)]
        public void GetAll_Success(bool? includeEmployee, bool? includeDesk, bool? includeOffice, int? take, int? skip)
        {
            // Arrange + Act
            Tuple<int?, List<Reservation>> reservations = _reservationRepository.GetAll(includeEmployee: includeEmployee, 
                includeDesk: includeDesk, includeOffice: includeOffice, take: take, skip: skip);

            // Assert
            if (take.HasValue && skip.HasValue)
            {
                Assert.IsTrue(reservations.Item1 == 3 && reservations.Item2.Count == take);
            }
            else
            {
                Assert.IsTrue(reservations.Item1 == null && reservations.Item2.Count == 3);
            }

            foreach (Reservation reservation in reservations.Item2)
            {
                if (includeEmployee == true)
                {
                    Assert.NotNull(reservation.Employee);
                }
                else
                {
                    Assert.IsNull(reservation.Employee);
                }

                if (includeDesk == true && reservation.DeskId.HasValue)
                {
                    Assert.NotNull(reservation.Desk);
                    if (includeOffice == true)
                    {
                        Assert.NotNull(reservation.Desk.Office);
                    }
                    else
                    {
                        Assert.IsNull(reservation.Desk.Office);
                    }
                }
                else
                {
                    Assert.IsNull(reservation.Desk);
                }
            }
        }

        [TestCase(null, null, null, null)]
        [TestCase(true, null, null, null)]
        [TestCase(true, null, true, null)]
        [TestCase(null, true, null, null)]
        [TestCase(null, true, true, null)]
        [TestCase(null, true, true, true)]
        [Order(4)]
        public void GetEmployeeReservations_Success(bool? includeDesk, bool? includeConferenceRoom, bool? includeOffice, bool? includeReviews)
        {
            // Arrange + Act
            List<Reservation> reservations = _reservationRepository.GetEmployeeReservations(2, includeDesk: includeDesk, 
                includeConferenceRoom: includeConferenceRoom, includeOffice: includeOffice, includeReviews: includeReviews);

            // Assert
            Assert.IsTrue(reservations.Count == 3);

            foreach (Reservation reservation in reservations)
            {
                // Reservation with Id = 3 doesn't have reviews
                if (includeReviews == true && reservation.Id != 3)
                {
                    Assert.IsTrue(reservation.Reviews.Count == 1);
                }
                else
                {
                    Assert.IsTrue(reservation.Reviews.Count == 0);
                }

                if (includeDesk == true && reservation.DeskId.HasValue)
                {
                    Assert.NotNull(reservation.Desk);
                    if (includeOffice == true)
                    {
                        Assert.NotNull(reservation.Desk.Office);
                    }
                    else
                    {
                        Assert.IsNull(reservation.Desk.Office);
                    }
                }
                else
                {
                    Assert.IsNull(reservation.Desk);
                }

                if (includeConferenceRoom == true && reservation.ConferenceRoomId.HasValue)
                {
                    Assert.NotNull(reservation.ConferenceRoom);
                    if (includeOffice == true)
                    {
                        Assert.NotNull(reservation.ConferenceRoom.Office);
                    }
                    else
                    {
                        Assert.IsNull(reservation.ConferenceRoom.Office);
                    }
                }
                else
                {
                    Assert.IsNull(reservation.ConferenceRoom);
                }
            }
        }

        [TestCase(null, null)]
        [TestCase(true, null)]
        [TestCase(null, true)]
        [TestCase(true, true)]
        [Order(5)]
        public void GetDeskReservations_Success(bool? includeReview, bool? includeEmployee)
        {
            // Arrange + Act
            List<Reservation> reservations = _reservationRepository.GetDeskReservations(1, includeReview: includeReview, includeEmployee: includeEmployee);

            // Assert
            Assert.IsTrue(reservations.Count == 2);

            foreach (Reservation reservation in reservations)
            {
                // Reservation with Id = 1 has reviews
                if (includeReview == true && reservation.Id == 1)
                {
                    Assert.IsTrue(reservation.Reviews.Count == 1);
                }
                else
                {
                    Assert.IsTrue(reservation.Reviews.Count == 0);
                }

                if (includeEmployee == true)
                {
                    Assert.NotNull(reservation.Employee);
                }
                else
                {
                    Assert.IsNull(reservation.Employee);
                }
            }
        }

        [Test]
        [Order(6)]
        public void Insert_Success()
        {
            // Arrange
            Reservation reservation = new Reservation()
            {
                DeskId = 3,
                EmployeeId = 2,
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                IsDeleted = false
            };

            // Act
            _reservationRepository.Insert(reservation);

            // Assert
            Reservation createdReservation = _reservationRepository.Get(reservation.Id);

            Assert.NotNull(createdReservation, "Reservation should not be null.");
            Assert.IsTrue(createdReservation.Id == reservation.Id);
        }

        [Test]
        [Order(7)]
        public void SoftDelete_Success()
        {
            // Arrange
            int id = 4;
            Reservation reservation = _reservationRepository.Get(id);

            // Act
            _reservationRepository.SoftDelete(reservation);

            // Assert
            Reservation deletedReservation = _reservationRepository.Get(id);
            Assert.IsNull(deletedReservation, "Reservation should be null.");
        }
    }
}