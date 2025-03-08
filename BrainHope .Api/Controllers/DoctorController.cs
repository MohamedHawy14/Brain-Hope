using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrainHope_.Api.Controllers
{
    [Route("doctor/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpPost("SetCalendlyLink")]
        public async Task<IActionResult> SetCalendlyLink([FromForm] SetCalendlyLinkDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.CalendlyLink))
            {
                return BadRequest("UserId and CalendlyLink are required.");
            }

            
            var doctor = _unitOfWork.Repository<Doctor>()
                                      .Get(d => d.UserId == dto.UserId, includeProperties: "AppUser");
            if (doctor == null)
            {
                return NotFound("Doctor not found for the given user id.");
            }

            doctor.CalendlyLink = dto.CalendlyLink;
            _unitOfWork.Repository<Doctor>().Update(doctor);
            await _unitOfWork.Complete();

            return Ok(new { message = "Calendly link updated successfully.", calendlyLink = doctor.CalendlyLink });
        }
    }
}
