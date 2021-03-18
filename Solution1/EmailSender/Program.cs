using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmailSender
{
    class Program
    {
       async static void Main(string[] args)
        {
            string projectId = "pfc2021";
            string subscriptionId = "pfc2021subscriptionra";


            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);

            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
            
            var messages = new List<PubsubMessage>();
            var texts = new List<string>();
            Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                string text = message.Data.ToStringUtf8();
                texts.Add(text);
                return Task.FromResult( SubscriberClient.Reply.Ack);
            });
            // Run for 7 seconds.
            // Lets make sure that the start task finished successfully after the call to stop.
            await startTask;


            //then send email

            var msg = texts[0];
            dynamic deserializedObject = JsonConvert.DeserializeObject<dynamic>(msg);

            string email = deserializedObject.email;
            dynamic blog = deserializedObject.blog;

            //code to send email


        }
    }
}
