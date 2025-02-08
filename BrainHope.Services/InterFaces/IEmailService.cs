using BrainHope.Services.DTO.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.InterFaces
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
