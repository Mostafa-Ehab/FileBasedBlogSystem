namespace BlogSystem.Infrastructure.EmailService;

public static class EmailRegistrationTemplate
{
    public static string GetEmailSubject()
    {
        return "Welcome to BlogSystem!";
    }

    public static string GetEmailBody(string fullName, string verificationCode)
    {
        return
    }
}