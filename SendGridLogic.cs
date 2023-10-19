using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace SBEmailAlertFunction
{
    public class SendGridLogic
    {



        public static void SendEmailAlert(string entityName, int activeMessageCount, int emailThreshold)
        {
            var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            var senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
            var recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail");

            var client = new SendGridClient(apiKey);

            var subject = $"Threshold Exceeded Alert for Entity: {entityName}";
            var plainTextContent = $"Threshold exceeded for entity {entityName}. Current message count: {activeMessageCount}. Threshold: {emailThreshold}.";
            var htmlContent = $"<p>Threshold exceeded for entity {entityName}. Current message count: {activeMessageCount}. Threshold: {emailThreshold}.</p>";

            var from = new EmailAddress(senderEmail);
            var to = new EmailAddress(recipientEmail);

            var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            client.SendEmailAsync(message).Wait();
        }

    }

}
