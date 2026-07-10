using FluentValidation;
using SlotMachine.DTO.Requests;

namespace SlotMachine.API.Validators;

public class UpdateBalanceRequestValidator : AbstractValidator<UpdateBalanceRequest>
{
    public UpdateBalanceRequestValidator()
    {
           RuleFor(q=>q.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0");
            RuleFor(q=>q.PlayerId).NotEmpty().WithMessage("PlayerId is required");
    }
}
