using BrainHope.DataAcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Authentication.User
{
    public class CreateUserResponse
    {
        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        [JsonIgnore]
        public string? Token { get; set; }
    }
}
