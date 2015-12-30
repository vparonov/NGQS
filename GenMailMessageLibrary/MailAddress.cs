using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GenMailServiceLibrary
{
    public class MailAddress 
    {
        public string Address { get; set; }

        public string DisplayName { get; set; }
        
        public Encoding DisplayNameEncoding { get; set; }

        public MailAddress() { } 
        public MailAddress(string address)  
        {
            Address = address ;
        }

        public MailAddress(string address, string displayName) 
        {
            Address = address;
            DisplayName = displayName ;
        }
        public MailAddress(string address, string displayName, Encoding displayNameEncoding) 
        {
            Address = address;
            DisplayName = displayName;
            DisplayNameEncoding = displayNameEncoding ;
        //!!!
        }
    }
}
