using MailKit.Net.Smtp;
using MimeKit;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Services
{
    public class EmailService
    {
        private ZoomOptions _options;

        public EmailService(SecretConfigManager<ZoomOptions> optionsManager)
        {
            _options = optionsManager.GetValue().Result;
        }

        public void Send(List<MimeMessage> messages)
        {
            using (var client = new SmtpClient())
            {
                client.Connect(_options.EmailOptions.smtpHost, 587, false);
                client.Authenticate(_options.EmailOptions.username, _options.EmailOptions.password.Value);

                messages.ForEach(m => client.Send(m));

                client.Disconnect(true);
            }
        }

        public bool Test()
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(_options.EmailOptions.smtpHost, 587, false);
                    client.Authenticate(_options.EmailOptions.username, _options.EmailOptions.password.Value);
                    client.Disconnect(true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
