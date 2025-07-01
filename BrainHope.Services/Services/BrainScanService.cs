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
using Newtonsoft.Json;

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

            // Save image locally and get URL
            string imageUrl = await ImageHelper.SaveImageAsync(image);

            // Send image to AI model
            using var stream = new MemoryStream();
            await image.CopyToAsync(stream);
            var imageBytes = stream.ToArray();

            // Create HTTP client
            var client = _httpClientFactory.CreateClient();
            using var content = new MultipartFormDataContent();

            // Prepare the file content
            var fileContent = new ByteArrayContent(imageBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg"); // تأكد من نوع الصورة

            // Add image content to form-data
            content.Add(fileContent, "image", image.FileName); 

            // Send POST request to Flask API
            var response = await client.PostAsync("https://8190-34-90-156-107.ngrok-free.app/predict", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Failed to get prediction from AI model. Details: {errorDetails}");
            }

            // Read prediction result from the API
            var predictionResult = await response.Content.ReadAsStringAsync();
            // Assuming the response is a JSON with 'prediction' and 'confidence'
            var predictionData = JsonConvert.DeserializeObject<Dictionary<string, object>>(predictionResult);
            var prediction = predictionData["prediction"].ToString();
            var confidence = float.Parse(predictionData["confidence"].ToString());

            // Save scan result to DB
            var scanResult = new BrainScanResult
            {
                UserId = userId,
                ImageName = imageUrl,
                PredictionResult = prediction,
                ScanDate = DateTime.UtcNow
            };

            _unitOfWork.Repository<BrainScanResult>().Add(scanResult);
            await _unitOfWork.Complete();

            // Get patient info
            var patient = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(userId);
            var patientName = patient?.UserName ?? "Unknown";

            return new BrainScanResultDTO
            {
                ImageName = imageUrl,
                PredictionResult = prediction,
                PatientName = patientName
            };
        }


    }
}

