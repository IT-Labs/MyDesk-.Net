using FluentValidation;
using inOffice.BusinessLogicLayer.Requests;

namespace inOfficeApplication.Validations
{
    public class DeleteRequestValidation : AbstractValidator<DeleteRequest>
    {
        public DeleteRequestValidation()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("ID of the entity needs to be grater than zero.");
            RuleFor(x => x.Type).NotEmpty().GreaterThan(0).LessThanOrEqualTo(2).WithMessage("Type of entity must be defined.");
        }
    }
}
