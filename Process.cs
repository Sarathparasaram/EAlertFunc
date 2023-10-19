//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Azure.Messaging.ServiceBus;
//using Microsoft.Azure.ServiceBus;
//using Microsoft.Azure.ServiceBus.Management;
////using Microsoft.ServiceBus;
////using Microsoft.ServiceBus.Messaging;
//using System.Net.Http;
//using Microsoft.Azure.WebJobs.Host;
//using Microsoft.ServiceBus;//using Microsoft.ApplicationInsights;
//using Microsoft.ApplicationInsights.DataContracts;
//using Microsoft.Azure.ServiceBus.Management;
//using static SBEmailAlertFunction.SendGridLogic;
//using Azure.Messaging.ServiceBus.Administration;
//using Microsoft.Extensions.Configuration;

using static System.Environment;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ApplicationInsights.Channel;
using static SBEmailAlertFunction.SendGridLogic;
using Microsoft.Extensions.Logging;

namespace SBEmailAlertFunction
{
    public class EmailALtertFunction
    {
        private ILogger<EmailALtertFunction> _logger;

        public EmailALtertFunction( ILogger<EmailALtertFunction> logger)
        {
            
            _logger = logger;
        }


        public async Task Run(TimerInfo myTimer, ILogger  log)
        {
            string serviceBusConnectionString = "ServiceBusConnectionString";
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(serviceBusConnectionString);

            int emailThreshold = 100; // Adjust the threshold as needed

            foreach (var topic in await namespaceManager.GetTopicsAsync())
            {
                foreach (var subscription in await namespaceManager.GetSubscriptionsAsync(topic.Path))
                {
                    await LogMessageCountsAsync($"{Escape(topic.Path)}.{Escape(subscription.Name)}", subscription.MessageCountDetails, log, emailThreshold);
                }
            }
            foreach (var queue in await namespaceManager.GetQueuesAsync())
            {
                await LogMessageCountsAsync(Escape(queue.Path), queue.MessageCountDetails, log, emailThreshold);
            }
        }


        private static async Task  LogMessageCountsAsync(string entityName, MessageCountDetails details, ILogger log, int emailThreshold)
        {
            var telemetryClient = new TelemetryClient();
            telemetryClient.InstrumentationKey = "InstrumentationKey";

            var activeMessageCount = details.ActiveMessageCount;
            var telemetry = new TraceTelemetry(entityName);
            telemetry.Properties.Add("Active Message Count", activeMessageCount.ToString());
            telemetry.Properties.Add("Dead Letter Count", details.DeadLetterMessageCount.ToString());

            telemetryClient.TrackMetric(new MetricTelemetry("Active Message Count", activeMessageCount));
            telemetryClient.TrackMetric(new MetricTelemetry("Dead Letter Count", details.DeadLetterMessageCount));

            if (activeMessageCount > emailThreshold)
            {
                // Send an email alert
                SendEmailAlert(entityName, (int)activeMessageCount, emailThreshold);
            }

            telemetryClient.TrackTrace(telemetry);
        }


        private static string Escape(string input) => Regex.Replace(input, @"[^A-Za-z0-9]+", "_");
        private static string Env(string name) => GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

    }
}
