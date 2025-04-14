using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.BookAppointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilites;

namespace BrainHope_.Api.Controllers
{
    [Route("Appointment/[controller]")]
    [ApiController]
    //[Authorize(Roles = SD.Role_Patient)]
    public class BookAppointmentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public BookAppointmentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var query = unitOfWork.Repository<Doctor>()
                .GetAllQuery(includeProperties: "AppUser")
                .AsQueryable();

           

            var doctors = await query
                .OrderByDescending(d => d.Rate)
                .Select(d => new AllDoctorDTO
                {
                    Id=d.AppUser.Id,
                    Name = d.AppUser.UserName,
                    Description = d.AppUser.Description,
                    Rate = d.Rate,
                    ProfilePhoto = d.AppUser.ProfilePhoto
                })
                .ToListAsync(); 

            return Ok(doctors);
        }


        [HttpGet("GetDoctorByUserId/{userId}")]
        public IActionResult GetDoctorByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var doctor = unitOfWork.Repository<Doctor>()
                            .Get(d => d.UserId == userId, includeProperties: "AppUser");

            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            var doctorDto = new DoctorByUIdDTO
            {
                Id=doctor.AppUser.Id,
                Name = doctor.AppUser.UserName,
                Description = doctor.AppUser.Description,
                Address = doctor.AppUser.Address,
                ProfilePhoto = doctor.AppUser.ProfilePhoto,
                CalendlyLink = !string.IsNullOrEmpty(doctor.CalendlyLink)  ? doctor.CalendlyLink : "No Calendly link available",
                PhoneNumber=doctor.AppUser.PhoneNumber
            };

            return Ok(doctorDto);
        }

        [HttpGet("GetDoctorCalendlyLink/{userId}")]
        public async Task<IActionResult> GetDoctorCalendlyLink(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var doctor = unitOfWork.Repository<Doctor>()
                            .Get(d => d.UserId == userId, includeProperties: "AppUser");

            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            if (string.IsNullOrEmpty(doctor.CalendlyLink))
            {
                return BadRequest("This doctor has not set up a Calendly link.");
            }

            return Ok(new { calendlyLink = doctor.CalendlyLink });
        }




    }
}
