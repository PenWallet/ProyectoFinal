using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIR_Entities.Persistent
{
    public class Message
    {
        public long ID { get; set; }
        public string text { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public DateTime date { get; set; }

        public Message(long ID, string text, string sender, string receiver, DateTime date)
        {
            this.ID = ID;
            this.text = text;
            this.sender = sender;
            this.receiver = receiver;
            this.date = date;
        }

        public Message()
        {
        }
    }
}
