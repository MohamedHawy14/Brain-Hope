using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Models.AiModel;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.AiModel;
using BrainHope.Services.InterFaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utilites;

[ApiController]
[Route("api/[controller]")]
public class BrainScanController : ControllerBase
{
    private readonly IBrainScanService _brainScanService;

    public BrainScanController(IBrainScanService brainScanService)
    {
        _brainScanService = brainScanService;
    }

    [HttpPost("scan/{userId}")]
    public async Task<IActionResult> AnalyzeBrainScan([FromForm] BrainScanImageDTO dto,  string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is required.");

        try
        {
            var result = await _brainScanService.AnalyzeBrainScanAsync(dto.Image, userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }



}