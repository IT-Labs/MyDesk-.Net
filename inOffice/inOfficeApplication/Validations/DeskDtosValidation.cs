using FluentValidation;
using inOfficeApplication.Data.DTO;

namespace inOfficeApplication.Validations
{
    public class DeskDtosValidation : AbstractValidator<List<DeskDto>>
    {
        public DeskDtosValidation()
        {
            RuleForEach(x => x).SetValidator(new DeskDtoValidation());
        }
    }

    public class DeskDtoValidation : AbstractValidator<DeskDto>
    {
        public DeskDtoValidation()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Desk needs to have an ID.");
            RuleFor(x => x.Category).NotNull().WithMessage("Desk needs to have a category.");
        }
    }
}
