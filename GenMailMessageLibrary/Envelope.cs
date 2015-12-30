using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenMailServiceLibrary
{
    public class Envelope
    {
        public MailMessage Message { get; set; }
        public Envelope()
        {
            TS = DateTime.Now;
        }

        public bool IsSuccessfullySent { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime TS { get; set; }
        public string RelatedMessageID { get; set; }
    }
}
