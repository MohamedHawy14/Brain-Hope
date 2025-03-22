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
        public async Task SendPostUpdate(int postId)
        {
            await Clients.All.SendAsync("ReceivePostUpdate", postId);
        }

        public async Task SendCommentUpdate(int postId, int commentId, string content)
        {
            await Clients.All.SendAsync("ReceiveCommentUpdate", postId, commentId, content);
        }

        public async Task SendLikeUpdate(int postId, string userId)
        {
            await Clients.All.SendAsync("ReceiveLikeUpdate", postId, userId);
        }

        public async Task SendUnlikeUpdate(int postId, string userId)
        {
            await Clients.All.SendAsync("ReceiveUnlikeUpdate", postId, userId);
        }
    }
}
