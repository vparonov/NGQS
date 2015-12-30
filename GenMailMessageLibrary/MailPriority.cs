using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenMailServiceLibrary
{
    // Summary:
    //     Specifies the priority of a System.Net.Mail.MailMessage.
    public enum MailPriority
    {
        // Summary:
        //     The email has normal priority.
        Normal = 0,
        //
        // Summary:
        //     The email has low priority.
        Low = 1,
        //
        // Summary:
        //     The email has high priority.
        High = 2,
    }
}
