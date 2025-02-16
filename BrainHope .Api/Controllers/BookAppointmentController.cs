using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.BookAppointment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrainHope_.Api.Controllers
{
    [Route("Appointment/[controller]")]
    [ApiController]
    public class BookAppointmentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public BookAppointmentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [HttpGet("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors(string? name = null)
        {
            var query = unitOfWork.Repository<Doctor>()
                .GetAll(includeProperties: "AppUser") 
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(d => d.AppUser.UserName.Contains(name)); // Search by Name
            }

            var doctors = query
                .OrderBy(d => d.Rate)  // Order by Rate in ascending order
                .Select(d => new AllDoctorDTO
                {
                    Name = d.AppUser.UserName,
                    Description = d.Description,
                    Rate = d.Rate,
                    ProfilePhoto = d.AppUser.ProfilePhoto // Keep as byte[]
                })
                .ToList();

            return Ok(doctors);
        }

        [HttpGet("GetDoctorByNationalId/{nationalId}")]
        public async Task<IActionResult> GetDoctorByNationalId(string nationalId)
        {
            if (string.IsNullOrEmpty(nationalId))
            {
                return BadRequest("National ID is required.");
            }

            var doctor = unitOfWork.Repository<Doctor>()
                .Get(d => d.AppUser.NationalId == nationalId, includeProperties: "AppUser");

            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            var doctorDto = new DoctorByNIdDTO
            {
                Name = doctor.AppUser.UserName,
                Description = doctor.Description,
                Address = doctor.AppUser.Address,
                ProfilePhoto = doctor.AppUser.ProfilePhoto 
            };

            return Ok(doctorDto);
        }


    }
}
