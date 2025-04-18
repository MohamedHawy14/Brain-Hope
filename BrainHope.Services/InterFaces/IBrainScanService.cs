using BrainHope.Services.DTO.AiModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.InterFaces
{
    public interface IBrainScanService
    {
        Task<BrainScanResultDTO> AnalyzeBrainScanAsync(IFormFile image, string userId);
    }

}
