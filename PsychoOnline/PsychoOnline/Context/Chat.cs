using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PsychoWeb.Context;

namespace PsychoOnline.Context
{
    public class Chat
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public Customer Sender { get; set; }

        public int ReceiverId { get; set; }
        public Customer Receiver { get; set; }

        public DateTime DateTime { get; set; }

        public string Text { get; set; }
    }
}
