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
public class PlayerController(
    IPlayerService _playerService,
    IValidator<UpdateBalanceRequest> _validator) : ControllerBase
{
    /// <summary>Add funds to a player's balance (creates the player if new).</summary>
    [HttpPost("balance")]
    public async Task<IActionResult> UpdateBalance([FromBody] UpdateBalanceRequest request)
    {
        var validateResult = await _validator.ValidateAsync(request);
        if (!validateResult.IsValid)
        {
            var validationErrors = validateResult.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(
                ApiResponse<string>.Fail(ErrorMessages.ValidationFailMessage, validationErrors));
        }

        var result = await _playerService.UpdateBalanceAsync(request);
        return Ok(ApiResponse<UpdateBalanceResponse>.Ok(result));
    }
}