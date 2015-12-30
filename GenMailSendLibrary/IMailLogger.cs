using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenMailServiceLibrary
{
    public interface IMailLogger
    {
        bool Log(MailMessage message);
    }
}
