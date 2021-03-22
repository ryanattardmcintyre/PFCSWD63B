using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;
using RestSharp.Authenticators;

namespace EmailSender
{
    class Program
    {
        static void Main(string[] args)
        {

            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\Ryan\Downloads\pfc2021-9ec46b4dd7a6.json");

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
                    text =msg.Message.Data.ToStringUtf8();
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

                    SendSimpleMessage(email, content);
                    emailSent =true;
                }
                catch(Exception ex)
                {
                    emailSent = false;
                }


                // If acknowledgement required, send to server.
                if (acknowledge && messageCount > 0)
                {
                    subscriberClient.Acknowledge(subscriptionName, response.ReceivedMessages.Select(msg => msg.AckId));
                }
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
            {
                // UNAVAILABLE due to too many concurrent pull requests pending for the given subscription.
            }


            //code to send an email
  
        }

        public static IRestResponse SendSimpleMessage(string email, string message)
        { 
            //install RestSharp from nuget

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            "55906d3a001f54dea7f1c36c35380db1-d32d817f-94c875e0");
            RestRequest request = new RestRequest();
          request.AddParameter("domain", "sandbox71aaf3a4083d41fbaf81858054139ab9.mailgun.org", ParameterType.UrlSegment);
           request.Resource = "{domain}/messages";
            request.AddParameter("from", "ryanattard83@gmail.com");
            request.AddParameter("to", email);
            request.AddParameter("subject", "no-reply pubsub test");
            request.AddParameter("text", message);
            request.Method = Method.POST;
            return client.Execute(request);
        }

    }
}
