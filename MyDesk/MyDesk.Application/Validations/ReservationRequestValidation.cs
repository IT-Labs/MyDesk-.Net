using FluentValidation;
using MyDesk.Core.Requests;

namespace MyDesk.Application.Validations
{
    public class ReservationRequestValidation : AbstractValidator<ReservationRequest>
    {
        public ReservationRequestValidation()
        {
            RuleFor(x => x.DeskId).GreaterThan(0).WithMessage("Desk ID must be greater than zero.");
            RuleFor(x => x.EmployeeEmail).NotEmpty().WithMessage("Employee email must have a value.");
            RuleFor(x => x.StartDate).Custom((startDate, context) =>
            {
                DateTime date = DateTime.ParseExact(startDate, "dd-MM-yyyy", null);
                if (DateTime.Compare(date, DateTime.Now.Date) < 0)
                {
                    context.AddFailure("Reservation's start date must be in the future.");
                }
            });
            RuleFor(x => x.EndDate).Custom((endDate, context) =>
            {
                DateTime date = DateTime.ParseExact(endDate, "dd-MM-yyyy", null);
                if (DateTime.Compare(date, DateTime.Now.Date) < 0)
                {
                    context.AddFailure("Reservation's end date must be in the future.");
                }
            });
        }
    }
}
