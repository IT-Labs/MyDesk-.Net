using FluentValidation;
using MyDesk.Core.DTO;

namespace MyDesk.Application.Validations
{
    public class ReviewDtoValidation : AbstractValidator<ReviewDto>
    {
        public ReviewDtoValidation()
        {
            RuleFor(x => x.Reviews).NotEmpty().WithMessage("Review must contain text message.");
            RuleFor(x => x.Reservation).NotEmpty().WithMessage("Review must be related to a reservation.");
            When(x => x.Reservation != null, () =>
            {
                RuleFor(x => x.Reservation.Id).GreaterThan(0).WithMessage("Review must be related to a reservation.");
            });
        }
    }
}
