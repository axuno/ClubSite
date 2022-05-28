// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using ClubSite.ConfigurationPoco;
using Microsoft.Extensions.Logging;
using MimeKit.Text;

namespace ClubSite.Services
{
    public interface IMailService
    {
        Task SendTextEmailAsync(string name, string email, string subject, string body,
            CancellationToken cancellationToken);

        Task SendContactFormEmailAsync(string fromName, string fromEmail, string subject, string plainBody,
            CancellationToken cancellationToken);
        
        Task SendTournamentRegistrationEmailAsync(string fromName, string fromEmail, string subject, string plainBody,
            CancellationToken cancellationToken);


        Task SendEmailAsync(MimeMessage mimeMessage, CancellationToken cancellationToken);
        MailSettings Settings { get; }
    }

    public class MailService : IMailService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> smtpSettings, IWebHostEnvironment env, ILogger<MailService> logger)
        {
            Settings = smtpSettings.Value;
            _env = env;
            _logger = logger;
        }

        public MailSettings Settings { get; }

        public async Task SendTextEmailAsync(string toName, string toEmail, string subject, string plainBody,
            CancellationToken cancellationToken)
        {
            var message = new MimeMessage();
            message.Headers.Add(HeaderId.Organization, Settings.Message.Organization ?? string.Empty);
            message.From.Add(new MailboxAddress(Settings.Message.DefaultFrom.Name, Settings.Message.DefaultFrom.Email));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Text) {
                Text = plainBody
            };

            await SendEmailAsync(message, cancellationToken);
        }

        public async Task SendContactFormEmailAsync(string fromName, string fromEmail, string subject, string plainBody,
            CancellationToken cancellationToken)
        {
            var message = new MimeMessage();
            message.Headers.Add(HeaderId.Organization, Settings.Message.Organization ?? string.Empty);
            message.ReplyTo.Add(new MailboxAddress(fromName, fromEmail));
            message.From.Add(new MailboxAddress(Settings.Message.DefaultFrom.Name, Settings.Message.DefaultFrom.Email));

            foreach (var mailAddress in Settings.Message.ContactFormTo)
                message.To.Add(new MailboxAddress(mailAddress.Name, mailAddress.Email));

            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Text) {
                Text = plainBody
            };

            await SendEmailAsync(message, cancellationToken);
        }

        public async Task SendTournamentRegistrationEmailAsync(string fromName, string fromEmail, string subject, string plainBody,
            CancellationToken cancellationToken)
        {
            var message = new MimeMessage();
            message.Headers.Add(HeaderId.Organization, Settings.Message.Organization ?? string.Empty);
            message.ReplyTo.Add(new MailboxAddress(fromName, fromEmail));
            message.From.Add(new MailboxAddress(Settings.Message.DefaultFrom.Name, Settings.Message.DefaultFrom.Email));
            
            foreach (var mailAddress in Settings.Message.ContactFormTo)
                message.To.Add(new MailboxAddress(mailAddress.Name, mailAddress.Email));

            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Text) {
                Text = plainBody
            };

            await SendEmailAsync(message, cancellationToken);
        }

        public async Task SendEmailAsync(MimeMessage mimeMessage, CancellationToken cancellationToken)
        {
            var messageOutput = _env.IsDevelopment() ? nameof(Sender.File) : nameof(Sender.Smtp);
            try
            {
                if (messageOutput == nameof(Sender.Smtp))
                {
                    using var client = new SmtpClient {
                        Timeout = Settings.Sender.Smtp.Timeout,
                        ServerCertificateValidationCallback = (s, c, h, e) => true
                    };
                    await client.ConnectAsync(Settings.Sender.Smtp.Server, Settings.Sender.Smtp.Port, true,
                        cancellationToken);
                    await client.AuthenticateAsync(Settings.Sender.Smtp.Username, Settings.Sender.Smtp.Password,
                        cancellationToken);
                    await client.SendAsync(mimeMessage, cancellationToken);
                    await client.DisconnectAsync(true, cancellationToken);
                }
                else
                {
                    var filename = Path.IsPathFullyQualified(Settings.Sender.File.Path)
                        ? Path.Combine(Settings.Sender.File.Path, mimeMessage.MessageId + ".eml")
                        : Path.Combine(_env.ContentRootPath, Settings.Sender.File.Path, mimeMessage.MessageId + ".eml");
                    await mimeMessage.WriteToAsync(new FormatOptions {MaxLineLength = 100}, filename,
                        cancellationToken);
                }

                _logger.LogInformation("Sending mail to '{messageOutput}' succeeded.\n{mimeMessage}", messageOutput, mimeMessage.ToString());
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Sending mail to '{messageOutput}' failed.\n{mimeMessage}", messageOutput, mimeMessage.ToString());
                throw;
            }
        }
    }
}