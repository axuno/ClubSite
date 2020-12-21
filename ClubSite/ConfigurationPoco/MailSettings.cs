//
// Copyright (C) axuno gGmbH and other contributors.
// Licensed under the MIT license.
//

namespace ClubSite.ConfigurationPoco
{
    public class MailSettings
    {
        public Sender Sender { get; set; }
        public Message Message { get; set; }
    }

    public class Sender
    {
        public string MessageOutput { get; set; }
        public Smtp Smtp { get; set; }
        public File File { get; set; }
    }

    public class Smtp
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
    }

    public class File
    {
        public string Path { get; set; }
    }

    public class Message
    {
        public MailAddress DefaultFrom { get; set; }
        public MailAddress[] ContactFormTo { get; set; }
        public string Organization { get; set; }
    }

    public class MailAddress
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
