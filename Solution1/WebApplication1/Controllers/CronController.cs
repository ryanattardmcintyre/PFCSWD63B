using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    public class CronController : Controller
    {
        public IActionResult Index()
        {
        //    System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\Ryan\Downloads\pfc2021-9ec46b4dd7a6.json");

            SendEmailViaCloudFunction("ryanattard@gmail.com", "test");


            string projectId = "pfc2021";
            string subscriptionId = "pfc2021subscriptionra2";

            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
            int messageCount = 0;
            bool acknowledge = false;
            string text = "";
            bool emailSent = false;
            try
            {
                // Pull messages from server,
                // allowing an immediate response if there are no messages.
                PullResponse response = subscriberClient.Pull(subscriptionName, returnImmediately: true, maxMessages: 1);
                // Print out each received message.
                string ackId = "";

                foreach (ReceivedMessage msg in response.ReceivedMessages)
                {
                    text = msg.Message.Data.ToStringUtf8();
                    ackId = msg.AckId;
                    Console.WriteLine($"Message {msg.Message.MessageId}: {text}");

                    Interlocked.Increment(ref messageCount);
                }

                try
                {

                    string email = "";
                    string content = "";

                    dynamic myJsonObject = JsonConvert.DeserializeObject(text);
                    email = myJsonObject.email;
                    content = myJsonObject.blog.Title;

                    SendEmailViaCloudFunction(email, content);
                    emailSent = true;
                }
                catch (Exception ex)
                {
                    emailSent = false;
                }

              
                // If acknowledgement required, send to server.
                if (acknowledge && messageCount > 0)
                {
                    subscriberClient.Acknowledge(subscriptionName, response.ReceivedMessages.Select(msg => msg.AckId));
                }
            }
            catch (RpcException ex) when (ex.Status.StatusCode ==  Grpc.Core.StatusCode.Unavailable)
            {
                // UNAVAILABLE due to too many concurrent pull requests pending for the given subscription.
            }
            return Content("done");

        }


        public static void SendEmailViaCloudFunction(string email, string message)
        {
            HttpClient client = new HttpClient();
            Task<string> t = client.GetStringAsync("https://us-central1-pfc2021.cloudfunctions.net/pfcswd63btestfunction?recipient=" + email);
            t.Wait();

            string json = t.Result;

            dynamic myResult = JsonConvert.DeserializeObject(json);

            Console.WriteLine(myResult.Message);
            Console.ReadLine();

        }

    }
}
