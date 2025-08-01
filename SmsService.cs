using Twilio;
using Twilio.Rest.Api.V2010.Account;

public class SmsService
{
    private const string AccountSid = ""; // Отримайте з Twilio
    private const string AuthToken = "";   // Отримайте з Twilio
    private const string FromPhoneNumber = ""; //  номер телефону у Twilio

    public SmsService()
    {
        TwilioClient.Init(AccountSid, AuthToken);
    }

    public void SendSms(string toPhoneNumber, string message)
    {
        MessageResource.Create(
            body: message,
            from: new Twilio.Types.PhoneNumber(FromPhoneNumber),
            to: new Twilio.Types.PhoneNumber(toPhoneNumber)
        );
    }
}
