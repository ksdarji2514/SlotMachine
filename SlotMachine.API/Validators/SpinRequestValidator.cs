using FluentValidation;
using SlotMachine.DTO.Requests;

namespace SlotMachine.API.Validators;

public class SpinRequestValidator : AbstractValidator<SpinRequest>
{
    public SpinRequestValidator()
    {
        RuleFor(q => q.Bet).GreaterThan(0).WithMessage("Bet must be greater than 0");
        RuleFor(q => q.PlayerId).NotEmpty().WithMessage("PlayerId is required");
    }
}