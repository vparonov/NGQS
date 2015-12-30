using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenMailServiceLibrary
{
    public interface IMailSender
    {
        void Send(MailMessage msg, MailSenderConfiguration cfg);
    }
}
