using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlotMachine.Common.Constants;
using SlotMachine.DTO.Common;
using SlotMachine.DTO.Requests;
using SlotMachine.DTO.Responses;
using SlotMachine.Service.Interfaces;

namespace SlotMachine.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SlotController(
    ISlotService _slotService,
    IValidator<SpinRequest> _validator) : ControllerBase
{
    [HttpPost("spin")]
    public async Task<IActionResult> Spin([FromBody] SpinRequest request)
    {
        var validateResult = await _validator.ValidateAsync(request);
        if (!validateResult.IsValid)
        {
            var errors = validateResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<string>.Fail(ErrorMessages.ValidationFailMessage, errors));
        }

        var result = await _slotService.SpinAsync(request);
        return Ok(ApiResponse<SpinResponse>.Ok(result));
    }
}