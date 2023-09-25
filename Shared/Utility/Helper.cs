using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Shared.Utility;

public class Helper
{
    public static bool IsValidEmail(string email)
    {
        var validFromRegex = Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        if (validFromRegex)
            return true;

        if (!MailAddress.TryCreate(email, out var mailAddress))
            return false;

        // And if you want to be more strict:
        var hostParts = mailAddress.Host.Split('.');
        if (hostParts.Length == 1)
            return false; // No dot.
        if (hostParts.Any(p => p == string.Empty))
            return false; // Double dot.
        if (hostParts[^1].Length < 2)
            return false; // TLD only one letter.

        if (mailAddress.User.Contains(' '))
            return false;
        if (mailAddress.User.Split('.').Any(p => p == string.Empty))
            return false; // Double dot or dot at end of user part.

        return true;
    }

    public static string ValidateAndReturnItalianPhoneNumber(string phoneNumber)
    {
        if (phoneNumber.StartsWith("+39"))
            return phoneNumber;

        if (phoneNumber.StartsWith("39"))
            return $"+{phoneNumber}";

        if (phoneNumber.StartsWith("0039"))
            return $"+{phoneNumber.Substring(2, phoneNumber.Length - 2)}";

        return $"+39{phoneNumber}";
    }

    public static bool IsValidItalianNumber(string number)
    {
        var regex = new Regex(@"^(\+39)?(0|3)\d{4,11}$");
        return regex.IsMatch(number);
    }

    public static string RemoveWhitespace(string input)
    {
        return new string(input.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());
    }

    public static string AddItalianPrefix(string phoneNumber)
    {
        phoneNumber = RemoveWhitespace(phoneNumber);

        if (phoneNumber.StartsWith("+39") && phoneNumber.Length == 13)
            return phoneNumber;
        else if (phoneNumber.StartsWith("0039") && phoneNumber.Length == 14)
            return "+39" + phoneNumber.Substring(4);
        else if (phoneNumber.StartsWith("39") && phoneNumber.Length == 12)
            return "+" + phoneNumber;
        else if (phoneNumber.StartsWith("0") && phoneNumber.Length == 10)
            return "+39" + phoneNumber.Substring(1);
        else if (phoneNumber.Length == 10)
            return "+39" + phoneNumber;
        else
            return ""; // Invalid phone number
    }

    public static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        char[] charArray = input.ToCharArray();
        charArray[0] = char.ToUpper(charArray[0]);
        return new string(charArray);
    }
}
