// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

namespace ClubSite.ConfigurationPoco
{
    public class MailSettings
    {
        public Sender Sender { get; set; } = new Sender();
        public Message Message { get; set; } = new Message();
    }

    public class Sender
    {
        public string MessageOutput { get; set; } = string.Empty;
        public Smtp Smtp { get; set; } = new Smtp();
        public File File { get; set; } = new File();
    }

    public class Smtp
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Timeout { get; set; } = 30;
    }

    public class File
    {
        public string Path { get; set; } = string.Empty;
    }

    public class Message
    {
        public MailAddress DefaultFrom { get; set; } = new MailAddress();
        public MailAddress[] ContactFormTo { get; set; } = { };
        public string? Organization { get; set; }
    }

    public class MailAddress
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}