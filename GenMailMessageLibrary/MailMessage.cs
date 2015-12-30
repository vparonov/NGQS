using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;

namespace GenMailServiceLibrary
{
    public class MailMessage
    {

        public class PropertyPair
        {
            public string Key { set; get; }
            public string Value { set; get; }

            public PropertyPair() { } 
        }

        public MailMessage()
        {
            To = new List<MailAddress>() ;
            BCC = new List<MailAddress>();
            CC = new List<MailAddress>();
            Attachments = new List<Attachment>();
            DeliveryNotificationOptions = DeliveryNotificationOptions.None;
            IsBodyHtml = false;
            Encoding = "utf-8"; // ??? TODO - дали това трябва да е дефолтающ
            Priority = MailPriority.Normal;
            Properties = new List<PropertyPair>();

            MessageID = String.Format("{0}@{1}", Guid.NewGuid().ToString(), "netgen.applss.com");
        }

        public void AddProperty(string key, string value)
        {
            Properties.Add(
                new PropertyPair()
                {
                    Key = key,
                    Value = value
                });
        }

        public MailAddress From { set; get; }
        public MailAddress Sender { set; get; }

        public string Subject { set; get; }

        public string Body
        {
            set;
            get;
        }

        public DeliveryNotificationOptions DeliveryNotificationOptions { get; set; }

        public bool IsBodyHtml { set; get; }

        public MailPriority Priority { set; get; }

        public string Encoding { set; get; }

        public List<MailAddress> To { set; get; }

        public List<MailAddress> BCC { set; get; }

        public List<MailAddress> CC { set; get; }

        public List<Attachment> Attachments { set; get; }

        public List<PropertyPair> Properties { set; get; }

        public string MessageID { set; get; }
    }
}
