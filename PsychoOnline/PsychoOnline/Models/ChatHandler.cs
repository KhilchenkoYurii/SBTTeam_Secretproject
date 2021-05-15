using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketManager;

namespace PsychoWeb.Models
{
    public class ChatHandler : WebSocketHandler
    {
        private readonly ChatManager _chatManager;
        public ChatHandler(WebSocketConnectionManager webSocketConnectionManager, ChatManager chatManager) : base(webSocketConnectionManager)
        {
            _chatManager = chatManager;
        }

        public async Task SendMessage(string userName, string message)
        {
            dynamic dynamicMessage = new ExpandoObject();
            dynamicMessage.UserName = userName;
            dynamicMessage.Message = message;
            _chatManager.Messages.Add(dynamicMessage);
            await InvokeClientMethodToAllAsync("pingMessage", userName, message);
        }
    }
}
