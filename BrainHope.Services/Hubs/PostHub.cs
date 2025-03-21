using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.Hubs
{
    public class PostHub : Hub
    {
        public async Task SendPostUpdate()
        {
            await Clients.All.SendAsync("ReceivePostUpdate");
        }
    }
}
