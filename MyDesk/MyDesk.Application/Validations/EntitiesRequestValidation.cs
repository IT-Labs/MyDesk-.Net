using FluentValidation;
using MyDesk.Core.Requests;

namespace MyDesk.Application.Validations
{
    public class EntitiesRequestValidation : AbstractValidator<EntitiesRequest>
    {
        public EntitiesRequestValidation()
        {
            RuleFor(x => x.NumberOfDesks).GreaterThan(0).LessThanOrEqualTo(500).WithMessage("Maximum number of desks to be created is 500.");
        }
    }
}
