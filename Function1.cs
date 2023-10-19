using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace SBEmailAlertFunction
{
    //public class Function1
    //{
    //    readonly EmailALtertFunction _emailAlertFunction;

    //    public Function1 (EmailALtertFunction emailAlertFunction)
    //    {
    //        _emailAlertFunction = emailAlertFunction;
    //    }

    //    [FunctionName("Function1")]
    //    public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
    //         ExecutionContext context, ILogger log)
    //    {
    //        log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
    //        try
    //        {
    //            await _emailAlertFunction.Run(myTimer, log);
    //        }
    //        catch (Exception ex)
    //        {
    //            log.LogInformation($"Exception raised in :", ex.Message);
    //        }
    //    }
    //}


        public class EmailAlertFunction1
    {
            readonly EmailALtertFunction _emailAlertFunction;

            public EmailAlertFunction1 (EmailALtertFunction emailAlertFunction)
            {
                _emailAlertFunction = emailAlertFunction;
            }

            [FunctionName("EmailAlertFunction1")]
            public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
                 ExecutionContext context, ILogger log)
            {
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                try
                {
                    await _emailAlertFunction.Run(myTimer, log);
                }
                catch (Exception ex)
                {
                    log.LogInformation($"Exception raised: {ex.Message}");
                }
            }
        }
    

}
