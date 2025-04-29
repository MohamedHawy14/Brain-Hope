using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.Medical_History;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrainHope_.Api.Controllers
{
    [Route("medical/[controller]")]
    [ApiController]
    public class MedicalHistoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicalHistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("add/{patientId}")]
        public async Task<IActionResult> Add( string patientId, [FromForm] MedicalHistoryDTO dto)
        {
            var repo = _unitOfWork.Repository<MedicalHistory>();

            var existing = repo.Get(h => h.PatientId == patientId);
            if (existing != null)
                return BadRequest("Medical history already exists. Use update instead.");

            var history = new MedicalHistory
            {
                PatientId = patientId,
                ChronicDiseases = dto.ChronicDiseases,
                Allergies = dto.Allergies,
                Surgeries = dto.Surgeries,
                CurrentMedications = dto.CurrentMedications,
                FamilyHistory = dto.FamilyHistory,
                IsSmoker = dto.IsSmoker,
                DrinksAlcohol = dto.DrinksAlcohol,
                Notes = dto.Notes,
                LastUpdated = DateTime.UtcNow
            };

            repo.Add(history);
            await _unitOfWork.Complete();

            var resultDto = new MedicalHistoryDTO
            {
                ChronicDiseases = history.ChronicDiseases,
                Allergies = history.Allergies,
                Surgeries = history.Surgeries,
                CurrentMedications = history.CurrentMedications,
                FamilyHistory = history.FamilyHistory,
                IsSmoker = history.IsSmoker,
                DrinksAlcohol = history.DrinksAlcohol,
                Notes = history.Notes
            };

            return Ok(resultDto);
        }

        [HttpGet("get/{patientId}")]
        public IActionResult GetMedicalHistory( string patientId)
        {
            var repo = _unitOfWork.Repository<MedicalHistory>();

            var history = repo.Get(h => h.PatientId == patientId);
            if (history == null)
                return NotFound("Medical history not found");

            var dto = new MedicalHistoryDTO
            {
                ChronicDiseases = history.ChronicDiseases,
                Allergies = history.Allergies,
                Surgeries = history.Surgeries,
                CurrentMedications = history.CurrentMedications,
                FamilyHistory = history.FamilyHistory,
                IsSmoker = history.IsSmoker,
                DrinksAlcohol = history.DrinksAlcohol,
                Notes = history.Notes
            };

            return Ok(dto);
        }

        //[HttpPut("update")]
        //public async Task<IActionResult> UpdateMedicalHistory([FromQuery] string patientId, [FromForm] MedicalHistoryEditDTO dto)
        //{
        //    var repo = _unitOfWork.Repository<MedicalHistory>();
        //    var history = repo.Get(h => h.PatientId == patientId);

        //    if (history == null)
        //        return NotFound("Medical history not found");

        //    // Apply changes only if new values are provided
        //    if (!string.IsNullOrWhiteSpace(dto.ChronicDiseases)) history.ChronicDiseases = dto.ChronicDiseases;
        //    if (!string.IsNullOrWhiteSpace(dto.Allergies)) history.Allergies = dto.Allergies;
        //    if (!string.IsNullOrWhiteSpace(dto.Surgeries)) history.Surgeries = dto.Surgeries;
        //    if (!string.IsNullOrWhiteSpace(dto.CurrentMedications)) history.CurrentMedications = dto.CurrentMedications;
        //    if (!string.IsNullOrWhiteSpace(dto.FamilyHistory)) history.FamilyHistory = dto.FamilyHistory;
        //    if (!string.IsNullOrWhiteSpace(dto.Notes)) history.Notes = dto.Notes;

        //    if (dto.IsSmoker.HasValue) history.IsSmoker = dto.IsSmoker.Value;
        //    if (dto.DrinksAlcohol.HasValue) history.DrinksAlcohol = dto.DrinksAlcohol.Value;

        //    history.LastUpdated = DateTime.UtcNow;

        //    repo.Update(history);
        //    await _unitOfWork.Complete();

        //    var meddto = new MedicalHistoryDTO
        //    {
        //        ChronicDiseases = history.ChronicDiseases,
        //        Allergies = history.Allergies,
        //        Surgeries = history.Surgeries,
        //        CurrentMedications = history.CurrentMedications,
        //        FamilyHistory = history.FamilyHistory,
        //        IsSmoker = history.IsSmoker,
        //        DrinksAlcohol = history.DrinksAlcohol,
        //        Notes = history.Notes
        //    };


        //    return Ok(meddto);
        //}

        [HttpPatch("update/{patientId}")]
        public async Task<IActionResult> Update(string patientId, [FromForm] MedicalHistoryEditDTO dto)
        {
            var repo = _unitOfWork.Repository<MedicalHistory>();
            var history = repo.Get(h => h.PatientId == patientId);

            if (history == null)
                return NotFound("Medical history not found.");

            // تعديل فقط القيم اللي اتبعتت
            if (dto.ChronicDiseases != null) history.ChronicDiseases = dto.ChronicDiseases;
            if (dto.Allergies != null) history.Allergies = dto.Allergies;
            if (dto.Surgeries != null) history.Surgeries = dto.Surgeries;
            if (dto.CurrentMedications != null) history.CurrentMedications = dto.CurrentMedications;
            if (dto.FamilyHistory != null) history.FamilyHistory = dto.FamilyHistory;
            if (dto.IsSmoker.HasValue) history.IsSmoker = dto.IsSmoker.Value;
            if (dto.DrinksAlcohol.HasValue) history.DrinksAlcohol = dto.DrinksAlcohol.Value;
            if (dto.Notes != null) history.Notes = dto.Notes;

            history.LastUpdated = DateTime.UtcNow;

            repo.Update(history);
            await _unitOfWork.Complete();

            var meddto = new MedicalHistoryDTO
            {
                ChronicDiseases = history.ChronicDiseases,
                Allergies = history.Allergies,
                Surgeries = history.Surgeries,
                CurrentMedications = history.CurrentMedications,
                FamilyHistory = history.FamilyHistory,
                IsSmoker = history.IsSmoker,
                DrinksAlcohol = history.DrinksAlcohol,
                Notes = history.Notes
            };

            return Ok(meddto);
        }





    }
}
