using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Reflection;

namespace GenMailServiceLibrary
{
    public static class MailMessageConversionExtensions
    {
        public static string ToEml(this MailMessage msg)
        {
            var message = msg.ToSystemNetMailMessage();

            var assembly = typeof(SmtpClient).Assembly;
            var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

            using (var memoryStream = new MemoryStream())
            {
                // Get reflection info for MailWriter contructor
                var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

                // Construct MailWriter object with our FileStream
                var mailWriter = mailWriterContructor.Invoke(new object[] { memoryStream });

                // Get reflection info for Send() method on MailMessage
                var sendMethod = typeof(System.Net.Mail.MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call method passing in MailWriter
                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true, true }, null);

                // Finally get reflection info for Close() method on our MailWriter
                var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public static System.Net.Mail.MailMessage ToSystemNetMailMessage(this MailMessage msg)
        {
            var readyMessage = new System.Net.Mail.MailMessage();
            readyMessage.Subject = msg.Subject;
            readyMessage.Headers.Add("Message-ID", msg.MessageID);

            if (msg.From != null)
            {
                readyMessage.From = msg.From.ToSystemNetMailAddress();
            }

            if (msg.Sender != null)
            {
                readyMessage.Sender = msg.Sender.ToSystemNetMailAddress();
            }

            foreach (var t in msg.To)
            {
                readyMessage.To.Add(t.ToSystemNetMailAddress());
            }
            foreach (var c in msg.CC)
            {
                readyMessage.CC.Add(c.ToSystemNetMailAddress());
            }
            foreach (var b in msg.BCC)
            {
                readyMessage.Bcc.Add(b.ToSystemNetMailAddress());
            }

            readyMessage.Body = addMessageIDStampToBody(msg);

            foreach (var a in msg.Attachments)
            {
                readyMessage.Attachments.Add(a.ToSytemNetMailAttachment());
            }

            // тъй като моят тип DeliveryNotificationOptions е копие на .NET-ския
            // си позволявам този каст
            readyMessage.DeliveryNotificationOptions = (System.Net.Mail.DeliveryNotificationOptions)msg.DeliveryNotificationOptions;

            // поддържаме един и същи енкодинг на subject / body
            readyMessage.BodyEncoding = msg.Encoding.ToEncoding();
            readyMessage.SubjectEncoding = msg.Encoding.ToEncoding();

            readyMessage.IsBodyHtml = msg.IsBodyHtml;

            // тъй като моят тип MailPriority е копие на .NET-ския
            // си позволявам този каст
            readyMessage.Priority = (System.Net.Mail.MailPriority)msg.Priority;

            return readyMessage;
        }

        public static MailMessage ToGenMailServiceLibraryMailMessage(this System.Net.Mail.MailMessage msg)
        {
            var readyMessage = new MailMessage();
            readyMessage.Subject = msg.Subject;
            readyMessage.MessageID = msg.Headers["Message-ID"];
            if (msg.From != null)
            {
                readyMessage.From = msg.From.ToGenMailServiceLibraryMailAddress();
            }

            if (msg.Sender != null)
            {
                readyMessage.Sender = msg.Sender.ToGenMailServiceLibraryMailAddress();
            }

            foreach (var t in msg.To)
            {
                readyMessage.To.Add(t.ToGenMailServiceLibraryMailAddress());
            }
            foreach (var c in msg.CC)
            {
                readyMessage.CC.Add(c.ToGenMailServiceLibraryMailAddress());
            }
            foreach (var b in msg.Bcc)
            {
                readyMessage.BCC.Add(b.ToGenMailServiceLibraryMailAddress());
            }

            readyMessage.Body = msg.Body;

            foreach (var a in msg.Attachments)
            {
                readyMessage.Attachments.Add(a.GenMailServiceLibraryAttachment());
            }

            // тъй като моят тип DeliveryNotificationOptions е копие на .NET-ския
            // си позволявам този каст
            readyMessage.DeliveryNotificationOptions = (DeliveryNotificationOptions)msg.DeliveryNotificationOptions;

            // поддържаме един и същи енкодинг на subject / body
            readyMessage.Encoding = msg.BodyEncoding.WebName;

            readyMessage.IsBodyHtml = msg.IsBodyHtml;

            // тъй като моят тип MailPriority е копие на .NET-ския
            // си позволявам този каст
            readyMessage.Priority = (MailPriority)msg.Priority;

            return readyMessage; 
        }

        public static System.Net.Mail.MailAddress ToSystemNetMailAddress(this MailAddress addr)
        {
            //var res = new System.Net.Mail.MailAddress(
            if (addr.DisplayName != null && addr.DisplayNameEncoding != null)
            {
                return new System.Net.Mail.MailAddress(addr.Address, addr.DisplayName, addr.DisplayNameEncoding);
            }
            else if (addr.DisplayName != null)
            {
                return new System.Net.Mail.MailAddress(addr.Address, addr.DisplayName);
            }
            else
            {
                return new System.Net.Mail.MailAddress(addr.Address);
            }
        }

        public static MailAddress ToGenMailServiceLibraryMailAddress(this System.Net.Mail.MailAddress addr)
        {
            if (addr.DisplayName != null)
            {
                return new MailAddress(addr.Address, addr.DisplayName);
            }
            else
            {
                return new MailAddress(addr.Address);
            }
        }

        public static System.Net.Mail.Attachment ToSytemNetMailAttachment(this Attachment attch)
        {
            var stream = new MemoryStream(attch.Body);

            return new System.Net.Mail.Attachment(stream, attch.Name, attch.MimeType); 
        }

        public static Attachment GenMailServiceLibraryAttachment(this System.Net.Mail.Attachment attch)
        {
            return new Attachment(attch.ContentStream, attch.Name, attch.ContentType.Name);
        }

        public static HigLabo.Net.Smtp.SmtpMessage ToSystemHigLaboSmtpMessage(this MailMessage msg)
        {
            var readyMessage = new HigLabo.Net.Smtp.SmtpMessage();

            readyMessage.Subject = msg.Subject;
            readyMessage.Header.Add(new HigLabo.Net.InternetTextMessage.Field("Message-ID", msg.MessageID));

            if (msg.From != null)
            {
                readyMessage.From = msg.From.ToHigLaboMailAddress();
            }
            else if (msg.Sender != null)
            {
                readyMessage.From = msg.Sender.ToHigLaboMailAddress();
            }

            foreach (var t in msg.To)
            {
                readyMessage.To.Add(t.ToHigLaboMailAddress());
            }
            foreach (var c in msg.CC)
            {
                readyMessage.Cc.Add(c.ToHigLaboMailAddress());
            }
            foreach (var b in msg.BCC)
            {
                readyMessage.Bcc.Add(b.ToHigLaboMailAddress());
            }

            readyMessage.HeaderTransferEncoding = HigLabo.Net.Mail.TransferEncoding.Base64;
            readyMessage.ContentTransferEncoding = HigLabo.Net.Mail.TransferEncoding.Base64;

            readyMessage.IsHtml = msg.IsBodyHtml;

            readyMessage.BodyText = addMessageIDStampToBody(msg);

            foreach (var a in msg.Attachments)
            {
                var ct = new HigLabo.Net.Smtp.SmtpContent();
                ct.Name = a.Name;
                ct.FileName = a.Name;
                ct.ContentTransferEncoding = HigLabo.Net.Mail.TransferEncoding.Base64;
                ct.LoadData(a.MimeType, a.Body);
                readyMessage.Contents.Add(ct);
               // readyMessage.Attachments.Add(a.ToSytemNetMailAttachment());
            }

            // поддържаме един и същи енкодинг на subject / body
            readyMessage.ContentEncoding = msg.Encoding.ToEncoding();
            return readyMessage;
        }

        private static string addMessageIDStampToBody(MailMessage msg)
        {
            var sb = new StringBuilder(msg.Body);

            if (msg.IsBodyHtml == false)
            {
                sb.AppendFormat("\r\nMessageID:({0})", msg.MessageID);
            }
            else
            {
                sb.Replace("</body>", String.Format(@"<div style=""display:none;""><p>MessageID:({0})</div></body>", msg.MessageID));
                sb.Replace("</BODY>", String.Format(@"<div style=""display:none;""><p>MessageID:({0})</div></BODY>", msg.MessageID));
            }
            return sb.ToString();
        }

        public static HigLabo.Net.Mail.MailAddress ToHigLaboMailAddress(this MailAddress addr)
        {
            if (addr.DisplayName != null)
            {
                return new HigLabo.Net.Mail.MailAddress(addr.Address, addr.DisplayName);
            }
            else
            {
                return new HigLabo.Net.Mail.MailAddress(addr.Address);
            }
        }

    }
}
