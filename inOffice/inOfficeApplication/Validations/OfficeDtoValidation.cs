﻿using FluentValidation;
using inOfficeApplication.Data.DTO;

namespace inOfficeApplication.Validations
{
    public class OfficeDtoValidation : AbstractValidator<OfficeDto>
    {
        public OfficeDtoValidation()
        {
            When(x => x.Id.HasValue, () =>
            {
                RuleFor(x => x.Id).GreaterThan(-1).WithMessage("Office ID cannot be negative number.");
            });
            RuleFor(x => x.Name).NotEmpty().WithMessage("Office name cannot be empty.");
        }
    }
}
