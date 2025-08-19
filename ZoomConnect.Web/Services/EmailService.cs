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
                var hostParts = _options.EmailOptions.SmtpHost.Split(':', StringSplitOptions.RemoveEmptyEntries);
                var host = hostParts[0];
                var port = 587;
                if (hostParts.Length > 1)
                {
                    int.TryParse(hostParts[1], out port);
                }
                client.RequireTLS = true;
                client.Connect(host, port, false);
                if (!String.IsNullOrWhiteSpace(_options.EmailOptions.Username) && !String.IsNullOrWhiteSpace(_options.EmailOptions.Password))
                {
                    client.Authenticate(_options.EmailOptions.Username, _options.EmailOptions.Password.Value);
                }

                messages.ForEach(m => client.Send(m));

                client.Disconnect(true);
            }
        }

        public bool Test()
        {
            try
            {
                var hostParts = _options.EmailOptions.SmtpHost.Split(':', StringSplitOptions.RemoveEmptyEntries);
                var host = hostParts[0];
                var port = 587;
                if (hostParts.Length > 1)
                {
                    int.TryParse(hostParts[1], out port);
                }
                using (var client = new SmtpClient())
                {
                    client.RequireTLS = true;
                    client.Connect(host, port, false);
                    if (!String.IsNullOrWhiteSpace(_options.EmailOptions.Username) && !String.IsNullOrWhiteSpace(_options.EmailOptions.Password))
                    {
                        client.Authenticate(_options.EmailOptions.Username, _options.EmailOptions.Password.Value);
                    }
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
