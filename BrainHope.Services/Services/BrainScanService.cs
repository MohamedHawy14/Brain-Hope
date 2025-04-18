using BrainHope.DataAcess.Models.AiModel;
using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.AiModel;
using BrainHope.Services.InterFaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace BrainHope.Services.Services
{
    public class BrainScanService : IBrainScanService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUnitOfWork _unitOfWork;

        public BrainScanService(IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork)
        {
            _httpClientFactory = httpClientFactory;
            _unitOfWork = unitOfWork;
        }

        public async Task<BrainScanResultDTO> AnalyzeBrainScanAsync(IFormFile image, string userId)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No image uploaded.");

            // 1. Save image locally and get URL
            string imageUrl = await ImageHelper.SaveImageAsync(image);

            // 2. Send image to AI model
            using var stream = new MemoryStream();
            await image.CopyToAsync(stream);
            var imageBytes = stream.ToArray();

            var client = _httpClientFactory.CreateClient();
            using var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(imageBytes), "file", image.FileName);

            var response = await client.PostAsync("https://17a4-34-57-166-153.ngrok-free.app/predict", content);
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException("Failed to get prediction from AI model.");

            var predictionResult = await response.Content.ReadAsStringAsync();

            // 3. Save to DB
            var scanResult = new BrainScanResult
            {
                PatientId = userId,
                ImageName = imageUrl,
                PredictionResult = predictionResult,
                ScanDate = DateTime.UtcNow
            };

            _unitOfWork.Repository<BrainScanResult>().Add(scanResult);
            await _unitOfWork.Complete();

            // ✅ 4. Get patient info from ApplicationUserRepository
            var patient = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(userId);
            var patientName = patient?.UserName ?? "Unknown";

            return new BrainScanResultDTO
            {
                ImageName = imageUrl,
                PredictionResult = predictionResult,
                PatientName = patientName
            };
        }
    }
}

