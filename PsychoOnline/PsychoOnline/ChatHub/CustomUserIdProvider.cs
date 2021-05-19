using System;
using Microsoft.AspNetCore.SignalR;

namespace Psycho.io.ChatForUser
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            var kek = connection.User?.Identity.Name;
            return connection.User?.Identity.Name;
        }
    }
}
