using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utilites;

namespace BrainHope_.Api.Controllers
{
    [Route("patient/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_Patient)]
    public class PatientController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpPost("SetRateToDoctor")]
        public async Task<IActionResult> SetRateToDoctor([FromBody] SetDoctorRatingDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid request.");
            }

            // Validate rating is between 1 and 5 (this is also handled by the DTO via data annotations).
            if (dto.Rate < 1 || dto.Rate > 5)
            {
                return BadRequest("Rate must be between 1 and 5.");
            }

            // Retrieve the doctor by matching the doctor’s UserId.
            var doctor = _unitOfWork.Repository<Doctor>()
                            .Get(d => d.UserId == dto.DoctorUserId, includeProperties: "AppUser");
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            // Update the doctor's rate.
            // NOTE: In a real-world application, you might want to average ratings from multiple patients.
            doctor.Rate = dto.Rate;

            _unitOfWork.Repository<Doctor>().Update(doctor);
            await _unitOfWork.Complete();

            return Ok(new { message = "Doctor rating updated successfully.", doctorRate = doctor.Rate });
        }

    }
}
