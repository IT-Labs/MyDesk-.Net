using FluentValidation;
using MyDesk.Core.DTO;

namespace MyDesk.Application.Validations
{
    public class EmployeeDtoValidation : AbstractValidator<EmployeeDto>
    {
        public EmployeeDtoValidation()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Employee must have an email address.");
            When(x => !string.IsNullOrEmpty(x.Email), () =>
            {
                RuleFor(x => x.Email).Custom((email, context) =>
                {
                    email = email.Trim();
                    if (email.Length < 3 || email.Length > 254)
                    {
                        context.AddFailure("Email length should be between 3 and 254.");
                    }

                    if (!email.Contains("@it-labs.com") && !email.Contains("@gmail.com"))
                    {
                        context.AddFailure("Invalid email address.");
                    }
                });
            });
        }
    }
}
