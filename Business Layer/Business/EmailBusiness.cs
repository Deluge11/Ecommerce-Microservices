

using Data_Layer.Data;



namespace Business_Layer.Business;

public class EmailBusiness 
{
    public EmailData EmailData { get; }

    public EmailBusiness(EmailData emailData)
    {
        EmailData = emailData;
    }


    public async Task<bool> EmailExists(string email)
    {
        email = email.Trim();

        if (email.Length < 1 || !email.Contains("@"))
        {
            return false;
        }
        return await EmailData.EmailExists(email);
    }
}
