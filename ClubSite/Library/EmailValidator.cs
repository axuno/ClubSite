// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite
//

namespace ClubSite.Library;

public static class EmailValidator
{
    public static bool IsValid(string? email)
    {
        try
        {
            _ = new MimeKit.MailboxAddress(email, email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}